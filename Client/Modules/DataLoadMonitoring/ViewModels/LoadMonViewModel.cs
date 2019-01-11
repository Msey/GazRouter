using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataLoadMonitoring.Module;
using GazRouter.DataLoadMonitoring.Views;
using GazRouter.DataProviders.DataLoadMonitoring;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.Sites;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ScheduleView;

namespace GazRouter.DataLoadMonitoring.ViewModels
{
    public class SeriesType
    {
        public const int evenSeries = 2; //серии с четными метками времени
        public const int oddSeries = 1;//серии с нечетными метками времени
    }

    public class LoadMonViewModel : LockableViewModel
    {
        private const string resourceSummaryName = "SummaryParams";
        private DateTime currentDt;
        private readonly DataLoadMonitoringDataProvider _DataLoadStatisticsProvider = new DataLoadMonitoringDataProvider();
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        private ObservableCollection<Appointment> appointments;
        private ObservableCollection<ResourceType> resourceTypes;
        private ObservableCollection<Slot> specialSlots;

        private List<SiteDataLoadStatistics> statisticsList;
        public List<Site> siteList { get; set; }
        public ICommand VisibleRangeChanged { get; set; }
        public DelegateCommand ClickCommand { get; set; }
        public LoadMonViewModel(int seriesType)
        {
            GetSiteList();
            VisibleRangeChanged = new DelegateCommand(OnVisibleRangeExecuted , OnVisibleRangeCanExecute);
            StartTime = GetStartTime(seriesType);
            EndTime = GetEndTime(seriesType); 
           // this.Appointments = new ObservableCollection<Appointment>();

            specialSlots = new ObservableCollection<Slot> {CreateReadOnlySlot()};

            ClickCommand = new DelegateCommand(ClickCommandExecuted,ClickCommandCanExecute);
        }


        private async void GetSiteList()
        {
            var list = await new ObjectModelServiceProxy().GetSiteListAsync(null);
            SiteListCallback(list, null);
        }


        public void Refresh()
        {
           // _DataLoadStatisticsProvider.GetDataLoadSiteStatistics(new DateTime(2013, 12, 8, 0, 0, 0, DateTimeKind.Local), Callback, Behavior);
        }

        private bool Callback(List<SiteDataLoadStatistics> siteDataLoadStatisticses, Exception exception)
        {
            if (exception != null)
            {
                return false;
            }

            statisticsList = siteDataLoadStatisticses;
            GenerateScheduleScale();
            return true;
        }

        private Slot CreateReadOnlySlot()
        {
            var slot = new Slot { Start = DateTime.MinValue, End = DateTime.MaxValue, IsReadOnly = true };
            slot.Resources.Add(ResourcesTypes);
            return slot;
        }

        public string GetStartTime(int seriesType)
        {
            string sTime = "00:00:00";
            switch (seriesType)
            {
                    //четные серии
                case SeriesType.evenSeries:
                    sTime = "10:00:00";
                    break;
                    //нечетные серии
                case SeriesType.oddSeries:
                    sTime = "11:00:00";
                    break;
            }
            return sTime;
        }
        public string GetEndTime(int seriesType)
        {
            string sTime = "00:00:00";
            switch (seriesType)
            {
                //четные серии
                case SeriesType.evenSeries:
                    sTime = "10:00:00";
                    break;
                //нечетные серии
                case SeriesType.oddSeries:
                    sTime = "13:00:00";
                    break;
            }
            return sTime;
        }
        public ObservableCollection<Appointment> Appointments
        {
            get
            {
                return appointments;
            }
            set
            {
                appointments = value;
                OnPropertyChanged(() => Appointments);
            }
        }
        public ObservableCollection<ResourceType> ResourcesTypes
        {
            get
            {
                return resourceTypes;
            }
            set
            {
                resourceTypes = value;
                OnPropertyChanged(() => ResourcesTypes);
            }
        }
        public ObservableCollection<Slot> SpecialSlots
        {
            get
            {
                return specialSlots;
            }
        }

        //формирование списка ЛПУ
        private bool SiteListCallback(List<SiteDTO> sites, Exception exception)
        {
            if (exception != null)
            {
                return false;
            }

            var lpuType = new ResourceType("LPU");
            var nResources = new ObservableCollection<ResourceType>();
            var resAll = new Resource {DisplayName = "Всего", ResourceName = resourceSummaryName};
            lpuType.Resources.Add(resAll);
            foreach (SiteDTO site in sites)
            {
                lpuType.Resources.Add(new Resource(site.Name)
                    {
                        DisplayName = site.Name,
                        ResourceName = site.Id.ToString()
                    });
            }
            nResources.Add(lpuType);
            ResourcesTypes = nResources;

            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
            _DataLoadStatisticsProvider.GetDataLoadSiteStatistics(dt, Callback, Behavior);
            return true;
        }

        private void GenerateScheduleScale()
        {
            var newAppointments = new ObservableCollection<Appointment>();
            if (statisticsList.Any()  &&  ( ResourcesTypes.Count > 0 )   ) 
            {
                var badCat = new Category("alert", new SolidColorBrush(Colors.Red));

                foreach (var elem in statisticsList)
                {
                    if (elem.DataSeries.PeriodTypeId == PeriodType.Twohours)
                    {
                        var loadEvent = new SiteAppointment
                            {
                                Subject = "Загрузка данных. " + elem.Site.Name,
                                CountParams = elem.ValuesCount.ToString(),
                                Body = elem.DataSeries.KeyDate.ToString("t"),
                                Start = elem.DataSeries.KeyDate
                            };

                        loadEvent.End = loadEvent.Start.AddHours(2);

                        loadEvent.Site = elem.Site;
                        loadEvent.KeyDate = elem.DataSeries;
                        loadEvent.IsSummary = false;
                        //if (elem.ValuesCount < 100)
                        //    LoadEvent.Category = badCat;
                        var res = (from r in ResourcesTypes[0].Resources
                            where r.ResourceName == elem.Site.Id.ToString()
                            select r).First();
                        loadEvent.Resources.Add(res);
                        // LoadEvent.Resources.Add(dataRes);
                        newAppointments.Add(loadEvent);
                    }
                }

                //суммарные значения
            }
            Appointments = newAppointments;
        }

        private bool OnVisibleRangeCanExecute(object param)
        {
            return param != null;
        }
        
        private void OnVisibleRangeExecuted(object param)
        {
            var dt = (IDateSpan)param;
            currentDt = dt.Start;
            _DataLoadStatisticsProvider.GetDataLoadSiteStatistics(new DateTime(dt.Start.Year, dt.Start.Month, dt.Start.Day, 0, 0, 0, DateTimeKind.Local), Callback, Behavior);

        }

        public void ClickCommandExecuted(object parameter)
        {
            if (parameter != null)
            {
                
                var app = parameter as SiteAppointment;
                if(app.IsSummary == false)
                {
                    var vm = new SiteDataViewModel(app.Site,app.KeyDate);
                    var viewData = new ViewDataBySite {DataContext = vm};
                    viewData.ShowDialog();
                }
            }
            
        }
        public bool ClickCommandCanExecute(object parameter)
        {
            return true;
        }
    }

    
}
