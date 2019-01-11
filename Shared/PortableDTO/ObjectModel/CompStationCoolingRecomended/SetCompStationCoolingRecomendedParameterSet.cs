using System;

namespace GazRouter.DTO.ObjectModel.CompStationCoolingRecomended
{
	public class SetCompStationCoolingRecomendedParameterSet
    {
		public Guid CompStationId { get; set; }
		public int Month { get; set; }
		public double? Temperature { get; set; }
    }
    
}
