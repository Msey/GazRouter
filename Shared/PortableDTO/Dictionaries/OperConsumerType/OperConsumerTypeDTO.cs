using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.OperConsumerType
{
	[DataContract]
    public class OperConsumerTypeDTO : BaseDictionaryDTO
	{
        public OperConsumerType ConsumerTypes
	    {
            get { return (OperConsumerType)Id; }
	    }
        
	}
}