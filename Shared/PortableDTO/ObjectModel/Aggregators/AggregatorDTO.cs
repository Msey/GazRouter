using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.AggregatorTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.Aggregators
{
    [DataContract]
    public class AggregatorDTO : EntityDTO
	{
        [DataMember]
        public AggregatorType AggregatorType { get; set; }

        [DataMember]
        public string AggregatorTypeName { get; set; }

        public override EntityType EntityType => EntityType.Aggregator;

        public bool IsSystem => AggregatorType != AggregatorType.Custom;

	}
}