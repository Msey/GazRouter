using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.Sources
{
	[DataContract]
    public class SourceDTO : BaseDictionaryDTO
	{
		[DataMember]
		public string SysName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsReadonly { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }
	}
}