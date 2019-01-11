using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.PipelineTypes
{
	[DataContract]
	public class PipelineTypesDTO : BaseDictionaryDTO
	{
        [DataMember]
        public string Description { get; set; }

        [DataMember]
		public int SortOrder { get; set; }

		public PipelineType PipelineType
	    {
			get { return (PipelineType)Id; }
	    }
        
	}
}