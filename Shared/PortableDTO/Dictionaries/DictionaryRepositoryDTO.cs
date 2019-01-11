using System.Collections.Generic;
using System.Runtime.Serialization;
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

namespace GazRouter.DTO.Dictionaries
{
    [DataContract]
    public class DictionaryRepositoryDTO
    {
        [DataMember]
        public List<EntityTypeDTO> EntityTypes { get; set; }

        [DataMember]
        public List<PipelineTypesDTO> PipelineTypes { get; set; }

        [DataMember]
        public List<ValvePurposeDTO> ValvePurposes { get; set; }

        [DataMember]
        public List<PipelineEndTypeDTO> PipelineEndTypes { get; set; }

        [DataMember]
        public List<ConsumerTypesDTO> ConsumerTypes { get; set; }

        [DataMember]
        public List<EngineClassDTO> EngineClasses { get; set; }

        [DataMember]
        public List<BalanceSignDTO> BalanceSigns { get; set; }

        [DataMember]
        public List<BalanceItemDTO> BalanceItems { get; set; }

        [DataMember]
        public List<ValveTypeDTO> ValveTypes { get; set; }

        [DataMember]
        public List<SuperchargerTypeDTO> SuperchargerTypes { get; set; }

        [DataMember]
        public List<CompUnitTypeDTO> CompUnitTypes { get; set; }

        [DataMember]
        public List<PeriodTypeDTO> PeriodTypes { get; set; }

        [DataMember]
        public List<PhysicalTypeDTO> PhisicalTypes { get; set; }

        [DataMember]
        public List<EventTypeDTO> EventTypes { get; set; }

        [DataMember]
        public List<EventPrioritiesDTO> EventPriorities { get; set; }

        [DataMember]
        public List<StatusTypeDTO> StatuseTypes { get; set; }

        [DataMember]
        public List<AnnuledReasonDTO> AnnuledReasons { get; set; }

        [DataMember]
        public List<SourceDTO> Sources { get; set; }
        
        [DataMember]
     
        public List<PropertyTypeDTO> PropertyTypes { get; set; }

        
        [DataMember]
        public List<RepairExecutionMeansDTO> RepairExecutionMeans { get; set; }

        [DataMember]
        public List<RegulatorTypeDTO> RegulatorTypes { get; set; }

        [DataMember]
        public List<ParameterTypeDTO> ParameterTypes { get; set; }
        
        [DataMember]
        public List<SysEventTypeDTO> SysEventTypes { get; set; }

        [DataMember]
        public List<EnterpriseDTO> Enterprises { get; set; }

        [DataMember]
        public List<RegionDTO> Regions { get; set; }

        [DataMember]
        public List<CompUnitFailureCauseDTO> CompUnitFailureCauses { get; set; }

        [DataMember]
        public List<CompUnitStopTypeDTO> CompUnitStopTypes { get; set; }

        [DataMember]
        public List<CompUnitRepairTypeDTO> CompUnitRepairTypes { get; set; }

        [DataMember]
        public List<CompUnitFailureFeatureDTO> CompUnitFailureFeatures { get; set; }

        [DataMember]
        public List<OperConsumerTypeDTO> OperConsumerTypes { get; set; }

        [DataMember]
        public List<HeaterTypeDTO> HeaterTypes { get; set; }

        [DataMember]
        public List<BoilerTypeDTO> BoilerTypes { get; set; }

        [DataMember]
        public List<PowerUnitTypeDTO> PowerUnitTypes { get; set; }

        [DataMember]
        public List<CoolingUnitTypeDTO> CoolingUnitTypes { get; set; }

        [DataMember]
        public List<EmergencyValveTypeDTO> EmergencyValveTypes { get; set; }

        [DataMember]
        public List<PlanTypeDTO> PlanTypes { get; set; }

		[DataMember]
		public List<GasTransportSystemDTO> GasTransportSystems { get; set; }

		[DataMember]
		public List<PipelineGroupDTO> PipelineGroups { get; set; }

        [DataMember]
        public List<DiameterDTO> Diameters { get; set; }

        [DataMember]
        public List<ExternalDiameterDTO> ExternalDiameters { get; set; }

        [DataMember]
		public List<InconsistencyTypeDTO> InconsistencyTypes { get; set; }

        [DataMember]
        public List<AlarmTypeDTO> AlarmTypes { get; set; }

        [DataMember]
        public List<StateBaseDTO> ValveStates { get; set; }

        [DataMember]
        public List<StateBaseDTO> CompUnitStates { get; set; }

        [DataMember]
        public List<CompUnitSealingTypeDTO> CompUnitSealingTypes { get; set; }

        [DataMember]
        public List<ExchangeTypeDTO> ExchangeTypes { get; set; }

        [DataMember]
        public List<TransportTypeDTO> TransportTypes { get; set; }

        [DataMember]
        public List<TargetDTO> Targets { get; set; }


        [DataMember]
        public List<RepairTypeDTO> RepairTypes { get; set; }


        [DataMember]
        public List<GasCostItemGroupDTO> GasCostItemGroups { get; set; }

    }
}