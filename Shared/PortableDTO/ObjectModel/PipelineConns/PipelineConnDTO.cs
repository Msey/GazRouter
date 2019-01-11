using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PipelineEndType;

namespace GazRouter.DTO.ObjectModel.PipelineConns
{
    [DataContract]
    public class PipelineConnDTO : NamedDto<int>
    {
        [DataMember]
        public double? Kilometr { get; set; }

        [DataMember]
        public PipelineEndType EndTypeId { get; set; }

        [DataMember]
        public Guid PipelineId { get; set; }

        [DataMember]
        public Guid? DestPipelineId { get; set; }

        [DataMember]
		public Guid? DistrStationId { get; set; }

        [DataMember]
        public Guid? CompShopId { get; set; }

        [DataMember]
        public string EndTypeName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public CommonEntityDTO Entity { get; set; }
    }
}
