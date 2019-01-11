using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.Cache;
using GazRouter.DataProviders.Dictionaries;
using GazRouter.DTO.Dictionaries;
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

namespace GazRouter.Client.Cache
{
    public class DictionaryRepositoryCache : IDictionaryRepository
    {
        private DictionaryRepositoryDTO _dto;
        private Dictionary<PipelineType, PipelineTypesDTO> _pipelineTypesDict;

        /*    private async void Refresh()
        {
            Loaded = false;
            Load();
         
            Loaded = true;
            //	        ClientCache.DictionaryRepository = new DictionaryRepositoryCache(dicts);
        }*/

        public List<EntityTypeDTO> EntityTypes => Dto.EntityTypes;

        public Dictionary<PipelineType, PipelineTypesDTO> PipelineTypes
        {
            get
            {
                return _pipelineTypesDict ??
                       (_pipelineTypesDict = Dto.PipelineTypes.ToDictionary(c => c.PipelineType, v => v));
            }
        }

        public List<ValvePurposeDTO> ValvePurposes => Dto.ValvePurposes;

        public List<PipelineEndTypeDTO> PipelineEndTypes => Dto.PipelineEndTypes;

        public List<ConsumerTypesDTO> ConsumerTypes => Dto.ConsumerTypes;

        public List<EngineClassDTO> EngineClasses => Dto.EngineClasses;

        public List<BalanceSignDTO> BalanceSigns => Dto.BalanceSigns;

        public List<BalanceItemDTO> BalanceItems => Dto.BalanceItems;

        public List<ValveTypeDTO> ValveTypes => Dto.ValveTypes;

        public List<SuperchargerTypeDTO> SuperchargerTypes => Dto.SuperchargerTypes;

        public List<CompUnitTypeDTO> CompUnitTypes => Dto.CompUnitTypes;

        public List<PeriodTypeDTO> PeriodTypes => Dto.PeriodTypes;

        public List<PhysicalTypeDTO> PhisicalTypes => Dto.PhisicalTypes;

        public List<EventTypeDTO> EventTypes => Dto.EventTypes;

        public List<EventPrioritiesDTO> EventPriorities => Dto.EventPriorities;

        public List<StatusTypeDTO> TaskStatusTypes => Dto.StatuseTypes;

        public List<AnnuledReasonDTO> AnnuledReasons => Dto.AnnuledReasons;

        public List<SourceDTO> Sources => Dto.Sources;

        public List<PropertyTypeDTO> PropertyTypes => Dto.PropertyTypes;

        public List<RepairExecutionMeansDTO> RepairExecutionMeans => Dto.RepairExecutionMeans;

        public List<RegulatorTypeDTO> RegulatorTypes => Dto.RegulatorTypes;

        public List<ParameterTypeDTO> ParameterTypes => Dto.ParameterTypes;

        public List<SysEventTypeDTO> SysEventTypes => Dto.SysEventTypes;

        public List<EnterpriseDTO> Enterprises => Dto.Enterprises;

        public List<RegionDTO> Regions => Dto.Regions;

        public List<CompUnitFailureCauseDTO> CompUnitFailureCauses => Dto.CompUnitFailureCauses;

        public List<CompUnitStopTypeDTO> CompUnitStopTypes => Dto.CompUnitStopTypes;

        public List<CompUnitRepairTypeDTO> CompUnitRepairTypes => Dto.CompUnitRepairTypes;

        public List<CompUnitFailureFeatureDTO> CompUnitFailureFeatures => Dto.CompUnitFailureFeatures;

        public List<OperConsumerTypeDTO> OperConsumerTypes => Dto.OperConsumerTypes;

        public List<HeaterTypeDTO> HeaterTypes => Dto.HeaterTypes;

        public List<BoilerTypeDTO> BoilerTypes => Dto.BoilerTypes;

        public List<PowerUnitTypeDTO> PowerUnitTypes => Dto.PowerUnitTypes;

        public List<CoolingUnitTypeDTO> CoolingUnitTypes => Dto.CoolingUnitTypes;

        public List<EmergencyValveTypeDTO> EmergencyValveTypes => Dto.EmergencyValveTypes;

        public List<PlanTypeDTO> PlanTypes => Dto.PlanTypes;

        public List<GasTransportSystemDTO> GasTransportSystems => Dto.GasTransportSystems;

        public List<PipelineGroupDTO> PipelineGroups => Dto.PipelineGroups;

        public List<DiameterDTO> Diameters => Dto.Diameters;

        public List<ExternalDiameterDTO> ExternalDiameters => Dto.ExternalDiameters;

        public List<InconsistencyTypeDTO> InconsistencyTypes => Dto.InconsistencyTypes;

        public List<AlarmTypeDTO> AlarmTypes => Dto.AlarmTypes;

        public List<StateBaseDTO> ValveStates => Dto.ValveStates;

        public List<StateBaseDTO> CompUnitStates => Dto.CompUnitStates;

        public List<CompUnitSealingTypeDTO> CompUnitSealingTypes => Dto.CompUnitSealingTypes;

        public List<ExchangeTypeDTO> ExchangeTypes => Dto.ExchangeTypes;

        public List<TransportTypeDTO> TransportTypes => Dto.TransportTypes;

        public List<TargetDTO> Targets => Dto.Targets;

        public bool Loaded { get; private set; }

        public List<RepairTypeDTO> RepairTypes => Dto.RepairTypes;

        public List<GasCostItemGroupDTO> GasCostItemGroups => Dto.GasCostItemGroups; 

        public DictionaryRepositoryDTO Dto
        {
            get
            {
                if (_dto == null)
                {
                    throw new Exception("Словари еще не загружены");
                }
                return _dto;
            }
            set { _dto = value; }
        }

        public async Task Load(bool force = false)
        {
            _dto = await new DictionaryServiceProxy().GetDictionaryRepositoryAsync(force).ConfigureAwait(true);
        }

        public List<StateBaseDTO> GetStateSet(StateSet set)
        {
            switch (set)
            {
                case StateSet.ValveStates:
                    return ValveStates;

                case StateSet.CompUnitStates:
                    return CompUnitStates;

                default:
                    throw new Exception("Справочник не найден");
            }
        }

        public StateBaseDTO GetState(StateSet set, double val)
        {
            var stateList = GetStateSet(set);
            return stateList.SingleOrDefault(s => s.Id == val);
        }
    }
}