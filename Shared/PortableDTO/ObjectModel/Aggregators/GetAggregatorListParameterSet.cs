
using System;
using GazRouter.DTO.Dictionaries.AggregatorTypes;

namespace GazRouter.DTO.ObjectModel.Aggregators
{
    public class GetAggregatorListParameterSet
    {
        public AggregatorType? AggregatorType { get; set; }

        public int? RegionId { get; set; }

        public int? SystemId { get; set; }

        public Guid? SiteId { get; set; }
    }
}