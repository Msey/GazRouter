using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.OperConsumerType;

namespace GazRouter.DTO.ObjectModel.OperConsumers
{
    [DataContract]
    public class OperConsumerDTO : EntityDTO
    {
        [DataMember]
        public OperConsumerType OperConsumerTypeId { get; set; }
        
       
        [DataMember]
        public bool IsDirectConnection { get; set; }

        [DataMember]
        public string OperConsumerTypeName { get; set; }

		[DataMember]
		public int RegionId { get; set; }

        [DataMember]
		public string RegionName { get; set; }


        [DataMember]
        public Guid? DistrStationId { get; set; }

        [DataMember]
        public string DistrStationName { get; set; }


        public override EntityType EntityType => EntityType.OperConsumer;
    }
}