using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Utils.Calculations;

namespace GazRouter.DTO.ManualInput.ChemicalTests
{
    [DataContract]
    public class ChemicalTestDTO
    {
        [DataMember]
        public int? ChemicalTestId { get; set; }

        [DataMember]
        public Guid MeasPointId { get; set; }
        
        [DataMember]
        public DateTime? TestDate { get; set; }

        [DataMember]
        public double? DewPoint { get; set; }

        [DataMember]
        public double? DewPointHydrocarbon { get; set; }

        [DataMember]
        public double? ContentNitrogen { get; set; }

        [DataMember]
        public double? ConcentrSourSulfur { get; set; }

        [DataMember]
        public double? ConcentrHydrogenSulfide { get; set; }

        [DataMember]
        public double? ContentCarbonDioxid { get; set; }

        [DataMember]
        public double? Density { get; set; }

        [DataMember]
        public double? CombHeatLow { get; set; }

        [DataMember]
        public double? CombHeatLowJoule
        {
            get { return CombHeatLow == null ? CombHeatLow : Math.Round((double)CombHeatLow * (PhysicalConstants.Cal * 0.001), 3); }
            set { CombHeatLow = value == null ? value : Math.Round((double)value * (1000 / PhysicalConstants.Cal ), 3); }
        }


        [DataMember]
        public Guid ParentId { get; set; }

        [DataMember]
        public string ParentName { get; set; }

        [DataMember]
        public string ParentShortPath { get; set; }

        [DataMember]
        public EntityType ParentEntityType { get; set; }

        [DataMember]
        public bool IsFrigid { get; set; }

    }
}