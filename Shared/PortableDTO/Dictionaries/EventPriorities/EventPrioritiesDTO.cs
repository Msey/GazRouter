using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.EventPriorities
{
	[DataContract]
	public class EventPrioritiesDTO : BaseDictionaryDTO
	{

		[DataMember]
		public string Description { get; set; }

		public EventPriority EventPriorities
		{
			get { return (EventPriority)Id; }
		}

	}
}