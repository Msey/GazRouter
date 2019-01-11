using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.TransportTypes
{
	[DataContract]
	public class TransportTypeDTO : BaseDictionaryDTO
	{
		public TransportType TransportType
		{
            get { return (TransportType)Id; }
		}

	}
}