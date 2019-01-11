using System;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.DTO.ObjectModel.Pipelines
{
    public class AddPipelineParameterSet
    {
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public bool Hidden { get; set; }
        public bool IsVirtual { get; set; }
        public PipelineType PipelineTypeId { get; set; }
        public double KilometerOfStart { get; set; }
        public double KilometerOfEnd{ get; set; }

		public int GasTransportSystemId { get; set; } 
    }
}
