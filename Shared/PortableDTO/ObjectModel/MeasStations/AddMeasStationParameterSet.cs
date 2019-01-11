using System;
using GazRouter.DTO.Dictionaries.BalanceSigns;

namespace GazRouter.DTO.ObjectModel.MeasStations
{
	public class AddMeasStationParameterSet : AddEntityParameterSet
    {
        public Guid? Id { get; set; }
        public Sign BalanceSignId { get; set; }
        public Guid? NeighbourEnterpriseId { get; set; }
        public bool IsIntermediate { get; set; }
    }
}
