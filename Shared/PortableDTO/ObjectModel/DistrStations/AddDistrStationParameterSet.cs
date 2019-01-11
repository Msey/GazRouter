using System;

namespace GazRouter.DTO.ObjectModel.DistrStations
{
    public class AddDistrStationParameterSet : AddEntityParameterSet
    {
        public int RegionId { get; set; }

        public double? PressureRated { get; set; }
        public double? CapacityRated { get; set; }
        public bool IsForeign { get; set; }
        public Guid? Id { get; set; }
    }
}