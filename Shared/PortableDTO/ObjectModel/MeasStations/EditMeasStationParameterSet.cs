using System;
using GazRouter.DTO.Dictionaries.BalanceSigns;

namespace GazRouter.DTO.ObjectModel.MeasStations
{
	public class EditMeasStationParameterSet : EditEntityParameterSet
    {
        public Sign BalanceSignId { get; set; }
        public Guid? NeighbourEnterpriseId { get; set; }

        public bool IsIntermediate { get; set; }
    }
}
