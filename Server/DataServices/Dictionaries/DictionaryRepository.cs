using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.AlarmTypes;
using GazRouter.DAL.Dictionaries.AnnuledReasons;
using GazRouter.DAL.Dictionaries.BalanceItems;
using GazRouter.DAL.Dictionaries.BalanceSigns;
using GazRouter.DAL.Dictionaries.BoilerTypes;
using GazRouter.DAL.Dictionaries.CompUnitFailureCauses;
using GazRouter.DAL.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DAL.Dictionaries.CompUnitRepairTypes;
using GazRouter.DAL.Dictionaries.CompUnitSealingTypes;
using GazRouter.DAL.Dictionaries.CompUnitStopTypes;
using GazRouter.DAL.Dictionaries.CompUnitTypes;
using GazRouter.DAL.Dictionaries.ConsumerTypes;
using GazRouter.DAL.Dictionaries.CoolingUnitTypes;
using GazRouter.DAL.Dictionaries.Diameters;
using GazRouter.DAL.Dictionaries.EmergencyValveTypes;
using GazRouter.DAL.Dictionaries.EngineClasses;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.EventTypes;
using GazRouter.DAL.Dictionaries.ExchangeTypes;
using GazRouter.DAL.Dictionaries.GasCostItemGroups;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.HeaterTypes;
using GazRouter.DAL.Dictionaries.InconsistencyTypes;
using GazRouter.DAL.Dictionaries.OperConsumerType;
using GazRouter.DAL.Dictionaries.ParameterTypes;
using GazRouter.DAL.Dictionaries.PeriodTypes;
using GazRouter.DAL.Dictionaries.PhysicalTypes;
using GazRouter.DAL.Dictionaries.PipelineEndType;
using GazRouter.DAL.Dictionaries.PipelineGroups;
using GazRouter.DAL.Dictionaries.PipelineTypes;
using GazRouter.DAL.Dictionaries.PlanTypes;
using GazRouter.DAL.Dictionaries.PowerUnitTypes;
using GazRouter.DAL.Dictionaries.PropertyTypes;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.Dictionaries.RegulatorTypes;
using GazRouter.DAL.Dictionaries.RepairExecutionMeans;
using GazRouter.DAL.Dictionaries.RepairTypes;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DAL.Dictionaries.States;
using GazRouter.DAL.Dictionaries.StatusTypes;
using GazRouter.DAL.Dictionaries.SuperchargerTypes;
using GazRouter.DAL.Dictionaries.SysEventTypes;
using GazRouter.DAL.Dictionaries.Targets;
using GazRouter.DAL.Dictionaries.TransportTypes;
using GazRouter.DAL.Dictionaries.ValvePurposes;
using GazRouter.DAL.Dictionaries.ValveTypes;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.Diameters;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.Dictionaries.StatesModel;


namespace GazRouter.DataServices.Dictionaries
{
    public static class DictionaryRepository
    {
        public static DictionaryRepositoryDTO Dictionaries { get; private set; }

        public static void Init(ExecutionContext context)
        {
            var pipeLineTypes = new GetPipelineTypesListQuery(context).Execute();

            var valvePurposes = new GetValvePurposeListQuery(context).Execute();

            var pipelineEndType = new GetPipelineEndTypeListQuery(context).Execute();

            var consumerTypes = new GetConsumerTypesListQuery(context).Execute();

            var engineClasses = new GetEngineClassesListQuery(context).Execute();

            var balanceSigns = new GetBalanceSignsListQuery(context).Execute();

            var balanceItems = new GetBalanceItemsListQuery(context).Execute();

            var valveTypes = new GetValveTypesListQuery(context).Execute();

            var regions = new GetRegionListQuery(context).Execute();

            var superchargerTypes = new GetSuperchargerTypesQuery(context).Execute();

            var superchargerTypePoints = new GetSuperchargerTypePointsQuery(context).Execute();
            if (superchargerTypePoints.Count > 0)
                foreach (var superchargerType in superchargerTypes)
                {
                    superchargerType.ChartPoints =
                        superchargerTypePoints.Where(p => p.ParentId == superchargerType.Id).ToList();
                }

            var compUnitTypes = new GetCompUnitTypeListQuery(context).Execute();

            var periodTypes = new GetPeriodTypesListQuery(context).Execute();

            var statuseTypes = new GetStatusTypeListQuery(context).Execute();

            var annuledReasons = new GetAnnuledReasonsListQuery(context).Execute();

            
            var eventTypes = new GetEventTypesListQuery(context).Execute();

            var sysEventTypes = new GetSysEventTypesListQuery(context).Execute();

            var sources = new GetSourcesListQuery(context).Execute();

            var regulatorTypes = new GetRegulatorTypeListQuery(context).Execute();

            var emergencyValveTypes = new GetEmergencyValveTypeListQuery(context).Execute();

            var parameterTypes = new GetParameterTypesListQuery(context).Execute();

            var repairExecutionMeans = new GetRepairExecutionMeansQuery(context).Execute();

            var enterprises = new GetEnterpriseListQuery(context).Execute(AppSettingsManager.CurrentEnterpriseId);

            var failureCauses = new GetCompUnitFailureCauseListQuery(context).Execute();

            var failureFeatures = new GetCompUnitFailureFeatureListQuery(context).Execute();

            var unitStopTypes = new GetCompUnitStopTypeListQuery(context).Execute();
            
            var unitRepairTypes = new GetCompUnitRepairTypeListQuery(context).Execute();

            var operConsumerTypes = new OperConsumerTypesQuery(context).Execute();

            var heaterTypes = new GetHeaterTypesListQuery(context).Execute();

            var boilerTypes = new GetBoilerTypeListQuery(context).Execute();

            var powerUnitTypes = new GetPowerUnitTypeListQuery(context).Execute();

            var coolingUnitTypes = new GetCoolingUnitTypeListQuery(context).Execute();

            var planTypeDTO = new GetPlanTypesListQuery(context).Execute();

            var gasTransportSystemDTO = new GetGasTransportSystemListQuery(context).Execute();

            var pipelineGroupDTO = new GetPipelineGroupListQuery(context).Execute();

            var diameters = new GetDiameterListQuery(context).Execute();

            var externalDiameters = new GetExternalDiameterListQuery(context).Execute();
            
            var inconsistencyTypes = new GetInconsistencyTypesQuery(context).Execute();

            var alarmTypes = new GetAlarmTypeListQuery(context).Execute();

            var compUnitSealingTypes = new GetCompUnitSealingTypeListQuery(context).Execute();

            var exchangeTypes = new GetExchangeTypeListQuery(context).Execute();

            var transportTypes = new GetTransportTypeListQuery(context).Execute();

            var targets = new GetTargetListQuery(context).Execute();


            var repairTypes = new GetRepairTypeListQuery(context).Execute();


            // Загрузка состояний
            var valveStates = new GetStateListQuery(context).Execute(StateSet.ValveStates);

            var compUnitStates = new GetStateListQuery(context).Execute(StateSet.CompUnitStates);

            var gasCostItemGroups = new GetGasCostItemGroupListQuery(context).Execute();


            // Загрузка иерархического справочника EntityType 
            // Очень важна последовательность
            var phisicalTypes = new GetPhysicalTypeListQuery(context).Execute();
            var propertyTypes = new GetPropertyTypeListQuery(context).Execute();

            var entityTypes = new GetEntityTypeListQuery(context).Execute();
            var etProps = new GetEntityTypePropertyListQuery(context).Execute(null);
            foreach (var et in entityTypes)
            {
                et.EntityProperties.AddRange(
                    etProps.Where(p => p.EntityType == et.EntityType)
                        .Select(p => propertyTypes.Single(pt => pt.PropertyType == p.PropertyType)));
            }

            Dictionaries = new DictionaryRepositoryDTO
            {
                EntityTypes = entityTypes,
                PipelineTypes = pipeLineTypes,
                ValvePurposes = valvePurposes,
                PipelineEndTypes = pipelineEndType,
                ConsumerTypes = consumerTypes,
                EngineClasses = engineClasses,
                BalanceSigns = balanceSigns,
                BalanceItems = balanceItems,
                ValveTypes = valveTypes,
                Regions = regions,
                SuperchargerTypes = superchargerTypes,
                CompUnitTypes = compUnitTypes,
                PeriodTypes = periodTypes,
                PhisicalTypes = phisicalTypes,
                StatuseTypes = statuseTypes,
                AnnuledReasons = annuledReasons,
                PropertyTypes = propertyTypes,
                EventTypes = eventTypes,
                Sources = sources,
                RegulatorTypes = regulatorTypes,
                EmergencyValveTypes = emergencyValveTypes,
                ParameterTypes = parameterTypes,
                SysEventTypes = sysEventTypes,
                Enterprises = enterprises,
                CompUnitFailureCauses = failureCauses,
                CompUnitFailureFeatures = failureFeatures,
                CompUnitStopTypes = unitStopTypes,
                CompUnitRepairTypes = unitRepairTypes,
                OperConsumerTypes = operConsumerTypes,
                HeaterTypes = heaterTypes,
                BoilerTypes = boilerTypes,
                PowerUnitTypes = powerUnitTypes,
                CoolingUnitTypes = coolingUnitTypes,
                PlanTypes = planTypeDTO,
                GasTransportSystems = gasTransportSystemDTO,
                PipelineGroups = pipelineGroupDTO,
                Diameters = diameters,
                ExternalDiameters = externalDiameters,
                RepairExecutionMeans = repairExecutionMeans,
                InconsistencyTypes = inconsistencyTypes,
                AlarmTypes = alarmTypes,
                ValveStates = valveStates,
                CompUnitStates = compUnitStates,
                CompUnitSealingTypes = compUnitSealingTypes,
                ExchangeTypes = exchangeTypes,
                TransportTypes = transportTypes,
                Targets = targets,
                RepairTypes = repairTypes,
                GasCostItemGroups = gasCostItemGroups
            };
        }
    }
}