using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.CompUnits
{
    public class CompUnitDTO : EntityDTO
	{
        public override EntityType EntityType => EntityType.CompUnit;

        [DataMember]
        public int CompUnitNum { get; set; }

        [DataMember]
        public Guid CompStationId { get; set; }

        [DataMember]
        public EngineClass EngineClass { get; set; }

        [DataMember]
        public EntityStatus? Status { get; set; }
        
        [DataMember]
        public int CompUnitTypeId { get; set; }

        [DataMember]
        public int SuperchargerTypeId { get; set; }

        [DataMember]
        public bool HasRecoveryBoiler { get; set; }          

        
        [DataMember]
        public double TurbineStarterConsumption { get; set; }
        
        [DataMember]
        public double DryMotoringConsumption { get; set; }          
        
        [DataMember]
        public double InjectionProfileVolume { get; set; }

        [DataMember]
        public string InjectionProfilePiping { get; set; }
        
        [DataMember]
        public double BleedingRate { get; set; }

        [DataMember]
        public int SealingCount { get; set; }


        [DataMember]
        public double StartValveConsumption { get; set; }

        [DataMember]
        public double StopValveConsumption { get; set; }

        [DataMember]
        public string ValveConsumptionDetails { get; set; }


        [DataMember]
        public CompUnitSealingType? SealingType { get; set; }
	}
}