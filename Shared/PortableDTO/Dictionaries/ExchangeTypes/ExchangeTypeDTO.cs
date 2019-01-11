using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Dictionaries.ExchangeTypes
{
	[DataContract]
	public class ExchangeTypeDTO : BaseDictionaryDTO
	{
        public ExchangeType ExchangeType
		{
            get { return (ExchangeType)Id; }
		}
	}
}