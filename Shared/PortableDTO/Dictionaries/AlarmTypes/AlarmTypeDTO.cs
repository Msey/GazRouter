using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.AlarmTypes
{
	[DataContract]
	public class AlarmTypeDTO : BaseDictionaryDTO
	{

		public AlarmType AlarmType
		{
			get { return (AlarmType)Id; }
		}
        
        [DataMember]
        public string Description { get; set; }

	}
}