using System;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.SeriesData.ValueMessages
{
    public class PerformCheckingParameterSet
    {
        public Guid? EntityId { get; set; }
        public int SerieId { get; set; }
        public bool SaveHistory { get; set; }
    }
}
