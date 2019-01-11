using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;
using System.Windows.Input;
using Telerik.Windows.Controls;
namespace GazRouter.Modes.GasCosts.MeasuringLoader
{
    /// <summary>
    /// 
    /// 
    /// копирование свойств:
    /// ST12 [UnitFuelCostsViewModel]
    /// ST19 [UnitStopCostsViewModel]
    /// ST38 [HeaterWorkCostsViewModel]
    /// 
    /// </summary>
    public class MeasuringLoaderViewModel : ViewModelBase
    {
#region constructor
        public MeasuringLoaderViewModel(Guid entityId, 
                                        DateTime date, 
                                        bool useMeasuringLoader, 
                                        Action loadingEnded, 
                                        Action loadLastModel)
        {
            _date         = date;
            _entityId     = entityId;
            _loadingEnded = loadingEnded;
            // commands
            LoadLastModelCommand = new DelegateCommand<MenuItem>(obj => { loadLastModel(); }, obj => true);
            SessionCommand = new DelegateCommand<MenuItem>(obj => { LoadMeasurings(obj.Number); }, obj => true);
            // menu
            var itemLoadLast = new MenuItem("Заполнить значения по последнему вводу", LoadLastModelCommand);
            var itemGroupSession = new MenuItem("Сеанс", SessionCommand);
            var itemsSession = GetSessionMenuItems();
            itemGroupSession.Items.AddRange(itemsSession);
            MenuItems = new ObservableCollection<MenuItem> {itemLoadLast};
            if (useMeasuringLoader) MenuItems.Add(itemGroupSession);
        }
#endregion
#region variables  
        private DateTime _date;
        private readonly Guid _entityId;
        private readonly Action _loadingEnded;
        private Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> _valueList;
        public ObservableCollection<MenuItem> MenuItems { get; set; }
#endregion
#region commands
        public DelegateCommand<MenuItem> LoadLastModelCommand { get; set; }
        public DelegateCommand<MenuItem> SessionCommand { get; set; }
#endregion
#region methods
        private IEnumerable<MenuItem> GetSessionMenuItems()
        {
            var list = new List<MenuItem>();
            for (var i = 0; i < 12; i++)
            {
                var number = i * 2;
                var header = $"{number:00}";
                var item = new MenuItem(header, SessionCommand, number);
                list.Add(item);
            }
            return list;
        }
        private async void LoadMeasurings(int hour)
        {
            var startDate = _date.AddHours(hour);// var endDate   = _date.AddHours(hour + 2);

            _valueList = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    EntityIdList = {_entityId},
                    PeriodType   = PeriodType.Twohours,
                    StartDate    = startDate,
                    EndDate      = startDate
                });
            _loadingEnded?.Invoke();
        }
        public double? GetValue(PropertyType propType)
        {
            return
                _valueList?.GetOrDefault(_entityId)?
                    .GetOrDefault(propType)?
                    .OfType<PropertyValueDoubleDTO>()
                    .FirstOrDefault()?.Value;
        }
#endregion
    }
    public class MenuItem
    {
        public MenuItem(string header, ICommand command, int number = 0,  bool isVisible = true)
        {
            Items     = new ObservableCollection<MenuItem>();
            Header    = header;
            Command   = command;
            Number    = number;
            IsVisible = isVisible;
        }

        public string Header { get; set; }
        public bool IsVisible { get; set; }
        public ICommand Command { get; set; }
        public int Number { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }
}
#region trash

//#region trash
//            Session = new DelegateCommand<MenuItem>( , );
//            MenuItemCommand       = new DelegateCommand<MenuItem>(ExecuteMenuItem);
//            LoadMeasuringsCommand = new DelegateCommand<int?>(LoadMeasurings); //             
//#endregion

//public DateTime Day
//{
//    get { return _day; }
//    set
//    {
//        _day = value;
//
//        Hours = new List<int>();
//        for (var i = 0; i < 24; i += 2)
//        {
//            if (_day.AddHours(i) < DateTime.Now) Hours.Add(i);
//            else break;
//        }
//        OnPropertyChanged(() => Hours);
//    }
//}
//public List<int> Hours { get; set; }




// var timestamp = _date;
//var timestamp = _day.AddHours(hour.Value);
//var timestamp2 = _day.AddHours(hour.Value+2);

//      public DelegateCommand Session { get; set; }
//      public DelegateCommand<int?> LoadMeasuringsCommand { get; set; }
//        public void ExecuteMenuItem(MenuItem menuItem)
//        {
//            menuItem.Action.Invoke();
//        }
#endregion