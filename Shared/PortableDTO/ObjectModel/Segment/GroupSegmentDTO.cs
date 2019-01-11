using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.Segment
{
    [DataContract]
	public class GroupSegmentDTO : BaseSegmentDTO
	{
		[DataMember]
        public Guid PipelineGroupId { get; set; }

        [DataMember]
        public string PipelineGroupName { get; set; }
        
	}
}
