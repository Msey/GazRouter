
using GazRouter.DTO.Dictionaries.AggregatorTypes;

namespace GazRouter.DTO.ObjectModel.Aggregators
{
    public class AddAggregatorParameterSet : AddEntityParameterSet
    {
        public AggregatorType AggregatorType { get; set; }
    }
}