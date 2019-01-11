using System;

namespace GazRouter.DTO.ObjectModel.Boilers
{
	public class AddBoilerParameterSet
    {
        public string Name { get; set; }
        public int? SortOrder { get; set; }
        public bool Hidden { get; set; }
        public bool IsVirtual { get; set; }
        public double Kilometer { get; set; }
		public int GasTransportSystemId { get; set; }
		public Guid? PipelineId { get; set; }
		public Guid? DistStationId { get; set; }
		public Guid? BoilerPlantId { get; set; }
		public Guid? MeasStationId { get; set; }
		public int BoilerTypeId { get; set; }
	    public double HeatLossFactor { get; set; }
        public double HeatSupplySystemLoad { get; set; }
    }
}
