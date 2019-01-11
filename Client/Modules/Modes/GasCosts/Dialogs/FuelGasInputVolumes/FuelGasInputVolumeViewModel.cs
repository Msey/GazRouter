using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.InputStory;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.Modes.GasCosts.Dialogs.FuelGasInputVolumes
{
    public class FuelGasInputVolumeViewModel : DialogViewModel
    {
        public readonly int Coef;
        public FuelGasInputVolumeViewModel(Action closeCallback, DateTime _selectedDate, Guid _selectedSiteId, int Coef=1)
            :base(closeCallback)
        {
            this.Coef = Coef;
            SelectedDate = _selectedDate;
            SelectedSiteId = _selectedSiteId;
            //SelectedDate = SeriesHelper.GetPastDispDay();
            //Items = new List<FuelGasInputItem>();

            LoadTree();

            PropertyChanged += FuelGasInputVolumeViewModel_PropertyChanged;

            SaveCommand = new DelegateCommand(Save);

            CancelCommand = new DelegateCommand(() => { DialogResult = false; });
        }

        public string VolumeType
        {
            get
            {
                return "Расход\nтопливного газа,"+ ((Coef>1)? "\nм3" : "\nтыс.м3");
            }
        }

        public string FormatType
        {
            get
            {
                return ((Coef > 1) ? "F0" : "n3");
            }
        }
            
        public DelegateCommand SaveCommand { get; set; }
        public new DelegateCommand CancelCommand { get; set; }

        private void FuelGasInputVolumeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Items))
            {

            }
        }


        #region property
        public TreeDataDTO TreeData { get; set; }
        public Guid SelectedSiteId { get; set; }
        #endregion

        private DateTime gasCostDate;
        private DateTime startDate;
        private DateTime endDate;
        private DateTime _selectedDate;
        public DateTime SelectedDate {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                gasCostDate = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day);
                startDate = SeriesHelper.GetDispDayStart(_selectedDate);// new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day, 10, 0, 0);
                endDate = SeriesHelper.GetDispDayEnd(_selectedDate);// new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day, 10, 0, 0).Add(new TimeSpan(24, 0, 0));
                //startDate = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day, 10, 0, 0);
                //endDate = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day, 10, 0, 0).Add(new TimeSpan(24, 0, 0));
            }
        }

        public string InputDate => SelectedDate.ToLongDateString();

        #region methods

       

        public async void Save()
        {

            Lock();

            var serie = await new SeriesDataServiceProxy().AddSerieAsync(
                new AddSeriesParameterSet
                {
                    Year = SelectedDate.Year,
                    Month = SelectedDate.Month,
                    Day = SelectedDate.Day,
                    PeriodTypeId = PeriodType.Day
                });

            if (serie != null)
            {

                var items = Items.SelectMany(e => e.Children);
                foreach (var kc in items)
                {
                    foreach (var gpa in kc.Children)
                    {
                        var proxy = new GasCostsServiceProxy();
                        if (gpa.GasVolume == 0)
                            for (var i = 0; i < gpa.gasCostDTO.Count; i++)
                                await new GasCostsServiceProxy().DeleteGasCostAsync(gpa.gasCostDTO[i].Id);
                        else
                        {

                            if (gpa.gasCostDTO.Count == 0)
                            {
                                await proxy.AddGasCostAsync(new AddGasCostParameterSet()
                                {
                                    SeriesId= serie.Id,
                                    //Date = S SelectedDate,
                                    CostType = CostType.CT12,
                                    EntityId = gpa.Entity.Id,
                                    SiteId = SelectedSiteId,
                                    Target = DTO.Dictionaries.Targets.Target.Fact,
                                    MeasuredVolume = gpa.GasVolume / Coef
                                }
                                    );
                            }
                            else
                            {
                                await new GasCostsServiceProxy().EditGasCostAsync(new EditGasCostParameterSet()
                                {
                                    CostId = gpa.gasCostDTO[0].Id,
                                    SeriesId = serie.Id,
                                    //Date = SelectedDate,
                                    CostType = CostType.CT12,
                                    EntityId = gpa.Entity.Id,
                                    SiteId = SelectedSiteId,
                                    Target = DTO.Dictionaries.Targets.Target.Fact,
                                    MeasuredVolume = gpa.GasVolume / Coef
                                });
                                for (var i = 1; i < gpa.gasCostDTO.Count; i++) await new GasCostsServiceProxy().DeleteGasCostAsync(gpa.gasCostDTO[i].Id);
                            }
                        }
                    }
                }

            }
            Unlock();

            DialogResult = true;
        }

        public List<FuelGasInputItem> Items { get; set; }
        public async Task LoadTree(TreeDataDTO data)
        {
            TreeData = data;
            Items = new List<FuelGasInputItem>();//            var CompShops = data.CompShops.Cast<EntityDTO>().ToList();
            foreach(var compStationDTO in TreeData.CompStations)
            {
                var compStationItem = new FuelGasInputItem(compStationDTO, Coef) { IsEditorVisible = false };
                Items.Add(compStationItem);
                foreach (var compShopDTO in TreeData.CompShops.Where(c => c.ParentId == compStationDTO.Id)) // КЦ
                {
                    var kc = new FuelGasInputItem(compShopDTO, Coef);// Items.Add(kc);
                    compStationItem.AddChild(kc);
                    kc._gasVolume = 0;
                    kc._gasVolume = 0;
                    foreach (var compUnitDTO in TreeData.CompUnits.Where(c => c.ParentId == compShopDTO.Id)) // ГПА
                    {
                        //qty
                        var child = new FuelGasInputItem(compUnitDTO, Coef);
                        var _currentCosts =
                            GasCosts.Where(dto => dto.Entity.Id == child.Entity.Id && dto.CostType == CostType.CT12)
                                .ToList();
                        if (_currentCosts != null) child.gasCostDTO.AddRange(_currentCosts);
                        child.GasVolume = child.gasCostDTO != null ? _currentCosts.Sum(x => x.Volume) : 0;
                        kc._gasVolume += child._gasVolume;
                        //hour
                        var states = UnitStates.Where(dto => dto.CompUnitId == child.Entity.Id).ToList();
                        double wh = 0;
                        for (int i = 0; i < states.Count; i++)
                        {
                            if (states[i].State == DTO.Dictionaries.StatesModel.CompUnitState.Work)
                            {
                                //var end = states[i].StateChangeDate.Add(states[i].InStateDuration);
                                //end = end < endDate ? end : endDate;
                                //var start = states[i].StateChangeDate < startDate
                                //    ? startDate
                                //    : states[i].StateChangeDate;
                                //var total = (end - start).TotalHours;
                                //if (total > 0) wh += total;
                                wh += CalcTime(states[i], startDate, endDate);
                                child.WorkedHours += Math.Round(wh, 2);
                            }
                            else if (states[i].StateChangeDate > startDate)
                            {
                                await TryToLoadMoreStates(startDate, states[i].StateChangeDate, child);
                            }
                        }
                       
                       // child.WorkedHours = Math.Round(wh, 2);
                        kc.AddChild(child);
                    }
                }
            };
            OnPropertyChanged(() => Items);
        }

        private double CalcTime(CompUnitStateDTO state, DateTime startDate, DateTime endDate)
        {
            var end = state.StateChangeDate.Add(state.InStateDuration);
            end = end < endDate ? end : endDate;
            var start = state.StateChangeDate < startDate
                ? startDate
                : state.StateChangeDate;
            var total = (end - start).TotalHours;
            return total > 0 ? total : 0;
        }

        private async Task TryToLoadMoreStates(DateTime startdate, DateTime enddate, FuelGasInputItem child)
        {
            var allstates = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                   new GetCompUnitStateListParameterSet
                   {
                       SiteId = SelectedSiteId,
                       Timestamp = enddate.Subtract(new TimeSpan(1))
                   });


            var states = allstates.Where(dto => dto.CompUnitId == child.Entity.Id).ToList();

            if (states.Count == 1)
            {
                if (states[0].State == DTO.Dictionaries.StatesModel.CompUnitState.Work)
                {
                    if (states[0].StateChangeDate <= startDate)
                    {
                        child.WorkedHours += Math.Round(CalcTime(states[0], startdate, enddate), 2);
                    }
                    else
                    {
                        await TryToLoadMoreStates(startDate, states[0].StateChangeDate, child);
                    }
                }
                else if (states[0].StateChangeDate > startDate)
                {
                    await TryToLoadMoreStates(startDate, states[0].StateChangeDate, child);
                }
            }
        }

        List<GasCostDTO> GasCosts = new List<GasCostDTO>();
        List<CompUnitStateDTO> UnitStates = new List<CompUnitStateDTO>();

        public async void LoadTree()
        {
            Behavior.TryLock();

            var serie = await new SeriesDataServiceProxy().GetSeriesAsync(
                new GetSeriesParameterSet
                {
                     PeriodType= PeriodType.Day,
                     TimeStamp = SelectedDate,
                });

            GasCosts =
               await
                   new GasCostsServiceProxy().GetGasCostListAsync(new GetGasCostListParameterSet
                   {
                       StartDate = SelectedDate,
                       EndDate = null,
                       PrdType = PeriodType.Day,
                       SiteId = SelectedSiteId,
                       SeriesId= serie==null? -1 : serie.Id,
                   });

            if(Coef!=1)
                foreach (var gasCostDTO in GasCosts)
                {
                    gasCostDTO.MeasuredVolume = gasCostDTO.MeasuredVolume * Coef;
                    gasCostDTO.CalculatedVolume = gasCostDTO.CalculatedVolume * Coef;
                }

            UnitStates = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                   new GetCompUnitStateListParameterSet
                   {
                       SiteId = SelectedSiteId,
                       Timestamp = endDate
                   });



            var dataProvider = new ObjectModelServiceProxy();
            await LoadTree(await dataProvider.GetCompStationTreeAsync(
                SelectedSiteId));


            

            Behavior.TryUnlock();
        }
        
    }
    #endregion

    public class FuelGasInputItem : PropertyChangedBase
    {
        public FuelGasInputItem(EntityDTO shop, int Coef)
        {
            Entity = shop;
            StationName = Entity.Name;
            WorkedHours = 0;
            Children = new List<FuelGasInputItem>();
            IsEditorVisible = true;
            this.Coef = Coef;
        }

        int Coef = 1;
        public CommonEntityDTO Entity { get; set; }

        public List<GasCostDTO> gasCostDTO = new List<GasCostDTO>();
        public bool IsEditorVisible { get; set; }
        public bool IsReadOnly { get { return Children.Count == 0; } }

        public double _gasVolume = 0;
        public double GasVolume
        {
            get { return _gasVolume; }
            set
            {
                double precision = Coef > 1 ? 1.0 : 1000.0;
                _gasVolume = value;

                //if (_gasVolume > 0 && _gasVolume < 3 && WorkedHours == 72) return;

                if (Children.Count > 0 && WorkedHours > 0)
                {
                    // проводятся рассчеты в случае, если дробная часть становится в промежутке
                    // пример дан ниже:
                    // m3 -> 10.0 / 3 = {3.333, 3.333, 3.334} 
                    // m  -> 10   / 3 = {4, 3, 3} (вместо 3.3333333..... в обоих случаях)

                    double perhour = _gasVolume / WorkedHours;
                    foreach (FuelGasInputItem item in Children)
                    {
                        item.GasVolume = perhour * item.WorkedHours;
                        item.GasVolume = ((int)(item.GasVolume * precision)) / precision;
                    }

                    double sum = 0;

                    for (int i = 0; i <= Children.Count - 1; i++)
                    {
                        sum += Children[i].GasVolume;
                    }
                    if (sum != _gasVolume)
                    {
                        double epsilon = Math.Round(_gasVolume - sum, 3);
                        double minVal = 0.001;

                        if (Coef > 1)
                        {
                            epsilon = Math.Round(_gasVolume - sum, 0);
                            minVal = 1;
                        }
                        
                        ShareEpsilon(Children.Where(ch => ch.WorkedHours > 0).OrderByDescending(ch => ch.WorkedHours).ToList(), minVal, epsilon);
                    }
                }

                OnPropertyChanged(() => GasVolume);
            }
        }

        private void ShareEpsilon(List<FuelGasInputItem> Items, double minVal, double epsilon) 
        {
            for(int i=0; i<Items.Count;i++)
            {
                Items[i].GasVolume += minVal;
                epsilon -= minVal;
                if (epsilon < minVal) break;
            }
        }

        public double WorkedHours { get; set; }

        public TimeSpan WorkedHoursts { get { return TimeSpan.FromHours(WorkedHours); } }

        public string StationName { get; set; }
        
        
        [Display(AutoGenerateField = false)]
        public List<FuelGasInputItem> Children
        {
            get; set;
        }

        public void AddChild(FuelGasInputItem child)
        {
            Children.Add(child);
            WorkedHours += child.WorkedHours;
        }

        //public DoubleMeasuringEditable GroupCount => Measurings.GetOrDefault(PropertyType.GroupCount);
        //public DoubleMeasuringEditable CompressionStageCount => Measurings.GetOrDefault(PropertyType.CompressionStageCount);
    }
}

#region trash
//public void LoadTree(TreeDataDTO data)
//{
//    TreeData = data;
//    Items = new List<FuelGasInputItem>();
//    //            var CompShops = data.CompShops.Cast<EntityDTO>().ToList();
//    TreeData.CompStations.ForEach(compStationDTO =>
//    {
//
//    });
//
//    foreach (var compStationDTO in TreeData.CompStations)
//    {
//        foreach (var compShopDTO in TreeData.CompShops.Where(c => c.ParentId == compStationDTO.Id))// КЦ
//        {
//            var kc = new FuelGasInputItem(compShopDTO);
//            Items.Add(kc);
//            kc._gasVolume = 0;
//            kc._gasVolume = 0;
//            foreach (var compUnitDTO in TreeData.CompUnits.Where(c => c.ParentId == compShopDTO.Id))// ГПА
//            {
//                //qty
//                var child = new FuelGasInputItem(compUnitDTO);
//                var _currentCosts = GasCosts.Where(dto => dto.Entity.Id == child.Entity.Id && dto.CostType == CostType.CT12).ToList();
//                if (_currentCosts != null) child.gasCostDTO.AddRange(_currentCosts);
//                child.GasVolume = child.gasCostDTO != null ? _currentCosts.Sum(x => x.Volume) : 0;
//                kc._gasVolume += child._gasVolume;
//                //hour
//                var states = UnitStates.Where(dto => dto.CompUnitId == child.Entity.Id).ToList();
//                double wh = 0;
//                for (int i = 0; i < states.Count; i++)
//                {
//                    if (states[i].State == DTO.Dictionaries.StatesModel.CompUnitState.Work)
//                    {
//                        var end = states[i].StateChangeDate + states[i].InStateDuration;
//                        end = end < endDate ? end : endDate;
//                        var start = states[i].StateChangeDate < startDate ? startDate : states[i].StateChangeDate;
//                        var total = (end - start).TotalHours;
//                        if (total > 0) wh += total;
//                    }
//                }
//                child.WorkedHours = Math.Round(wh, 2);
//                kc.AddChild(child);
//            }
//        }
//    }
//    OnPropertyChanged(() => Items);
//}
#endregion