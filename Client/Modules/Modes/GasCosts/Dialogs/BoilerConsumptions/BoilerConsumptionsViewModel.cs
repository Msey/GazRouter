using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.HeatingCosts;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.MeasuringLoader;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Utils.Extensions;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.BoilerConsumptions
{
    public class BoilerConsumptionsViewModel : DialogViewModel
    {
        public BoilerConsumptionsViewModel(Action closeCallback, List<GasCostDTO> currentDayCosts, DateTime selectedDate, ConsumptionViewModelBase[] consumptions, Guid selectedSiteId, List<DefaultParamValues> defaultParamValues, bool showDayly, int coef)
            : base(closeCallback)
        {
            this.defaultParamValues = defaultParamValues;
            this.showDayly = showDayly;
            this.Coef = coef;
            this.selectedSiteId = selectedSiteId;
            this.currentDayCosts = currentDayCosts;
            this.selectedDate = selectedDate;
            SaveCommand = new DelegateCommand(Save);
            LoadTree(consumptions);
        }

        public DelegateCommand SaveCommand { get; set; }
        private readonly List<DefaultParamValues> defaultParamValues;
        private readonly bool showDayly;
        private readonly DateTime selectedDate;
        private readonly Guid selectedSiteId;
        public  readonly int Coef;
        public string InputDate => selectedDate.ToLongDateString();
        private async void Save()
        {
            Behavior.TryLock();

            if (Items.Any(x => x.HasErrors))
            {
                MessageBoxProvider.Alert("Было допущено несколько ошибок валидации.\n" +
                                         "Сохранение отменено.", "Ошибка", "Ок");
                Behavior.TryUnlock();
                return;
            }

            try
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    await Items[i].Save();
                }
            }
            finally
            {
                Behavior.TryUnlock();
                DialogResult = true;
            }
        }

        private List<GasCostDTO> currentDayCosts;

        private List<BoilerConsumptionsInputModel> _items;
        public List<BoilerConsumptionsInputModel> Items { get { return _items; } private set { SetProperty(ref _items, value); } }

        public string VolumeType => ((Coef > 1) ? "м3" : "тыс.м3");
        public string FormatType => ((Coef > 1) ? "F0" : "n3");


        private async void LoadTree(ConsumptionViewModelBase[] consumptions)
        {
            Behavior.TryLock();
            
            var _Items = new List<BoilerConsumptionsInputModel>();

            var previousDayCosts = (await new GasCostsServiceProxy().GetGasCostListAsync(new GetGasCostListParameterSet
            {
                StartDate = selectedDate.AddDays(-1),
                EndDate = selectedDate.AddDays(-1),
                SiteId = selectedSiteId
            }));

            TreeDataDTO allTreeData;
            CostType? extractedCostType = CostType.CT14;

            if (consumptions.Length > 1)
            {
                allTreeData = ExtractConsumptionFromBase<CompStationConsumptionViewModel>(consumptions, out extractedCostType).TreeData;

                foreach (EntityDTO station in allTreeData.CompStations)
                {
                    AddParentBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, station, extractedCostType);
                }

                allTreeData = ExtractConsumptionFromBase<PipelineConsumptionViewModel>(consumptions, out extractedCostType).TreeData;

                foreach (var station in allTreeData.Pipelines)
                {
                    var pipelineStation = new BoilerPlantDTO();
                    pipelineStation.Id = station.Id;
                    pipelineStation.Name = station.Name;
                    AddBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, pipelineStation, null, extractedCostType);
                }

                allTreeData = ExtractConsumptionFromBase<DistrStationConsumptionViewModel>(consumptions, out extractedCostType).TreeData;

                foreach (EntityDTO station in allTreeData.DistrStations)
                {
                    AddBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, station, null, extractedCostType);
                }

                allTreeData = ExtractConsumptionFromBase<MeasStationConsumptionViewModel>(consumptions, out extractedCostType).TreeData;

                foreach (EntityDTO station in allTreeData.MeasStations)
                {
                    AddBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, station, null, extractedCostType);
                }
            }
            else
            {

                if ((allTreeData = ExtractConsumptionFromBase<CompStationConsumptionViewModel>(consumptions, out extractedCostType)?.TreeData) != null)
                {
                    foreach (EntityDTO station in allTreeData.CompStations)
                    {
                        AddParentBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, station, extractedCostType);
                    }
                }
                else if ((allTreeData = ExtractConsumptionFromBase<PipelineConsumptionViewModel>(consumptions, out extractedCostType)?.TreeData) != null)
                {
                    foreach (PipelineDTO station in allTreeData.Pipelines)
                    {
                        var pipelineStation = new BoilerPlantDTO();
                        pipelineStation.Id = station.Id;
                        pipelineStation.Name = station.Name;
                        AddBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, pipelineStation, null, extractedCostType);
                    }
                }
                else if ((allTreeData = ExtractConsumptionFromBase<DistrStationConsumptionViewModel>(consumptions, out extractedCostType)?.TreeData) != null)
                {
                    foreach (EntityDTO station in allTreeData.DistrStations)
                    {
                        AddBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, station, null, extractedCostType);
                    }
                }
                else if ((allTreeData = ExtractConsumptionFromBase<MeasStationConsumptionViewModel>(consumptions, out extractedCostType)?.TreeData) != null)
                {
                    foreach (EntityDTO station in allTreeData.MeasStations)
                    {
                        AddBoiler(selectedSiteId, _Items, previousDayCosts, allTreeData, station, null, extractedCostType);
                    }
                }
            }

            Items = _Items;
            OnPropertyChanged(() => Items);
            Behavior.TryUnlock();
        }

        private T ExtractConsumptionFromBase<T>(ConsumptionViewModelBase[] items, out CostType? costType) where T : ConsumptionViewModelBase
        {
            var consumption = items.SingleOrDefault(item => item as T != null) as T;
            costType = consumption?.GetCostTypeCollection().FirstOrDefault(cost => cost == CostType.CT14 ||
                                                                                   cost == CostType.CT36 || 
                                                                                   cost == CostType.CT45 ||
                                                                                   cost == CostType.CT52);
            return consumption;
        }

        private void AddParentBoiler(Guid selectedSiteId, List<BoilerConsumptionsInputModel> items, List<GasCostDTO> previousDayCosts, TreeDataDTO treeData, EntityDTO station, CostType? costType)
        {
            foreach (EntityDTO boilerPlantDTO in treeData.BoilerPlants.Where(plantdto => plantdto.ParentId == station.Id))
            {
                AddBoiler(selectedSiteId, items, previousDayCosts, treeData, station, boilerPlantDTO, costType);
            }
        }

        private void AddBoiler(Guid selectedSiteId, List<BoilerConsumptionsInputModel> items, List<GasCostDTO> previousDayCosts, TreeDataDTO treeData, EntityDTO station, EntityDTO boilerPlantDTO, CostType? costType)
        {
            foreach (EntityDTO boilerDTO in treeData.Boilers.Where(boilerdto => (boilerdto.SiteId == selectedSiteId) && (boilerdto.ParentId == station.Id || boilerdto.ParentId == boilerPlantDTO?.Id)))
            {
                BoilerConsumptionsInputModel boiler = new BoilerConsumptionsInputModel(null, defaultParamValues, ClientCache);

                foreach (var gasCost in currentDayCosts.Where(dto => dto.Entity != null && dto.Entity.Id == boilerDTO.Id && (dto.CostType == costType)))
                {
                    BoilerConsumptionsInputModel boilerCost = new BoilerConsumptionsInputModel(gasCost, defaultParamValues, ClientCache, Coef)
                    {
                        LocationName = station.Name,
                        ItemName = station.Name + " / " + boilerDTO.Name
                    };

                    boiler.AddChild(boilerCost);
                    items.Add(boilerCost);
                }

                if (boiler.Children.Count <= 0)
                {
                    var gasCost =
                        new GasCostDTO
                        {
                            Date = selectedDate,
                            CostType = (CostType)costType,
                            Entity = boilerDTO,
                            SiteId = selectedSiteId,
                            ChangeDate = selectedDate,
                            ChangeUserName = UserProfile.Current.UserName,
                            Target = Target.Fact
                        };

                    BoilerConsumptionsInputModel boilerCostPreviousDayPrototype = new BoilerConsumptionsInputModel(previousDayCosts.LastOrDefault(x => x.Entity.Id == boilerDTO.Id),
                                                                                                                   defaultParamValues, ClientCache, Coef);

                    BoilerConsumptionsInputModel boilerCost = new BoilerConsumptionsInputModel(gasCost, defaultParamValues, ClientCache, Coef)
                    {
                        LocationName = station.Name,
                        ItemName = ((boilerPlantDTO != null) ? boilerPlantDTO.Name + " / " : string.Empty) + boilerDTO.Name,
                        CombHeat = (boilerCostPreviousDayPrototype.Entity != null) ? boilerCostPreviousDayPrototype.CombHeat : CombustionHeat.FromKCal(7000),
                        LightingCount = (boilerCostPreviousDayPrototype.Entity != null) ? boilerCostPreviousDayPrototype.LightingCount : 5,
                        Period = 24,
                        ShutdownPeriod = (boilerCostPreviousDayPrototype.Entity != null) ? boilerCostPreviousDayPrototype.ShutdownPeriod : 5
                    };

                    items.Add(boilerCost);
                }

            }
        }
    }




    public class BoilerConsumptionsInputModel : ValidationViewModel
    {
        public BoilerConsumptionsInputModel(GasCostDTO gasCost, List<DefaultParamValues> defaultParamValues, IClientCache clientCache, int coef = 1)
        {
            Children = new List<BoilerConsumptionsInputModel>();
            Coef = coef;

            if (gasCost != null)
            {
                IsVisibleBoiler = true;
                date = gasCost.Date;
                EventDateRangeStart = date.MonthStart();
                EventDateRangeEnd = date.MonthEnd();
                Entity = gasCost.Entity;
                CostType = gasCost.CostType;
                TargetId = gasCost.Target;
                SiteId = gasCost.SiteId;
                Id = gasCost.Id != 0 ? gasCost.Id : (int?)null;

                if (IsEdit)
                {
                    _result = gasCost.CalculatedVolume;
                    _measured = gasCost.MeasuredVolume;
                }

                Model = IsEdit ? ParseModel(gasCost.InputString) : new HeatingCostsModel();
                SetValidationForItem();

                LoadBoilerInfo(clientCache);
            }
            else
            {
                IsVisibleBoiler = false;
                Model = new HeatingCostsModel();
            }
        }

        public int? Id;

        public readonly int Coef;

        public List<BoilerConsumptionsInputModel> Children { get; }
        public bool IsEdit => Id.HasValue;
        public bool IsBigVisibleBoiler => IsVisibleBoiler && !Model.BoilerType.IsSmall;
        public bool IsVisibleBoiler { get; }

        private DateTime date;
        public DateTime EventDateRangeStart { get; }
        public DateTime EventDateRangeEnd { get; }
        public uint RangeInHours => 24;//(uint)(EventDateRangeEnd.AddDays(1) - EventDateRangeStart).TotalHours;

        public readonly HeatingCostsModel Model;

        private readonly CostType CostType;

        public readonly Target TargetId;

        public readonly Guid SiteId;

        public readonly CommonEntityDTO Entity;

        private string itemName;
        public string ItemName
        {
            get
            {
                return itemName;
            }
            set
            {
                if (SetProperty(ref itemName, value))
                {
                    OnPropertyChanged(() => ItemName);
                }
            }
        }

        public bool IsSmallBoiler => Model.BoilerType.IsSmall;

        private string locationName;
        public string LocationName
        {
            get
            {
                return locationName;
            }
            set
            {
                if (SetProperty(ref locationName, value))
                {
                    OnPropertyChanged(() => locationName);
                }
            }
        }

        /// <summary>
        /// Выбранная модель котла
        /// </summary>
        public BoilerType BoilerType => Model.BoilerType;

        public string BoilerTypeString => Model.BoilerType.ToString();



            /// <summary>
            /// Время работы агрегата, ч
            /// </summary>
        public int Period
        {
            get { return Model.Period; }
            set
            {
                Model.Period = value;
                OnPropertyChanged(() => Period);
                PerformCalculate();

            }
        }


        /// <summary>
        /// Кол-во растопок
        /// </summary>
        public int LightingCount
        {
            get { return Model.LightingCount; }
            set
            {
                Model.LightingCount = value;
                OnPropertyChanged(() => LightingCount);
                PerformCalculate();
            }
        }


        /// <summary>
        /// Длительность остановки между пусками котла, ч
        /// </summary>
        public int ShutdownPeriod
        {
            get { return Model.ShutdownPeriod; }
            set
            {
                Model.ShutdownPeriod = value;
                OnPropertyChanged(() => ShutdownPeriod);
                PerformCalculate();

            }
        }


        /// <summary>
        /// Низшая теплота сгорания газа, ккал/м³
        /// </summary>
        public CombustionHeat CombHeat
        {
            get { return Model.CombHeat; }
            set
            {
                Model.CombHeat = value;
                OnPropertyChanged(() => CombHeat);
                PerformCalculate();
            }
        }

        private void SetValidationForItem()
        {
            AddValidationFor(() => Period)
        .When(() => Period <= 0 || Period > RangeInHours)
        .Show(string.Format("Недопустимое значение (интервал допустимых значений 1 - {0})", RangeInHours));

            AddValidationFor(() => LightingCount)
                .When(() => !BoilerType.IsSmall && (LightingCount < 0 || LightingCount > 60))
                .Show("Недопустимое значение (интервал допустимых значений 0 - 60)");

            AddValidationFor(() => ShutdownPeriod)
                .When(() => !BoilerType.IsSmall && (ShutdownPeriod < 0 || ShutdownPeriod > RangeInHours))
                .Show(string.Format("Недопустимое значение (интервал допустимых значений 0 - {0})", RangeInHours));

            AddValidationFor(() => CombHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);

            AddValidationFor(() => MeasuredInputField)
                .When(() => MeasuredInputField <= 0)
                .Show("Недопустимое значение.Должно быть больше 0.");
        }

        private HeatingCostsModel ParseModel(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return new HeatingCostsModel();
            var xmlSerializer = new XmlSerializer(typeof(HeatingCostsModel));
            TextReader tr = new StringReader(inputString);
            return (HeatingCostsModel)xmlSerializer.Deserialize(tr);
        }

        protected string GetInputDataString()
        {
            using (TextWriter tw = new StringWriter())
            {
                new XmlSerializer(typeof(HeatingCostsModel)).Serialize(tw, Model);
                return tw.ToString();
            }
        }

        private async void LoadBoilerInfo(IClientCache ClientCache)
        {

            var boiler = await new ObjectModelServiceProxy().GetBoilerByIdAsync(Entity.Id);

            var boilerType =
                ClientCache.DictionaryRepository.BoilerTypes.FirstOrDefault(b => b.Id == boiler.BoilerTypeId);
            Model.BoilerType = new BoilerType(boilerType);

            Model.HeatLossFactor = boiler.HeatLossFactor;
            Model.HeatSupplySystemLoad = boiler.HeatSupplySystemLoad;

            OnPropertyChanged(() => BoilerTypeString);
            OnPropertyChanged(() => ItemName);
            OnPropertyChanged(() => IsBigVisibleBoiler);
            OnPropertyChanged(() => IsSmallBoiler);
            PerformCalculate();

        }

        private double? _measured;
        private double? _result;

        public double? Result
        {
            get
            {
                return _result * Coef;
            }
            set
            {
                SetProperty(ref _result, value);
            }
        }

        public double? Measured
        {
            get
            {
                return _measured * Coef;
            }
            set
            {
                if (SetProperty(ref _measured, value))
                    PerformCalculate();
            }
        }

        public double? MeasuredInputField
        {
            get
            {
                return _measured;
            }
            set
            {
                if (SetProperty(ref _measured, value)) PerformCalculate();
            }
        }

        public MeasuringLoaderViewModel MeasuringLoader { get; set; }
        protected virtual void PerformCalculate()
        {
            ValidateAll();
            Result = Math.Round(Model.Calculate(), 3);
        }

        public void AddChild(BoilerConsumptionsInputModel child) => Children.Add(child);

        public async Task Save()
        {
            var inputDataString = GetInputDataString();
            if (!Id.HasValue)
            {
                var addGasCostParameterSet = new AddGasCostParameterSet
                {
                    CalculatedVolume = Result / (Coef),
                    MeasuredVolume = _measured / (Coef),
                    Date = date,
                    EntityId = Entity.Id,
                    CostType = CostType,
                    Target = TargetId,
                    InputData = inputDataString,
                    SiteId = SiteId
                };
                Id = await new GasCostsServiceProxy().AddGasCostAsync(addGasCostParameterSet);
            }
            else
            {
                var parameterSet = new EditGasCostParameterSet
                {
                    CostId = Id.Value,
                    CalculatedVolume = Result / (Coef),
                    MeasuredVolume = _measured / (Coef),
                    Date = date,
                    EntityId = Entity.Id,
                    CostType = CostType,
                    Target = TargetId,
                    InputData = inputDataString,
                    SiteId = SiteId
                };
                await new GasCostsServiceProxy().EditGasCostAsync(parameterSet);
            }
        }
    }
}
