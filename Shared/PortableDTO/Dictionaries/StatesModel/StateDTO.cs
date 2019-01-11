using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.StatesModel
{
	[DataContract]
    public class StateDTO<TEnum> : StateBaseDTO where TEnum : struct
	{
        public TEnum State
	    {
            get { return (TEnum)(object)Id; }
	    }
	}
}