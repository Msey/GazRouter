using System.Collections.Generic;
using System.Linq;
using GazRouter.Controls.Converters;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.Modes.ProcessMonitoring.ObjectStory.CompShop;
using GazRouter.Modes.ProcessMonitoring.PipelineTrend;
using Microsoft.Practices.Prism.Regions;
namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.CompShops
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class CompShopsViewModel : FormViewModelBase
    {
        public override ReportSettings GetReportSettings()
        {
            return new ReportSettings
            {
                SiteSelector = true,
                EmptySiteAllowed = true,
                SerieSelector = true,
                DetailView = true
            };
        }

        public override void Refresh()
        {
            //if (Site != null)
            //    LoadBySite();
            //else
            //    LoadByPipeline();

            LoadByPipeline();
        }
        

        public List<ItemBase> Items { get; set; }



        private async void LoadByStation()
        {
            try
            {
                Behavior.TryLock();

                // Получить список КЦ
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(Site.Id);

                // Получить текущие состояния ГПА
                var unitStates = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                    new GetCompUnitStateListParameterSet
                    {
                        SiteId = Site.Id,
                        Timestamp = Timestamp
                    });


                // получаем значения измеренных параметров
                var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = stationTree.CompShops.Select(s => s.Id).ToList(),
                        StartDate = Timestamp,
                        EndDate = Timestamp,
                        PeriodType = PeriodType.Twohours,
                        LoadMessages = true
                    });


                // Сформировать дерево
                Items = new List<ItemBase>();

                foreach (var station in stationTree.CompStations)
                {
                    var stationItem = new ItemBase(station);
                    Items.Add(stationItem);

                    foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                    {
                        var shopItem = new ShopItem(shop);
                        stationItem.Childs.Add(shopItem);


                        shopItem.Pattern.Extract(propValues, Timestamp);
                        shopItem.PressureInlet.Extract(propValues, Timestamp);
                        shopItem.PressureOutlet.Extract(propValues, Timestamp);
                        shopItem.CompressionRatio.Extract(propValues, Timestamp);
                        shopItem.TemperatureInlet.Extract(propValues, Timestamp);
                        shopItem.TemperatureOutlet.Extract(propValues, Timestamp);
                        shopItem.TemperatureCooling.Extract(propValues, Timestamp);
                        shopItem.FuelGasConsumption.Extract(propValues, Timestamp);

                        // Типы ГПА
                        shopItem.UnitTypes =
                            stationTree.CompUnits.Where(u => u.ParentId == shop.Id)
                                .Select(u => CompUnitTypeToNameConverter.Convert(u.CompUnitTypeId))
                                .Distinct()
                                .Aggregate("", (seed, n) => seed + (string.IsNullOrEmpty(seed) ? "" : ", ") + n);

                        // Добавить состояния агрегатов
                        var i = 1;
                        foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id).OrderBy(u => u.SortOrder))
                        {
                            shopItem.UnitStateList.Add(
                                new CompUnitStateItem
                                {
                                    Dto = unit,
                                    State = unitStates.Single(s => s.CompUnitId == unit.Id).State
                                });
                        }
                    }
                }

                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void LoadByPipeline()
        {
            try
            {
                Lock();

                
                // Получить список магистральных газопроводов
                var pipeList = await new ObjectModelServiceProxy().GetPipelineListAsync(
                    new GetPipelineListParameterSet
                    {
                        PipelineTypes = new List<PipelineType> {PipelineType.Main, PipelineType.Looping}
                    });

                
                // Получить список КЦ
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(Site?.Id);

                // Получить текущие состояния ГПА
                var unitStates = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                    new GetCompUnitStateListParameterSet
                    {
                        Timestamp = Timestamp,
                        SiteId = Site?.Id
                    });


                var objList = stationTree.CompShops.Select(s => s.Id).ToList();
                //objList.AddRange(stationTree.CompUnits.Select(u => u.Id));

                // получаем значения измеренных параметров
                var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = objList,
                        StartDate = Timestamp.AddHours(-2),
                        EndDate = Timestamp,
                        PeriodType = PeriodType.Twohours,
                        LoadMessages = true
                    });


                // Сформировать дерево
                Items = new List<ItemBase>();

                foreach (var pipe in pipeList)
                {
                    var pipeItem = new ItemBase(pipe);
                    
                    foreach (var shop in stationTree.CompShops.Where(cs => cs.PipelineId == pipe.Id).OrderBy(cs => cs.KmOfConn))
                    {
                        var shopItem = new ShopItem(shop) { UseShortPathAsName = true };
                        pipeItem.Childs.Add(shopItem);


                        shopItem.Pattern.Extract(propValues, Timestamp);
                        shopItem.PressureInlet.Extract(propValues, Timestamp);
                        shopItem.PressureOutlet.Extract(propValues, Timestamp);
                        shopItem.CompressionRatio.Extract(propValues, Timestamp);
                        shopItem.TemperatureInlet.Extract(propValues, Timestamp);
                        shopItem.TemperatureOutlet.Extract(propValues, Timestamp);
                        shopItem.TemperatureCooling.Extract(propValues, Timestamp);
                        shopItem.FuelGasConsumption.Extract(propValues, Timestamp);

                        // Типы ГПА
                        shopItem.UnitTypes =
                            stationTree.CompUnits.Where(u => u.ParentId == shop.Id)
                                .Select(u => CompUnitTypeToNameConverter.Convert(u.CompUnitTypeId))
                                .Distinct()
                                .Aggregate("", (seed, n) => seed + (string.IsNullOrEmpty(seed) ? "" : ", ") + n);

                        // Добавить состояния агрегатов
                        var i = 1;
                        foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id).OrderBy(u => u.SortOrder))
                        {
                            var state = unitStates.SingleOrDefault(s => s.CompUnitId == unit.Id)?.State;
                            if (state.HasValue)
                                shopItem.UnitStateList.Add(
                                    new CompUnitStateItem
                                    {
                                        Dto = unit,
                                        State = state.Value
                                    });
                        }
                    }

                    if (pipeItem.Childs.Any())
                        Items.Add(pipeItem);
                }

                OnPropertyChanged(() => Items);
            }
            finally
            {
                Unlock();
            }
        }



        public List<PressureByKmPoint> PressureByKmPointList { get; set; }

        private ItemBase _selectedItem;

        public ItemBase SelectedItem
        {
            get { return _selectedItem;}

            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    OnPropertyChanged(() => IsPipelineSelected);
                    OnPropertyChanged(() => IsCompShopSelected);

                    RefreshDetails();

                }
            }
        }

        public bool IsPipelineSelected => _selectedItem != null && _selectedItem.EntityType == EntityType.Pipeline;

        public bool IsCompShopSelected => _selectedItem != null && _selectedItem.EntityType == EntityType.CompShop;

        public CompShopStoryViewModel CompShopStory { get; set; }

        public PipelineTrendViewModel PipelineTrend { get; set; }

        public int CombineRows
        {
            get { return ShowDetails ? 1 : 2; }
        }



        public override void RefreshDetails()
        {
            OnPropertyChanged(() => ShowDetails);
            OnPropertyChanged(() => CombineRows);

            if (!ShowDetails) return;
            
            if (IsPipelineSelected)
            {
                PipelineTrend = new PipelineTrendViewModel(_selectedItem.Dto.Id, Timestamp);
                OnPropertyChanged(() => PipelineTrend);
            }
            if (IsCompShopSelected)
            {
                CompShopStory = new CompShopStoryViewModel(_selectedItem.Dto.Id, Timestamp);
                OnPropertyChanged(() => CompShopStory);
            }
        }
    }


    public class ItemBase
    {
        protected CommonEntityDTO _dto;


        public ItemBase(CommonEntityDTO dto)
        {
            _dto = dto;

            Childs = new List<ItemBase>();
        }

        public List<ItemBase> Childs { get; set; }

        public CommonEntityDTO Dto => _dto;

        public virtual string Name => _dto.Name;


        public EntityType EntityType => _dto.EntityType;

        public bool UseShortPathAsName { get; set; }

    }

    public class ShopItem : ItemBase
    {
        public ShopItem(CompShopDTO dto) : base(dto)
        {
            Pattern = new StringMeasuring(dto.Id, PropertyType.CompressorShopPattern, PeriodType.Twohours);
            PressureInlet = new DoubleMeasuring(dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, true);
            PressureOutlet = new DoubleMeasuring(dto.Id, PropertyType.PressureOutlet, PeriodType.Twohours, true);
            CompressionRatio = new DoubleMeasuring(dto.Id, PropertyType.CompressionRatio, PeriodType.Twohours);
            TemperatureInlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, true);
            TemperatureOutlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureOutlet, PeriodType.Twohours, true);
            TemperatureCooling = new DoubleMeasuring(dto.Id, PropertyType.TemperatureCooling, PeriodType.Twohours, true);
            FuelGasConsumption = new DoubleMeasuring(dto.Id, PropertyType.FuelGasConsumption, PeriodType.Twohours, true);

            UnitStateList = new List<CompUnitStateItem>();
        }

        public string UnitTypes { get; set; }

        public StringMeasuring Pattern { get; set; }
        public DoubleMeasuring PressureInlet { get; set; }
        public DoubleMeasuring PressureOutlet { get; set; }
        public DoubleMeasuring CompressionRatio { get; set; }
        public DoubleMeasuring TemperatureInlet { get; set; }
        public DoubleMeasuring TemperatureOutlet { get; set; }
        public DoubleMeasuring TemperatureCooling { get; set; }
        public DoubleMeasuring FuelGasConsumption { get; set; }


        public bool InWork => !string.IsNullOrEmpty(Pattern.Value) && Pattern.Value != "0X0";


        public List<CompUnitStateItem> UnitStateList { get; set; }

    }

    public class CompUnitStateItem
    {
        public CompUnitDTO Dto { get; set; }
        public CompUnitState State { get; set; }
    }
    
}