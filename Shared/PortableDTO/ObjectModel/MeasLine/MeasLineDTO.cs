using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.MeasLine
{
    [DataContract]
    public class MeasLineDTO : EntityDTO
    {
        public override EntityType EntityType
        {
            get { return EntityType.MeasLine;}
        }

		[DataMember]
		public Guid PipelineId { get; set; }

		[DataMember]
		public double KmOfConn { get; set; }
		
        [DataMember]
		public string PipelineName { get; set; }

        [DataMember]
        public EntityStatus? Status { get; set; }
    }
}
