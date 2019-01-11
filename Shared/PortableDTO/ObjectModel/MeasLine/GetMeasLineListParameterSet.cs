using System;
using System.Collections.Generic;

namespace GazRouter.DTO.ObjectModel.MeasLine
{
    public class GetMeasLineListParameterSet
    {
        public Guid? MeasStationId { get; set; }
        public int? SystemId { get; set; }
        public List<Guid> MeasStationList { get; set; }
    }
}
