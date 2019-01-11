using System;

namespace GazRouter.DTO.ObjectModel.MeasPoint
{
	public class AddMeasPointParameterSet : AddEntityParameterSet
	{
		public Guid? MeasLineId { get; set; }
		public Guid? CompShopId { get; set; }
		public Guid? DistrStationId { get; set; }

	    public double ChromatographConsumptionRate { get; set; }
        public int ChromatographTestTime { get; set; }
	}
}
