using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.PipelineEndType
{
	[DataContract]
	public class PipelineEndTypeDTO : BaseDictionaryDTO
	{
        [DataMember]
        public string Description { get; set; }

        [DataMember]
		public int SortOrder { get; set; }

		public PipelineEndType PipelineEndType
	    {
			get { return (PipelineEndType)Id; }
	    }
        
	}
}