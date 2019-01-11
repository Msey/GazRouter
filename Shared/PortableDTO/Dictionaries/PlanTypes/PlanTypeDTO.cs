using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.PlanTypes
{
	[DataContract]
    public class PlanTypeDTO : BaseDictionaryDTO
	{
        public PlanType PlanType
	    {
            get { return (PlanType)Id; }
	    }
	}
}