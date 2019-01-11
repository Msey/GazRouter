using System;

namespace GazRouter.DTO.ObjectModel.MeasPoint
{
	public class GetMeasPointListParameterSet
	{
		public Guid? Id { get; set; }

		public Guid? ParentId { get; set; }

	    public int? SystemId { get; set; }
	}
}
