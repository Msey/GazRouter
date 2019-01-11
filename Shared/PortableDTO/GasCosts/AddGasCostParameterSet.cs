using System;
using GazRouter.DTO.Dictionaries.Targets;
namespace GazRouter.DTO.GasCosts
{
    public class AddGasCostParameterSet
    {
        public CostType CostType { get; set; }
        
        public Guid EntityId { get; set; }

        public DateTime Date { get; set; }

        public double? CalculatedVolume { get; set; }

        public double? MeasuredVolume { get; set; }

        public string InputData  { get; set; }
        public Target Target { get; set; }
        public Guid SiteId { get; set; }

        public int? ImportId { get; set; }

        public int SeriesId { get; set; } = -1;
    }
}