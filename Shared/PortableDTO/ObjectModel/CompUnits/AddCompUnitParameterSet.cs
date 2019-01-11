using System;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;

namespace GazRouter.DTO.ObjectModel.CompUnits
{
    public class AddCompUnitParameterSet : AddEntityParameterSet
    {
        public int CompUnitNum { get; set; }
        public int CompUnitTypeId { get; set; }
        public int SuperchargerTypeId { get; set; }

        public double InjectionProfileVolume { get; set; }
        public string InjectionProfilePiping { get; set; }
        public double TurbineStarterConsumption { get; set; }
        public double DryMotoringConsumption { get; set; }
        public double BleedingRate { get; set; }
        public int SealingCount { get; set; }
        public CompUnitSealingType SealingType { get; set; }
        public double StartValveConsumption { get; set; }
        public double StopValveConsumption { get; set; }
        public string ValveConsumptionDetails { get; set; }

        public bool HasRecoveryBoiler { get; set; }
        public Guid? Id { get; set; }
    }
}
