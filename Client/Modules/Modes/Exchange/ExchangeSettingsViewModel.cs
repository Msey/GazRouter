using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Browser;
using System.Windows.Data;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.DTO.Exchange.ExchangeSettings;
using GazRouter.Utils.Extensions;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using UriBuilder = GazRouter.DataProviders.UriBuilder;

namespace GazRouter.Modes.Exchange
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ExchangeSettingsViewModel : MainViewModelBase
    {
        public ExchangeSettingsViewModel()
        {
            ExchangeSettingsList = new RangeEnabledObservableCollection<ExchangeTaskDTO>();
            AddCommand = new DelegateCommand(Add);
            EditCommand = new DelegateCommand(Edit, () => SelectedExchangeSettings != null);
            DeleteCommand = new DelegateCommand(Delete, () => SelectedExchangeSettings != null);
            CloneCommand = new DelegateCommand(Clone, () => SelectedExchangeSettings != null);
            RunCommand = new DelegateCommand(Run, () => SelectedTimeStamp.HasValue && SelectedExchangeSettings != null);
            SaveCommand = new DelegateCommand(SaveAs, () => SelectedTimeStamp.HasValue && SelectedExchangeSettings != null);

            _provider = new DataExchangeServiceProxy();
        }

        public DelegateCommand EmailGetCommand { get; set; }

        public DelegateCommand EmailSendCommand { get; set; }

        private void SaveAs()
        {
            HtmlPage.Window.Navigate(
                UriBuilder.GetSpecificExchangeHandlerUri(SelectedExchangeSettings.Id, SelectedTimeStamp.Value, xmlOnly: !SelectedExchangeSettings.IsTransform));
        }

        public DelegateCommand SaveCommand { get; set; }

        private async void Run()
        {
            await _provider.RunExchangeTaskAsync(new RunExchangeTaskParameterSet { Id = SelectedExchangeSettings.Id, TimeStamp = ((DateTime)SelectedTimeStamp).ToLocal()});
        }

        public DelegateCommand RunCommand { get; set; }

        private void Clone()
        {
            var viewModel = new AddEditExchangeSettingViewModel(id => Refresh(), SelectedExchangeSettings, isClone: true);
            RadWindow view = new AddEditExchangeSettingView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void Delete()
        {
            //RadWindow.Confirm(
            //    new DialogParameters
            //    {
            //        Content = "Удалить",
            //        Header = "Подтверждение",
            //        OkButtonContent = "Да",
            //        CancelButtonContent = "Отмена",
            //        Closed = (s, e) =>
            //        {
            //            if (e.DialogResult.HasValue && e.DialogResult.Value)
            //            {
            //                _provider.DeleteExchangeSettings(
            //                    SelectedExchangeSettings.Id,
            //                    ex =>
            //                    {
            //                        if (ex == null)
            //                        {
            //                            Refresh();
            //                        }
            //                        return ex == null;
            //                    },
            //                    Behavior);
            //            }
            //        }
            //    });
        }

        private void Edit()
        {
            var viewModel = new AddEditExchangeSettingViewModel(id => Refresh(), SelectedExchangeSettings);
            RadWindow view = new AddEditExchangeSettingView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void Add()
        {
            var viewModel = new AddEditExchangeSettingViewModel(id => Refresh());
            RadWindow view = new AddEditExchangeSettingView { DataContext = viewModel };
            view.ShowDialog();
        }

        public async void Refresh()
        {
            var tasks = await _provider.GetExchangeTaskListAsync(new GetExchangeTaskListParameterSet());
            ExchangeSettingsList.Clear();
            ExchangeSettingsList.AddRange(tasks);
        }

        private readonly DataExchangeServiceProxy _provider;

        private ExchangeTaskDTO _selectedExchangeSettings;

        public ExchangeTaskDTO SelectedExchangeSettings
        {
            get
            {
                return _selectedExchangeSettings;
            }
            set
            {
                if (_selectedExchangeSettings == value)
                {
                    return;
                }
                _selectedExchangeSettings = value;
                OnPropertyChanged(() => SelectedExchangeSettings);
                LastRunTimeStamp = null;
                RaiseCommands();
            }
        }

        private void RaiseCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            RunCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            CloneCommand.RaiseCanExecuteChanged();
        }

        public RangeEnabledObservableCollection<ExchangeTaskDTO> ExchangeSettingsList { get; set; }

        public DelegateCommand AddCommand { get; set; }

        public DelegateCommand DeleteCommand { get; private set; }

        public DelegateCommand EditCommand { get; set; }

        public DelegateCommand ParseCommand { get; set; }

        public DelegateCommand<ExchangeTaskDTO> SaveSpecificExchangesCommand { get; set; }

        public DelegateCommand CloneCommand { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }

        private DateTime? _selectedTimeStamp;

        private DateTime? _lastRunTimeStamp;

        private DateTime? LastRunTimeStamp
        {
            get
            {
                return _lastRunTimeStamp;
            }
            set
            {
                _lastRunTimeStamp = value;
                OnPropertyChanged(() => SelectedTimeStamp);
                RaiseCommands();
            }
        }

        public DateTime? SelectedTimeStamp
        {
            get
            {
                return _selectedTimeStamp;
            }
            set
            {
                _selectedTimeStamp = value;
                OnPropertyChanged(() => SelectedTimeStamp);
                RaiseCommands();
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }
    }


    public class TransportTypeConverter : IValueConverter
    {
        public object Convert(object raw, Type targetType, object parameter, CultureInfo culture)
        {
            var cfg = (ExchangeTaskDTO)raw;
            switch (cfg.TransportTypeId ?? TransportType.Folder)
            {
                case TransportType.Folder:
                    return "smb";
                case TransportType.Ftp:
                    return "ftp";
                case TransportType.Email:
                    return "email";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}