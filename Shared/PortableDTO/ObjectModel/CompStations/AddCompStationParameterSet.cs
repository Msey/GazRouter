
using System;

namespace GazRouter.DTO.ObjectModel.CompStations
{
    public class AddCompStationParameterSet : AddEntityParameterSet
    {
        public int RegionId { get; set; }
        public Guid? Id { get; set; }
    }
}