using System;

namespace GazRouter.DTO.ObjectModel.DistrStationOutlets
{
    public class AddDistrStationOutletParameterSet : AddEntityParameterSet
    {
        public double? CapacityRated { get; set; }

        public double? PressureRated { get; set; }

        public Guid? ConsumerId { get; set; }

        public Guid? Id { get; set; }
    }
}
