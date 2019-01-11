using System;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.Bindings;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.Exchange.ExchangeSettings;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Modes.Exchange
{
    public class TimerSettingsViewModel : MainViewModelBase
    {
        public TimerSettingsViewModel()
        {
            StartCommand = new DelegateCommand(Start, () => SelectedTimer != null);
            StopCommand = new DelegateCommand(Stop, () => SelectedTimer != null);
            EditCommand = new DelegateCommand(Edit, () => SelectedTimer != null);
            TimerSettings = new RangeEnabledObservableCollection<TimerSettingsWrapper>();
        }
        private bool _isSelected;
        private TimerSettingsWrapper _selectedTimer;

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
        public async void Refresh()
        {
            var timers = await new DataExchangeServiceProxy().GetTimersAsync();
            TimerSettings.Clear();
            TimerSettings.AddRange(timers.Select(t => new TimerSettingsWrapper(t)));

        }

        

        public RangeEnabledObservableCollection<TimerSettingsWrapper> TimerSettings { get; set; }

        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get;  set; }
        public DelegateCommand EditCommand { get; set; }

        public TimerSettingsWrapper SelectedTimer
        {
            get { return _selectedTimer; }
            set
            {
                if (Equals(value, _selectedTimer)) return;
                _selectedTimer = value;
                OnPropertyChanged(() => SelectedTimer);
                RaiseCommands();
            }
        }

        private void RaiseCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// запуск выбранного таймера
        /// </summary>
        async void Start()
        {
            SelectedTimer.TimerStatus = TimerStatus.InProgress;
            await new DataExchangeServiceProxy().EditExportTimerStatusAsync(SelectedTimer.Dto);
        }


        /// <summary>
        /// остановка выбранного таймера
        /// </summary>
        async void Stop()
        {
            SelectedTimer.TimerStatus = TimerStatus.Stopped;
            await new DataExchangeServiceProxy().EditExportTimerStatusAsync(SelectedTimer.Dto);
        }
        /// <summary>
        /// редактирование настроек таймера
        /// </summary>
        void Edit()
        {
            var viewModel = new AddEditTimerSettingViewModel(id => Refresh(), SelectedTimer.Dto);
            RadWindow view = new AddEditTimerSettingView { DataContext = viewModel };
            view.ShowDialog();
        }

    }

    public class TimerSettingsWrapper : PropertyChangedBase
    {
        public readonly TimerSettingsDTO Dto;

        public TimerSettingsWrapper(TimerSettingsDTO dto)
        {
            Dto = dto;
        }

        public int TimerId
        {
            get { return Dto.TimerId; }
            set { Dto.TimerId = value; }
        }

        public string TimerName
        {
            get { return Dto.TimerName; }
            set { Dto.TimerName = value; }
        }

        public TimeSpan Frequency
        {
            get { return Dto.Frequency; }
            set { Dto.Frequency = value; }
        }

        public TimerStatus TimerStatus
        {
            get { return Dto.TimerStatus; }
            set
            {
                Dto.TimerStatus = value;
                OnPropertyChanged(() => TimerStatus);
            }
        }

        public TimerStartType StartType
        {
            get { return Dto.StartType; }
            set { Dto.StartType = value; }
        }
    }
}
