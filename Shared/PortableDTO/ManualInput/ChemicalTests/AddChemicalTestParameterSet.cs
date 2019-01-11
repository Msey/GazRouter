using System;
using Utils.Calculations;

namespace GazRouter.DTO.ManualInput.ChemicalTests
{
    public class AddChemicalTestParameterSet
    {
        public Guid MeasPointId { get; set; }
        public DateTime TestDate { get; set; }
        public double? DewPoint { get; set; }
        public double? DewPointHydrocarbon { get; set; }
        public double? ContentNitrogen { get; set; }
        public double? ConcentrSourSulfur { get; set; }
        public double? ConcentrHydrogenSulfide { get; set; }
        public double? ContentCarbonDioxid { get; set; }
        public double? Density { get; set; }
        public double? CombHeatLow { get; set; }
        public double? CombHeatLowJoule
        {
            get { return CombHeatLow == null ? CombHeatLow : Math.Round((double)CombHeatLow * (PhysicalConstants.Cal * 0.001), 3); }
            set { CombHeatLow = value == null ? value : Math.Round((double)value * (1000 /PhysicalConstants.Cal), 3); }
        }

    }
}