using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.EngineClasses
{
	[DataContract]
	public class EngineClassDTO : BaseDictionaryDTO
	{
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int SortOrder { get; set; }

		public EngineClass EngineClass
		{
			get { return (EngineClass)Id; }
		}

	}
}