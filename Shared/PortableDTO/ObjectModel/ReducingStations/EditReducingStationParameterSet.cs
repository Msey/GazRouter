using System;

namespace GazRouter.DTO.ObjectModel.ReducingStations
{
    public class EditReducingStationParameterSet : AddReducingStationParameterSet
    {
        public Guid ReducingStationId { get; set; }
    }
}
