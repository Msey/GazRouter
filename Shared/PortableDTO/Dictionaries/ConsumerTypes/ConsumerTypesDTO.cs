using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.ConsumerTypes
{
	[DataContract]
	public class ConsumerTypesDTO : BaseDictionaryDTO
	{
		public ConsumerType ConsumerTypes
	    {
			get { return (ConsumerType)Id; }
	    }
        
	}
}