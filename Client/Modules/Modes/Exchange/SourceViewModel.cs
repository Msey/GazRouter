using System;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.Bindings;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.DataExchange.DataSource;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Modes.Exchange
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class SourceViewModel : MainViewModelBase
    {
        public SourceViewModel(Action sourceListChangedAction)
        {
            _sourceListChangedAction = sourceListChangedAction;
            SourceList = new RangeEnabledObservableCollection<DataSourceDTO>();
            AddCommand = new DelegateCommand(Add);
            EditCommand = new DelegateCommand(Edit, () => SelectedSource != null);
            DeleteCommand = new DelegateCommand(Delete, () => SelectedSource != null);
        }


        private void Delete()
        {
            RadWindow.Confirm(
                new DialogParameters
                {
                    Content = "Удалить",
                    Header = "Подтверждение",
                    OkButtonContent = "Да",
                    CancelButtonContent = "Отмена",
                    Closed = async (s, e) =>
                             {
                                 if (e.DialogResult.HasValue && e.DialogResult.Value)
                                 {
                                     await new DataExchangeServiceProxy().DeleteDataSourceAsync(SelectedSource.Id);
                                    Refresh();
                                 }
                             }
                });
        }

        private void Edit()
        {
            var viewModel = new AddEditSourceViewModel(id => Refresh(), SelectedSource);
            RadWindow view = new AddEditSourceView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void Add()
        {
            var viewModel = new AddEditSourceViewModel(id => Refresh());
            RadWindow view = new AddEditSourceView{ DataContext = viewModel };
            view.ShowDialog();
        }

        public async void Refresh()
        {
            var sources = await new DataExchangeServiceProxy().GetDataSourceListAsync(new GetDataSourceListParameterSet());
            SourceList.Clear();
            SourceList.AddRange(sources);

            _sourceListChangedAction();
        }

        private DataSourceDTO _selectedSource;

        public DataSourceDTO SelectedSource
        {
            get
            {
                return _selectedSource;
            }
            set
            {
                if (_selectedSource == value)
                {
                    return;
                }
                _selectedSource = value;
                OnPropertyChanged(() => SelectedSource);
                RaiseCommands();
            }
        }

        private void RaiseCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        public RangeEnabledObservableCollection<DataSourceDTO> SourceList { get; set; }

        public DelegateCommand AddCommand { get; set; }

        public DelegateCommand DeleteCommand { get; private set; }

        public DelegateCommand EditCommand { get; set; }

        private bool _isSelected;
        private readonly Action _sourceListChangedAction;

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


        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }
    }
}