using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.CompUnitSealingTypes
{
	[DataContract]
	public class CompUnitSealingTypeDTO : BaseDictionaryDTO
	{
        public CompUnitSealingType SealingType
        {
            get { return (CompUnitSealingType)Id; }
        }
	}
}