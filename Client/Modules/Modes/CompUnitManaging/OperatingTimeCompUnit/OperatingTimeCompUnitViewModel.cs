using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.SeriesData;
using GazRouter.DTO.Stoppages.GetListStoppage;
using Microsoft.Practices.ObjectBuilder2;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.DataBars;
using Telerik.Windows.Controls.Timeline;
using Utils.Extensions;

namespace GazRouter.Modes.CompressorUnitManaging.OperatingTimeCompUnit
{
    public class OperatingTimeCompUnitViewModel : LockableViewModel
    {
        private bool _initialized;

        private async void Load()
        {
            var daysInMonth = DateTime.DaysInMonth(_selectedDate.Year, _selectedDate.Month);
            var beginDate = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
            var endDate = new DateTime(SelectedDate.Year, SelectedDate.Month, daysInMonth, 23, 59, 59);
            BeginDate = beginDate;
            EndDate = endDate;
            VisibleBeginDate = beginDate;
            VisibleEndDate = endDate;

            try
            {
                Behavior.TryLock();
                var result =
                    await new SeriesDataServiceProxy().GetOperatingTimeCompUnitListAsync(
                        new DateIntervalParameterSet
                        {
                            BeginDate =
                                Settings.DispatherDayStartHour % 2 == 0 ? beginDate.ToLocal() : beginDate.AddHours(-1).ToLocal(),
                            EndDate = endDate.ToLocal()
                        });

                GetListCallback(result);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }

        private async void GetListCallback(CompUnitsOperatingTimeDto data)
        {
            //_stopages = 
            //await new ManualInputServiceProxy().GetStoppageListAsync(
            //    new GetStoppagesParameterSet
            //    {
            //        StartDate = Settings.DispatherDayStartHour % 2 == 0 ? BeginDate.ToLocal() : BeginDate.AddHours(-1).ToLocal(),
            //        EndDate = EndDate.ToLocal()
            //    });

            _stopages = new List<StoppageDTO>();
            
				   
			var tmp =
                data.OperatingTime.SelectMany(cUnit => cUnit.Value)
                    .SelectMany(cUnitState => cUnitState.Value)
                    .Where(k => k.BeginDate < BeginDate);

			tmp.ForEach(di => di.BeginDate = BeginDate);

            _tmpCallBack = data;
            var tmpCol = new Collection<TreeListItem>();
            foreach (var cSite in data.EntityTree)
            {
                var site = new TreeListItem { Name = cSite.Key.Name, EntityType = EntityType.Site };//CompStation
                var compStations = new List<TreeListItem>();
                foreach (var station in cSite.Value)
                {
                    var compstation = new TreeListItem
                    {
                        Name = station.Key.Name,
                        EntityType = EntityType.CompStation,
                        CompStationName = station.Key.Name,
                        SiteName = cSite.Key.Name

                    };
                    var compShops = new List<TreeListItem>();
                    foreach (var cShop in station.Value)
                    {
                        var compShop = new TreeListItem
                        {
                            Name = cShop.Key.Name,
                            EntityType = EntityType.CompShop,
                            CompStationName = station.Key.Name,
                            CompShopName = cShop.Key.Name,
                            SiteName = cSite.Key.Name
                        };
                        var compUnits = new List<TreeListItem>();
                        foreach (var cUnit in cShop.Value)
                        {
                            var res = new Dictionary<CompUnitState, double>
												{
													{CompUnitState.Work, 0},
													{CompUnitState.Reserve, 0},
													{CompUnitState.Repair, 0}
												};
                            Dictionary<CompUnitState, List<DateIntervalDTO>> compUnitStateDictionary;

                            if (data.OperatingTime.TryGetValue(cUnit.Id, out compUnitStateDictionary))
                            {
                                foreach (var item in compUnitStateDictionary)
                                {
                                    var sum = item.Value.Sum(dateValue => (dateValue.EndDate - dateValue.BeginDate).TotalHours);
                                    switch (item.Key)
                                    {
                                        case CompUnitState.Work:
                                            res[CompUnitState.Work] = sum;
                                            break;

                                        case CompUnitState.Reserve:
                                            res[CompUnitState.Reserve] = sum;
                                            break;

                                        case CompUnitState.Repair:
                                            res[CompUnitState.Repair] = sum;
                                            break;
                                    }
                                }
                            }

                            var compUnit = new TreeListItem
                            {
                                Name = cUnit.Name,
                                SuperchargerTypeName = ClientCache.DictionaryRepository.SuperchargerTypes.First(st => st.Id == cUnit.SuperchargerTypeId).Name,
                                CompUnitTypeName = ClientCache.DictionaryRepository.CompUnitTypes.First(ut => ut.Id == cUnit.CompUnitTypeId).Name,
                                Work = res[CompUnitState.Work],
                                Reserve = res[CompUnitState.Reserve],
                                Repair = res[CompUnitState.Repair],
                                EntityType = EntityType.CompUnit,
                                CompStationName = station.Key.Name,
                                CompShopName = cShop.Key.Name,
                                SiteName = cSite.Key.Name
                            };

                            compUnits.Add(compUnit);
                            compShop.Children = new List<TreeListItem>(compUnits.OrderBy(item => item.Name));
                        }
                        compShops.Add(compShop);
                        compstation.Children = new List<TreeListItem>(compShops.OrderBy(item => item.Name));
                    }
                    compStations.Add(compstation);
                }
                site.Children = new List<TreeListItem>(compStations.OrderBy(i => i.Name));
                tmpCol.Add(site);
            }
            Items = new List<TreeListItem>(tmpCol.OrderBy(item => item.Name));
            SelectedItem = Items.FirstOrDefault();
			
				   
        }

        #region Properties

		private List<BaseTimeLineSource> _timeLineSource;
		public List<BaseTimeLineSource> TimeLineSource
        {
            get { return _timeLineSource; }
            set
            {
                _timeLineSource = value;
                OnPropertyChanged(() => TimeLineSource);
            }
        }

        private List<TreeListItem> _items;
        public List<TreeListItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(() => Items);
            }
        }

        private TreeListItem _selectedItem;
        public TreeListItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if(value == null)
                    return;
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
                LoadTimeLine();
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if(_selectedDate == value)
                    return;
                
                _selectedDate = value;
                OnPropertyChanged(() => SelectedDate);
                Load();
            }
        }

        private DateTime _beginDate;
        public DateTime BeginDate
        {
            get { return _beginDate; }
            set
            {
                _beginDate = value;
                OnPropertyChanged(() => BeginDate);
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(() => EndDate);
            }
        }

        private DateTime _visibleBeginDate;
        public DateTime VisibleBeginDate
        {
            get { return _visibleBeginDate; }
            set
            {
                _visibleBeginDate = value;
                OnPropertyChanged(() => VisibleBeginDate);
            }
        }

        private DateTime _visibleEndDate;
        public DateTime VisibleEndDate
        {
            get { return _visibleEndDate; }
            set
            {
                _visibleEndDate = value;
                OnPropertyChanged(() => VisibleEndDate);
            }
        }

        public CultureInfo CultureWithFormattedPeriod
        {
            get
            {
                var tempCultureInfo = new CultureInfo("ru-RU") { DateTimeFormat = { ShortDatePattern = "MMMM yyyy" } };
                return tempCultureInfo;
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (!_initialized)
                {
                    _initialized = true;
                    SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
            }
        }

        #endregion

        #region LoadTimeLine

        private CompUnitsOperatingTimeDto _tmpCallBack;

        private bool _isActive;

	    private void LoadTimeLine()
	    {
		    var @query = (from site in _tmpCallBack.EntityTree
						  from cStation in site.Value
		                  from cShop in cStation.Value
		                  from cUnit in cShop.Value
		                  select new TimeLineSource
			                         {
										 SiteName = site.Key.Name,
				                         CompStationName = cStation.Key.Name,
				                         CompShopName = cShop.Key.Name,
				                         CompUnitName = cUnit.Name,
				                         StartPath = BeginDate,
				                         DurationPath = EndDate - BeginDate,
				                         ValuesList = DateCalculationBarValues(cUnit.Id),
				                         Brushes = DateCalculationToBrushList(cUnit.Id),
				                         Id = cUnit.Id
			                         });

		    switch (SelectedItem.EntityType)
		    {
				case EntityType.Site:
					@query = @query.Where(key => key.SiteName == SelectedItem.Name);

					break;
			    case EntityType.CompStation:
					@query = @query.Where(key => key.SiteName == SelectedItem.SiteName && key.CompStationName == SelectedItem.Name);

				    break;

			    case EntityType.CompShop:
				    @query =
					    @query.Where(
						    key =>
							key.SiteName == SelectedItem.SiteName && key.CompStationName == SelectedItem.CompStationName && key.CompShopName == SelectedItem.Name);
				    break;

			    case EntityType.CompUnit:
				    @query =
					    @query.Where(
						    key =>
							key.SiteName == SelectedItem.SiteName && key.CompStationName == SelectedItem.CompStationName && key.CompShopName == SelectedItem.CompShopName &&
						    key.CompUnitName == SelectedItem.Name);
				    break;
		    }


		    var temp = new List<BaseTimeLineSource>(
			    @query.OrderBy(cStation => cStation.CompStationName).
			           ThenBy(cShop => cShop.CompShopName).ThenBy(cu => cu.CompUnitName));
		    TimeLineHeight = Math.Max(290, temp.Count*35);
		    temp.AddRange(
			    _stopages.Where(p => temp.Select(r => r.Id).Contains(p.CompressorUnitId)).Select(
				    t =>
				    new TimeLineStopSource
					    {
						    Parent = t.CompressorStationId,
						    Id = t.CompressorUnitId,
						    StartPath = t.FailureDate,
						    DurationPath = new TimeSpan(0, 0, 0),
						    CompUnitName = t.StoppageCauseDescription
					    }));
		    TimeLineSource = temp;
		    OnPropertyChanged(() => TimeLineSource);
	    }

	    private List<StoppageDTO> _stopages=new List<StoppageDTO>(); 

        private List<RadDataBarItemClass> DateCalculationBarValues(Guid id)
        {
            var res = new List<RadDataBarItemClass>();
            var lastDate = BeginDate;
            double allTicks = (EndDate - BeginDate).Ticks;
            double result;
            Dictionary<CompUnitState, List<DateIntervalDTO>> compUnitStateDictionary;
            if (_tmpCallBack.OperatingTime.TryGetValue(id, out compUnitStateDictionary) && compUnitStateDictionary.Any())
            {
                var firstDate = compUnitStateDictionary.SelectMany(kvp => kvp.Value, (state, dInt) => new { DInt = dInt }).
                    Min(pair => pair.DInt.BeginDate);

                if (firstDate > BeginDate)
                {
                    double dtoTicks = (firstDate - BeginDate).Ticks;
                    result = (dtoTicks * 100) / allTicks;
                    res.Add(new RadDataBarItemClass { BarValue = result, ToolTipItem = "Нет данных" });
                }

                foreach (var dto in compUnitStateDictionary.SelectMany(item => item.Value).OrderBy(item => item.BeginDate))
                {
                    double dtoTicks = (dto.EndDate - dto.BeginDate).Ticks;
                    result = (dtoTicks * 100) / allTicks;
                    res.Add(new RadDataBarItemClass { BarValue = result, ToolTipItem = string.Format("{0} - {1}", dto.BeginDate, dto.EndDate) });
                }

                lastDate = compUnitStateDictionary.SelectMany(kvp => kvp.Value, (state, dInt) => new { DInt = dInt }).
                    Max(pair => pair.DInt.EndDate);
            }

            if (lastDate < EndDate)
            {
                double ticks = (EndDate - lastDate).Ticks;
                result = (ticks * 100) / allTicks;
                res.Add(new RadDataBarItemClass { BarValue = result, ToolTipItem = "Нет данных" });
            }

            return res;
        }

        private BrushCollection DateCalculationToBrushList(Guid id)
        {
            var res = new BrushCollection();
            var lastDate = BeginDate;
            Dictionary<CompUnitState, List<DateIntervalDTO>> compUnitStateDictionary;
            if (_tmpCallBack.OperatingTime.TryGetValue(id, out compUnitStateDictionary) && compUnitStateDictionary.Any())
            {
                var firstDate = compUnitStateDictionary.SelectMany(kvp => kvp.Value, (state, dInt) => new { DInt = dInt }).
                    Min(pair => pair.DInt.BeginDate);

                if (firstDate > BeginDate)
                {
                    res.Add(new SolidColorBrush(Colors.Gray));
                }

                var states = compUnitStateDictionary.SelectMany(kvp => kvp.Value, (state, dInt) => new { State = state, DInt = dInt }).OrderBy(pair => pair.DInt.BeginDate).
                    Select(pair => pair.State);

                foreach (var item in states)
                {
                    switch (item.Key)
                    {
                        case CompUnitState.Work:
                            res.Add(new SolidColorBrush(Colors.Green));
                            break;

                        case CompUnitState.Reserve:
                            res.Add(new SolidColorBrush(Colors.Orange));
                            break;

                        case CompUnitState.Repair:
                            res.Add(new SolidColorBrush(Colors.Red));
                            break;
                    }
                }

                lastDate = compUnitStateDictionary.SelectMany(kvp => kvp.Value, (state, dInt) => new { DInt = dInt }).
                    Max(pair => pair.DInt.EndDate);
            }
            
            if (lastDate < EndDate)
            {
                res.Add(new SolidColorBrush(Colors.Gray));
            }

            return res;
        }

        #endregion

		private double _timeLiteHeight;

		public double TimeLineHeight
		{
			get { return _timeLiteHeight; }
			set { _timeLiteHeight = value; OnPropertyChanged(()=>TimeLineHeight); }
		}
    }

    #region Classes

    public class TreeListItem
    {
        public string Name { get; set; }
        [Display(AutoGenerateField = false)]
        public List<TreeListItem> Children { get; set; }
        public double? Work { get; set; }
        public double? Reserve { get; set; }
        public double? Repair { get; set; }

        public string SuperchargerTypeName { get; set; }

        public string CompUnitTypeName { get; set; }

        public EntityType EntityType;
        public string CompStationName;
        public string CompShopName;
		public string SiteName;
    }

	public class TimeLineSource : BaseTimeLineSource
    {
        public string CompStationName { get; set; }
        public string CompShopName { get; set; }
		public string SiteName { get; set; }
        
        public List<RadDataBarItemClass> ValuesList { get; set; }
        public BrushCollection Brushes { get; set; }
    }

	public class TimeLineStopSource : BaseTimeLineSource
	{
		public Guid Parent { get; set; }
	}

	public abstract class BaseTimeLineSource
	{
		public DateTime StartPath { get; set; }
		public string CompUnitName { get; set; }
		public Guid Id { get; set; }
		public TimeSpan DurationPath { get; set; }

	}

    public class RadDataBarItemClass
    {
        public double BarValue { get; set; }
        public string ToolTipItem { get; set; }
    }

	public class OperatingTemplateSelector :DataTemplateSelector
	{
		public DataTemplate Template { get; set; }
		public DataTemplate TemplateStop { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return ((TimelineDataItem) item).DataItem is TimeLineSource ? Template : TemplateStop;
		}
	}

	public class RowIndexGenerator:IItemRowIndexGenerator
	{
		public void GenerateRowIndexes(List<TimelineRowItem> dataItems)
		{
			int row = 0;
			var timelinedata = dataItems.Where(t => t.DataItem.GetType() == typeof(TimeLineSource)).ToList();
			foreach (var timelineRowItem in timelinedata)
			{
				timelineRowItem.RowIndex = row;
				var item = timelineRowItem;
				var stopsource =
					dataItems.Where(
						t =>
						t.DataItem.GetType() == typeof (TimeLineStopSource) &&
						((BaseTimeLineSource)t.DataItem).Id == ((BaseTimeLineSource)item.DataItem).Id);
				foreach (var rowItem in stopsource)
				{
					rowItem.RowIndex = row;
				}
				row++;
			}
		}
	}
    #endregion
}