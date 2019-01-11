using System;

namespace GazRouter.DTO.ObjectModel.PowerUnits
{
	public class AddPowerUnitParameterSet
    {
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public bool Hidden { get; set; }
        public bool IsVirtual { get; set; }
        public double Kilometer { get; set; }
		public int GasTransportSystemId { get; set; }
		public Guid? PipelineId { get; set; }
		public Guid? PowerPlantId { get; set; }
		public int PowerUnitTypeId { get; set; }
        public double OperatingTimeFactor { get; set; }
        public double TurbineConsumption { get; set; }
        public int TurbineRuntime { get; set; }
    }
}
