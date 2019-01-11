using System;
using GazRouter.DTO.Dictionaries.AggregatorTypes;

namespace GazRouter.DTO.ObjectModel.Aggregators
{
    public class EditAggregatorParameterSet : EditEntityParameterSet
    {
        public AggregatorType AggregatorType { get; set; }
    }
}
