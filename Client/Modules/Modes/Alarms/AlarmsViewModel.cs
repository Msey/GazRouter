using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Alarms;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO;
using GazRouter.DTO.Alarms;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Trends;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;


namespace GazRouter.Modes.Alarms
{
    public class AlarmsViewModel : AsyncViewModelBase
    {
        private AlarmItem _selectedAlarm;
        
        public AlarmsViewModel()
        {
            AlarmList = new List<AlarmItem>();
         
            RefreshCommand = new DelegateCommand(Refresh);
            AddCommand = new DelegateCommand(
                () =>
                {
                    var vm = new AddEditAlarmViewModel(x => Refresh());
                    var v = new AddEditAlarmView {DataContext = vm};
                    v.ShowDialog();
                });
            EditCommand = new DelegateCommand(
                () =>
                {
                    var vm = new AddEditAlarmViewModel(x => Refresh(), SelectedAlarm.Dto);
                    var v = new AddEditAlarmView {DataContext = vm};
                    v.ShowDialog();
                });
            DeleteCommand = new DelegateCommand(async() =>
            {
                Behavior.TryLock();
                await new AlarmsServiceProxy().DeleteAlarmAsync(SelectedAlarm.Id);
                Behavior.TryUnlock();
                Refresh();
            }, () => SelectedAlarm != null);


            TrackInfoUpdatedCommand = new Telerik.Windows.Controls.DelegateCommand(OnTrackInfoUpdated);
            
            Refresh();
        }

        private int _filter;
        /// <summary>
        /// Выбор отображения: 0 - все тревоги своего подразделения, 1 - только свои
        /// </summary>
        public int Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                Refresh();
            }
        }


        public List<AlarmItem> AlarmList { get; set; }
        
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public Telerik.Windows.Controls.DelegateCommand TrackInfoUpdatedCommand { get; private set; }

        
        private async void Refresh()
        {
            try
            {
                Behavior.TryLock();

                var result = await new AlarmsServiceProxy().GetAlarmListAsync(
                    new GetAlarmListParameterSet
                    {
                        SiteId = UserProfile.Current.Site.Id,
                        UserId = _filter == 0 ? null : (int?)UserProfile.Current.Id
                    });

                AlarmList = new List<AlarmItem>(result.Select(a => new AlarmItem(a)));
                OnPropertyChanged(() => AlarmList);
                SelectedAlarm = AlarmList.Count > 0 ? AlarmList.First() : null;
                
            }
            finally
            {
                Behavior.TryUnlock();    
            }
            
        }

        /// <summary>
        /// Выбранная уставка
        /// </summary>
        public AlarmItem SelectedAlarm
        {
            get { return _selectedAlarm; }
            set
            {
                _selectedAlarm = value;
                OnPropertyChanged(() => SelectedAlarm);

                GetTrendData();
            }
        }
        

        public List<BasePropertyValueDTO> TrendData { get; set; }
        
        private async void GetTrendData()
        {
            TrendData = new List<BasePropertyValueDTO>();
            var alrm = _selectedAlarm;
            if (alrm != null)
            {
                Behavior.TryLock();

                var startDate = alrm.CreationDate;
                var endDate = DateTime.Now < alrm.ExpirationDate ? DateTime.Now : alrm.ExpirationDate;

                if (alrm.Dto.PeriodTypeId == PeriodType.Twohours && (endDate - startDate).TotalDays > 2)
                    startDate = endDate.AddDays(-2);

                if (alrm.Dto.PeriodTypeId == PeriodType.Day && (endDate - startDate).TotalDays > 10)
                    startDate = endDate.AddDays(-10);
                

                var result =
                    await new SeriesDataServiceProxy().GetTrendAsync(
                        new GetTrendParameterSet
                        {
                            EntityId = SelectedAlarm.Dto.EntityId,
                            PropertyTypeId = SelectedAlarm.Dto.PropertyTypeId,
                            PeriodTypeId = SelectedAlarm.Dto.PeriodTypeId,
                            StartDate = startDate,
                            EndDate = endDate
                        });
                TrendData = result.Data;


                TrendData.OfType<PropertyValueDoubleDTO>()
                    .ForEach(v => v.Value = UserProfile.ToUserUnits(v.Value, alrm.PropertyType.PropertyType));

                

                TrendMin = 0;
                TrendMax = 0;
                SettingLine = new List<PropertyValueDoubleDTO>();

                if (result.Data != null && result.Data.Count > 0)
                {

                    //формирование линии уставки
                    SettingLine.Add(new PropertyValueDoubleDTO
                    {
                        Date = TrendData.Min(v => v.Date),
                        Value = alrm.Setting
                    });
                    SettingLine.Add(new PropertyValueDoubleDTO
                    {
                        Date = TrendData.Max(v => v.Date),
                        Value = alrm.Setting
                    });
                    

                    //расчет максимально и минимально значения для шкалы трендов
                    var min = result.Data.OfType<PropertyValueDoubleDTO>().Min(v => v.Value);
                    var max = result.Data.OfType<PropertyValueDoubleDTO>().Max(v => v.Value);

                    min = alrm.Setting < min ? alrm.Setting : min;
                    max = alrm.Setting > max ? alrm.Setting : max;

                    var delta = max - min;
                    var digit = 0.001;
                    while (Math.Round(delta/digit) > 0) digit *= 10;
                    digit /= 10;

                    min = Math.Round(min/digit)*digit;
                    max = Math.Round(max/digit)*digit;

                    delta = max - min;

                    min -= 2*delta;
                    max += 2*delta;

                    if (SelectedAlarm.PropertyType.PhysicalType.PhysicalType == PhysicalType.Pressure && min < 0)
                        min = 0;
                    if (SelectedAlarm.PropertyType.PhysicalType.PhysicalType == PhysicalType.Volume && min < 0)
                        min = 0;

                    TrendMin = min;
                    TrendMax = max;

                    //Расчет интервала для временной шкалы
                    switch (alrm.Dto.PeriodTypeId)
                    {
                        case PeriodType.Twohours:
                            TrendTimeStepUnit = TimeInterval.Day;
                            TrendTimeStep = 1;
                            break;

                        case PeriodType.Day:
                            TrendTimeStepUnit = TimeInterval.Day;
                            TrendTimeStep = 1;
                            break;

                        default:
                            TrendTimeStepUnit = TimeInterval.Month;
                            TrendTimeStep = 1;
                            break;
                    }

                    Behavior.TryUnlock();


                }
            }

            OnPropertyChanged(() => TrendData);
            OnPropertyChanged(() => TrendMin);
            OnPropertyChanged(() => TrendMax);
            OnPropertyChanged(() => TrendStep);
            OnPropertyChanged(() => SettingLine);
            OnPropertyChanged(() => TrendTimeStepUnit);
            OnPropertyChanged(() => TrendTimeStep);



            // Формирование списка событий по уставке
            


        }

        public double TrendMin { get; set; }
        public double TrendMax { get; set; }
        public double TrendStep { get; set; }
        public TimeInterval TrendTimeStepUnit { get; set; }
        public int TrendTimeStep { get; set; }

        /// <summary>
        /// Линия уставки для построения тренда
        /// </summary>
        public List<PropertyValueDoubleDTO> SettingLine { get; set; }



        public string TrackInfoDate { get; set; }
        public string TrackInfoValue { get; set; }

        private void OnTrackInfoUpdated(object obj)
        {
            if (obj == null) return;
            var tbi = (TrackBallInfoEventArgs)obj;
            var closesDataPt = tbi.Context.ClosestDataPoint.DataPoint as CategoricalDataPoint;
            if (closesDataPt == null) return;

            TrackInfoDate = ((DateTime) closesDataPt.Category).ToString("s");
            TrackInfoValue = closesDataPt.Value.Value.ToString("0,0.000");

            OnPropertyChanged(() => TrackInfoDate);
            OnPropertyChanged(() => TrackInfoValue);
        }

    }



    public class AlarmEvent
    {
        public DateTime Timestamp { get; set; }
        public double PropertyValue { get; set; }
        public bool Status { get; set; }

    }
}
