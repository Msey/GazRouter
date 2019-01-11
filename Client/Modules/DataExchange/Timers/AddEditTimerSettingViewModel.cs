using System;
using System.Threading.Tasks;
using System.Windows;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.Exchange.ExchangeSettings;

namespace DataExchange.Timers
{
    public class AddEditTimerSettingViewModel : AddEditViewModelBase<TimerSettingsDTO, int>
    {
        public AddEditTimerSettingViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            Init();
        }


        public AddEditTimerSettingViewModel(Action<int> actionBeforeClosing, TimerSettingsDTO es)
            : base(actionBeforeClosing, es)
        {
            Init();
	        TimerName = es.TimerName;
			TimerId = es.TimerId;
            Id = es.TimerId;
            Interval = es.Interval;
        }


        private void Init()
        {
        }

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }

        public string TimerName
        {
            get { return _timerName; }
            set
            {
                _timerName = value; 
                OnPropertyChanged(() => TimerName);
                OnSaveCommandCanExecute();
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return new DataExchangeServiceProxy().EditTimerAsync(
                    new TimerSettingsDTO
                {
                    TimerId = Model.TimerId,
                    TimerName = TimerName,
                    TimerStatus = Model.TimerStatus,
                    Interval = Interval
                });
            }
        }

        private int _timerId;
        private string _timerName;
        private TimeSpan _interval;
        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof (object), typeof (AddEditTimerSettingViewModel), new PropertyMetadata(default(object)));

        public int TimerId
        {
            get { return _timerId; }
            set
            {
                if (_timerId == value) return;
                _timerId = value;
                OnPropertyChanged(() => TimerId);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                if (_interval == value) return;
                _interval = value;
                OnPropertyChanged(() => TimerId);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        protected override string CaptionEntityTypeName
        {
            get { return " таймера"; }
        }

    }
}