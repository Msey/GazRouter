using System.Collections.Generic;
using System.Threading.Tasks;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.Dictionaries.AnnuledReasons;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.CompUnitTypes;
using GazRouter.DTO.Dictionaries.ConsumerTypes;
using GazRouter.DTO.Dictionaries.CoolingUnitTypes;
using GazRouter.DTO.Dictionaries.Diameters;
using GazRouter.DTO.Dictionaries.EmergencyValveTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.Dictionaries.EventTypes;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.GasCostItemGroups;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.HeaterTypes;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.Dictionaries.OperConsumerType;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.Dictionaries.PipelineGroups;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.Dictionaries.RegulatorTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Dictionaries.RepairTypes;
using GazRouter.DTO.Dictionaries.Sources;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using GazRouter.DTO.Dictionaries.SysEventTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.Dictionaries.ValveTypes;

namespace GazRouter.Common.Cache
{
    public interface IDictionaryRepository
    {
        List<EntityTypeDTO> EntityTypes { get; }
        Dictionary<PipelineType,PipelineTypesDTO> PipelineTypes { get; }
        List<ValvePurposeDTO> ValvePurposes { get; }
        List<PipelineEndTypeDTO> PipelineEndTypes { get; }
        List<ConsumerTypesDTO> ConsumerTypes { get; }
        List<EngineClassDTO> EngineClasses { get; }
        List<BalanceSignDTO> BalanceSigns { get; }
        List<BalanceItemDTO> BalanceItems { get; }
        List<ValveTypeDTO> ValveTypes { get; }
        List<SuperchargerTypeDTO> SuperchargerTypes { get; }
        List<CompUnitTypeDTO> CompUnitTypes { get; }
        List<PeriodTypeDTO> PeriodTypes { get; }
        List<PhysicalTypeDTO> PhisicalTypes { get; }
        List<EventTypeDTO> EventTypes { get; }
        List<EventPrioritiesDTO> EventPriorities { get; }
        List<StatusTypeDTO> TaskStatusTypes { get; }
        List<AnnuledReasonDTO> AnnuledReasons { get; }
        List<SourceDTO> Sources { get; }
        List<PropertyTypeDTO> PropertyTypes { get; }
        List<RepairTypeDTO> RepairTypes { get; }
        List<RepairExecutionMeansDTO> RepairExecutionMeans { get; }
        List<RegulatorTypeDTO> RegulatorTypes { get; }
        List<ParameterTypeDTO> ParameterTypes { get; }
        List<SysEventTypeDTO> SysEventTypes { get; }
        List<EnterpriseDTO> Enterprises { get; }
        List<RegionDTO> Regions { get; }
        List<CompUnitFailureCauseDTO> CompUnitFailureCauses { get; }
        List<CompUnitStopTypeDTO> CompUnitStopTypes { get; }
        List<CompUnitRepairTypeDTO> CompUnitRepairTypes { get; }
        List<CompUnitFailureFeatureDTO> CompUnitFailureFeatures { get; }
        List<OperConsumerTypeDTO> OperConsumerTypes { get; }
        List<HeaterTypeDTO> HeaterTypes { get; }
        List<BoilerTypeDTO> BoilerTypes { get; }
        List<PowerUnitTypeDTO> PowerUnitTypes { get; }
        List<CoolingUnitTypeDTO> CoolingUnitTypes { get; }
        List<EmergencyValveTypeDTO> EmergencyValveTypes { get; }
        List<PlanTypeDTO> PlanTypes { get; }
        List<GasTransportSystemDTO> GasTransportSystems { get; }
        List<PipelineGroupDTO> PipelineGroups { get; }
        List<DiameterDTO> Diameters { get; }
        List<ExternalDiameterDTO> ExternalDiameters { get; }
        List<InconsistencyTypeDTO> InconsistencyTypes { get; }
        List<AlarmTypeDTO> AlarmTypes { get; }
        List<StateBaseDTO> ValveStates { get; }
        List<StateBaseDTO> CompUnitStates { get; }
        List<CompUnitSealingTypeDTO> CompUnitSealingTypes { get; }
        List<ExchangeTypeDTO> ExchangeTypes { get; }
        List<TransportTypeDTO> TransportTypes { get; }
        List<TargetDTO> Targets { get; }

        List<GasCostItemGroupDTO> GasCostItemGroups { get; }

        bool Loaded { get;  }

        List<StateBaseDTO> GetStateSet(StateSet set);
        StateBaseDTO GetState(StateSet set, double val);

        Task Load(bool force = false);
    }
}