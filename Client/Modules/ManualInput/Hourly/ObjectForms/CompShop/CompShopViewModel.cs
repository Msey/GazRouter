using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.ManualInput.Hourly.ObjectForms.CompShop
{
    public class CompShopViewModel : EntityForm
    {

        public CompShopViewModel(ManualInputEntityNode node, DateTime date, int serieId)
            : base (node, date, serieId)
        {
            UnitList = new List<ManualInputCompUnit>();
            
            PressureInlet = new ManualInputPropertyValue();
            TemperatureInlet = new ManualInputPropertyValue();
            PressureOutlet = new ManualInputPropertyValue();
            TemperatureOutlet = new ManualInputPropertyValue();
            TemperatureCooling = new ManualInputPropertyValue();

            FuelGasConsumption = new ManualInputPropertyValue();
            Pumping = new ManualInputPropertyValue();

            CoolingUnitsInUse = new ManualInputPropertyValue();
            CoolingUnitsInReserve = new ManualInputPropertyValue();
            CoolingUnitsUnderRepair = new ManualInputPropertyValue();

            GroupCount = new ManualInputPropertyValue();
            GroupCount.PropertyChanged += (sender, args) =>
            {
                if (GroupCount.Value.HasValue && GroupCount.Value == 0) StageCount.Value = 0;
                OnPropertyChanged(() => Scheme);
            };
            StageCount = new ManualInputPropertyValue();
            StageCount.PropertyChanged += (sender, args) => OnPropertyChanged(() => Scheme);


            DustCatchersInUse = new ManualInputPropertyValue();
            DustCatchersInReserve = new ManualInputPropertyValue();
            DustCatchersUnderRepair = new ManualInputPropertyValue();


            SetValidationRules();
        }

        
        public override async void Load()
        {
            Behavior.TryLock();
            try
            {
                // получаем список ГПА
                var unitList =
                    await
                        new ObjectModelServiceProxy().GetCompUnitListAsync(new GetCompUnitListParameterSet
                        {
                            ShopId = NodeId
                        });

                // Теперь нужно исключить из списка агрегаты, которые находятся в резерве или ремонте
                // Для этого получаем состояния ГПА по выбранному цеху за указанную дату
                var stateList = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                    new GetCompUnitStateListParameterSet
                    {
                        ShopId = NodeId,
                        Timestamp = Date
                    });
                
                // получаем значения параметров
                var entityList = unitList.Select(u => u.Id).ToList();
                entityList.Add(NodeId);
                var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = entityList,
                        StartDate = Date,
                        EndDate = Date,
                        PeriodType = PeriodType,
                        LoadMessages = true
                    });

                UnitList = unitList.Select(u =>
                    new ManualInputCompUnit
                    {
                        Id = u.Id,
                        Name = u.Name,
                        State = stateList.Any(s => s.CompUnitId == u.Id) ? stateList.Single(s => s.CompUnitId == u.Id).State : CompUnitState.Undefined
                    }).ToList();
                OnPropertyChanged(() => UnitList);
                OnPropertyChanged(() => InUseUnitList);

                
                ExtractPropertyValue(PressureInlet, propValues, NodeId, PropertyType.PressureInlet, Date);
                ExtractPropertyValue(PressureOutlet, propValues, NodeId, PropertyType.PressureOutlet, Date);
                ExtractPropertyValue(TemperatureInlet, propValues, NodeId, PropertyType.TemperatureInlet, Date);
                ExtractPropertyValue(TemperatureOutlet, propValues, NodeId, PropertyType.TemperatureOutlet, Date);
                ExtractPropertyValue(TemperatureCooling, propValues, NodeId, PropertyType.TemperatureCooling, Date);
                ExtractPropertyValue(FuelGasConsumption, propValues, NodeId, PropertyType.FuelGasConsumption, Date);
                ExtractPropertyValue(Pumping, propValues, NodeId, PropertyType.Pumping, Date);

                ExtractPropertyValue(CoolingUnitsInUse, propValues, NodeId, PropertyType.CoolingUnitsInUse, Date);
                ExtractPropertyValue(CoolingUnitsInReserve, propValues, NodeId, PropertyType.CoolingUnitsInReserve, Date);
                ExtractPropertyValue(CoolingUnitsUnderRepair, propValues, NodeId, PropertyType.CoolingUnitsUnderRepair, Date);

                ExtractPropertyValue(GroupCount, propValues, NodeId, PropertyType.GroupCount, Date);
                ExtractPropertyValue(StageCount, propValues, NodeId, PropertyType.CompressionStageCount, Date);

                ExtractPropertyValue(DustCatchersInUse, propValues, NodeId, PropertyType.DustCatchersInUse, Date);
                ExtractPropertyValue(DustCatchersInReserve, propValues, NodeId, PropertyType.DustCatchersInReserve, Date);
                ExtractPropertyValue(DustCatchersUnderRepair, propValues, NodeId, PropertyType.DustCatchersUnderRepair, Date);
                

                foreach (var unit in UnitList)
                {
                    ExtractPropertyValue(unit.PressureSuperchargerInlet, propValues, unit.Id, PropertyType.PressureSuperchargerInlet, Date);
                    ExtractPropertyValue(unit.PressureSuperchargerOutlet, propValues, unit.Id, PropertyType.PressureSuperchargerOutlet, Date);
                    ExtractPropertyValue(unit.TemperatureSuperchargerInlet, propValues, unit.Id, PropertyType.TemperatureSuperchargerInlet, Date);
                    ExtractPropertyValue(unit.TemperatureSuperchargerOutlet, propValues, unit.Id, PropertyType.TemperatureSuperchargerOutlet, Date);
                    ExtractPropertyValue(unit.FuelGasConsumption, propValues, unit.Id, PropertyType.FuelGasConsumption, Date);
                    ExtractPropertyValue(unit.Pumping, propValues, unit.Id, PropertyType.Pumping, Date);
                    ExtractPropertyValue(unit.PressureFallConfusor, propValues, unit.Id, PropertyType.PressureFallConfusor, Date);

                    ExtractPropertyValue(unit.RpmSupercharger, propValues, unit.Id, PropertyType.RpmSupercharger, Date);
                    ExtractPropertyValue(unit.RpmHighHeadTurbine, propValues, unit.Id, PropertyType.RpmHighHeadTurbine, Date);
                    ExtractPropertyValue(unit.RpmLowHeadTurbine, propValues, unit.Id, PropertyType.RpmLowHeadTurbine, Date);

                    ExtractPropertyValue(unit.TemperatureHighHeadTurbineInlet, propValues, unit.Id, PropertyType.TemperatureHighHeadTurbineInlet, Date);
                    ExtractPropertyValue(unit.TemperatureHighHeadTurbineOutlet, propValues, unit.Id, PropertyType.TemperatureHighHeadTurbineOutlet, Date);
                    ExtractPropertyValue(unit.TemperatureLowHeadTurbineInlet, propValues, unit.Id, PropertyType.TemperatureLowHeadTurbineInlet, Date);
                    ExtractPropertyValue(unit.TemperatureLowHeadTurbineOutlet, propValues, unit.Id, PropertyType.TemperatureLowHeadTurbineOutlet, Date);
                    ExtractPropertyValue(unit.TemperatureFreeTurbineInlet, propValues, unit.Id, PropertyType.TemperatureFreeTurbineInlet, Date);
                    ExtractPropertyValue(unit.TemperatureFreeTurbineOutlet, propValues, unit.Id, PropertyType.TemperatureFreeTurbineOutlet, Date);

                    ExtractPropertyValue(unit.PressureAxialFlowCompressorOutlet, propValues, unit.Id, PropertyType.PressureAxialFlowCompressorOutlet, Date);
                    ExtractPropertyValue(unit.TemperatureAxialFlowCompressorInlet, propValues, unit.Id, PropertyType.TemperatureAxialFlowCompressorInlet, Date);
                    ExtractPropertyValue(unit.TemperatureBearing, propValues, unit.Id, PropertyType.TemperatureBearing, Date);

                    if (unit.State != CompUnitState.Work) unit.ZeroValues();
                }

                
                
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        

        /// <summary>
        /// Список ГПА
        /// </summary>
        public List<ManualInputCompUnit> UnitList { get; set; }

        /// <summary>
        /// Список ГПА в работе
        /// Ввод данных осуществляется только по ним
        /// </summary>
        public List<ManualInputCompUnit> InUseUnitList
        {
            get { return UnitList.Where(u => u.State == CompUnitState.Work).ToList(); }
            //get { return UnitList; }
        }


        /// <summary>
        /// Давление газа на входе КЦ, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureInlet { get; set; }


        /// <summary>
        /// Давление газа на выходе КЦ, кг/см²
        /// </summary>
        public ManualInputPropertyValue PressureOutlet { get; set; }

        
        /// <summary>
        /// Температура газа на входе КЦ, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureInlet { get; set; }


        /// <summary>
        /// Температура газа на выходе КЦ, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureOutlet { get; set; }


        /// <summary>
        /// Температура газа после АВО, Гр.С
        /// </summary>
        public ManualInputPropertyValue TemperatureCooling { get; set; }


        /// <summary>
        /// Расход топливного газа, тыс.м3
        /// </summary>
        public ManualInputPropertyValue FuelGasConsumption { get; set; }


        /// <summary>
        /// Объем газа, перекачиваемый КЦ, тыс.м3
        /// </summary>
        public ManualInputPropertyValue Pumping { get; set; }



        /// <summary>
        /// Кол-во АВО в работе, шт.
        /// </summary>
        public ManualInputPropertyValue CoolingUnitsInUse { get; set; }

        /// <summary>
        /// Кол-во АВО в резерве, шт.
        /// </summary>
        public ManualInputPropertyValue CoolingUnitsInReserve { get; set; }

        /// <summary>
        /// Кол-во АВО в ремонте, шт.
        /// </summary>
        public ManualInputPropertyValue CoolingUnitsUnderRepair { get; set; }




        /// <summary>
        /// Кол-во групп
        /// </summary>
        public ManualInputPropertyValue GroupCount { get; set; }


        /// <summary>
        /// Кол-во ступеней
        /// </summary>
        public ManualInputPropertyValue StageCount { get; set; }




        /// <summary>
        /// Кол-во п/у в работе, шт.
        /// </summary>
        public ManualInputPropertyValue DustCatchersInUse { get; set; }

        /// <summary>
        /// Кол-во п/у в резерве, шт.
        /// </summary>
        public ManualInputPropertyValue DustCatchersInReserve { get; set; }

        /// <summary>
        /// Кол-во п/у в ремонте, шт.
        /// </summary>
        public ManualInputPropertyValue DustCatchersUnderRepair { get; set; }


        /// <summary>
        /// Схема работы для отображения
        /// </summary>
        public string Scheme
        {
            get
            {
                if (GroupCount == null || !GroupCount.Value.HasValue) return "";
                if (StageCount == null || !StageCount.Value.HasValue) return "";
                return string.Format("{0:0}x{1:0}", GroupCount.Value.Value, StageCount.Value.Value);
            }
        }
        

        private void SetValidationRules()
        {
            PressureInlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureInlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureInlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            PressureOutlet.GetPropertyValidation()
                .When(
                    () =>
                        PressureOutlet.Value < ValueRangeHelper.OldPressureRange.Min ||
                        PressureOutlet.Value > ValueRangeHelper.OldPressureRange.Max)
                .Show(ValueRangeHelper.OldPressureRange.ViolationMessage);

            
            TemperatureInlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureInlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureInlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureOutlet.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureOutlet.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureOutlet.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);

            TemperatureCooling.GetPropertyValidation()
                .When(
                    () =>
                        TemperatureCooling.Value < ValueRangeHelper.OldTemperatureRange.Min ||
                        TemperatureCooling.Value > ValueRangeHelper.OldTemperatureRange.Max)
                .Show(ValueRangeHelper.OldTemperatureRange.ViolationMessage);


            FuelGasConsumption.GetPropertyValidation()
                .When(() => FuelGasConsumption.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");

            Pumping.GetPropertyValidation()
                .When(() => Pumping.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");



            CoolingUnitsInUse.GetPropertyValidation()
                .When(() => CoolingUnitsInUse.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");

            CoolingUnitsInReserve.GetPropertyValidation()
                .When(() => CoolingUnitsInReserve.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");

            CoolingUnitsUnderRepair.GetPropertyValidation()
                .When(() => CoolingUnitsUnderRepair.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");



            StageCount.GetPropertyValidation()
                .When(() => StageCount.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");

            GroupCount.GetPropertyValidation()
                .When(() => GroupCount.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");


            DustCatchersInUse.GetPropertyValidation()
                .When(() => DustCatchersInUse.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");

            DustCatchersInReserve.GetPropertyValidation()
                .When(() => DustCatchersInReserve.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");

            DustCatchersUnderRepair.GetPropertyValidation()
                .When(() => DustCatchersUnderRepair.Value < 0)
                .Show("Недопустимое значение. Не может быть меньше 0.");
        }


        public override async void Save()
        {
            Behavior.TryLock();
            try
            {
               
                var serie = await new SeriesDataServiceProxy().AddSerieAsync(
                    new AddSeriesParameterSet
                    {
                        KeyDate = Date,
                        PeriodTypeId = PeriodType
                    });
               

                var valueList = new List<SetPropertyValueParameterSet>();

                if (PressureInlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.PressureInlet,
                        Value = PressureInlet.Value.Value
                    });

                if (PressureOutlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.PressureOutlet,
                        Value = PressureOutlet.Value.Value
                    });

                if (TemperatureInlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureInlet,
                        Value = TemperatureInlet.Value.Value
                    });

                if (TemperatureOutlet.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureOutlet,
                        Value = TemperatureOutlet.Value.Value
                    });

                if (TemperatureCooling.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.TemperatureCooling,
                        Value = TemperatureCooling.Value.Value
                    });



                if (FuelGasConsumption.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.FuelGasConsumption,
                        Value = FuelGasConsumption.Value.Value
                    });

                if (Pumping.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.Pumping,
                        Value = Pumping.Value.Value
                    });


                if (CoolingUnitsInUse.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.CoolingUnitsInUse,
                        Value = CoolingUnitsInUse.Value.Value
                    });
                
                if (CoolingUnitsInReserve.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.CoolingUnitsInReserve,
                        Value = CoolingUnitsInReserve.Value.Value
                    });

                if (CoolingUnitsUnderRepair.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.CoolingUnitsUnderRepair,
                        Value = CoolingUnitsUnderRepair.Value.Value
                    });


                if (GroupCount.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.GroupCount,
                        Value = GroupCount.Value.Value
                    });

                if (StageCount.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.CompressionStageCount,
                        Value = StageCount.Value.Value
                    });


                if (DustCatchersInUse.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.DustCatchersInUse,
                        Value = DustCatchersInUse.Value.Value
                    });

                if (DustCatchersInReserve.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.DustCatchersInReserve,
                        Value = DustCatchersInReserve.Value.Value
                    });

                if (DustCatchersUnderRepair.Value.HasValue)
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = NodeId,
                        PropertyTypeId = PropertyType.DustCatchersUnderRepair,
                        Value = DustCatchersUnderRepair.Value.Value
                    });



                foreach (var unit in UnitList)
                {
                    
                    valueList.Add(new SetPropertyValueParameterSet
                    {
                        SeriesId = serie.Id,
                        EntityId = unit.Id,
                        PropertyTypeId = PropertyType.CompressorUnitState,
                        Value = (double)unit.State
                    });


                    if (unit.PressureSuperchargerInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.PressureSuperchargerInlet,
                            Value = unit.PressureSuperchargerInlet.Value.Value
                        });

                    if (unit.PressureSuperchargerOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.PressureSuperchargerOutlet,
                            Value = unit.PressureSuperchargerOutlet.Value.Value
                        });

                    if (unit.TemperatureSuperchargerInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureSuperchargerInlet,
                            Value = unit.TemperatureSuperchargerInlet.Value.Value
                        });

                    if (unit.TemperatureSuperchargerOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureSuperchargerOutlet,
                            Value = unit.TemperatureSuperchargerOutlet.Value.Value
                        });

                    if (unit.RpmSupercharger.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.RpmSupercharger,
                            Value = unit.RpmSupercharger.Value.Value
                        });

                    if (unit.FuelGasConsumption.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.FuelGasConsumption,
                            Value = unit.FuelGasConsumption.Value.Value
                        });

                    if (unit.Pumping.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.Pumping,
                            Value = unit.Pumping.Value.Value
                        });

                    if (unit.PressureFallConfusor.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.PressureFallConfusor,
                            Value = unit.PressureFallConfusor.Value.Value
                        });


                    // вторая вкладка
                    if (unit.TemperatureHighHeadTurbineInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureHighHeadTurbineInlet,
                            Value = unit.TemperatureHighHeadTurbineInlet.Value.Value
                        });

                    if (unit.TemperatureHighHeadTurbineOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureHighHeadTurbineOutlet,
                            Value = unit.TemperatureHighHeadTurbineOutlet.Value.Value
                        });

                    if (unit.RpmHighHeadTurbine.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.RpmHighHeadTurbine,
                            Value = unit.RpmHighHeadTurbine.Value.Value
                        });

                    if (unit.TemperatureLowHeadTurbineInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureLowHeadTurbineInlet,
                            Value = unit.TemperatureLowHeadTurbineInlet.Value.Value
                        });

                    if (unit.TemperatureLowHeadTurbineOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureLowHeadTurbineOutlet,
                            Value = unit.TemperatureLowHeadTurbineOutlet.Value.Value
                        });

                    if (unit.RpmLowHeadTurbine.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.RpmLowHeadTurbine,
                            Value = unit.RpmLowHeadTurbine.Value.Value
                        });

                    if (unit.TemperatureFreeTurbineInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureFreeTurbineInlet,
                            Value = unit.TemperatureFreeTurbineInlet.Value.Value
                        });

                    if (unit.TemperatureFreeTurbineOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureFreeTurbineOutlet,
                            Value = unit.TemperatureFreeTurbineOutlet.Value.Value
                        });

                    if (unit.PressureAxialFlowCompressorOutlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.PressureAxialFlowCompressorOutlet,
                            Value = unit.PressureAxialFlowCompressorOutlet.Value.Value
                        });

                    if (unit.TemperatureAxialFlowCompressorInlet.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureAxialFlowCompressorInlet,
                            Value = unit.TemperatureAxialFlowCompressorInlet.Value.Value
                        });

                    if (unit.TemperatureBearing.Value.HasValue)
                        valueList.Add(new SetPropertyValueParameterSet
                        {
                            SeriesId = serie.Id,
                            EntityId = unit.Id,
                            PropertyTypeId = PropertyType.TemperatureBearing,
                            Value = unit.TemperatureBearing.Value.Value
                        });
                }

                await new SeriesDataServiceProxy().SetPropertyValueAsync(valueList);

                // Выполнение проверок
                var checkList = new List<PerformCheckingParameterSet>
                {
                    new PerformCheckingParameterSet
                    {
                        EntityId = NodeId,
                        SerieId = serie.Id,
                        SaveHistory = true
                    }
                };
                checkList.AddRange(UnitList.Select(unit => new PerformCheckingParameterSet
                {
                    EntityId = unit.Id,
                    SerieId = serie.Id
                }));

                await new SeriesDataServiceProxy().PerformCheckingAsync(checkList);

                if (UpdateNodeStatus != null)
                    UpdateNodeStatus();

                Load();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
    }


    
}