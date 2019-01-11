using System;

namespace GazRouter.DTO.ObjectModel.DistrStationOutlets
{
    public class EditDistrStationOutletParameterSet : EditEntityParameterSet
    {
        public double? CapacityRated { get; set; }

        public double? PressureRated { get; set; }

        public Guid? ConsumerId { get; set; }
    }
}
