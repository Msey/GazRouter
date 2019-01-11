using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.StatesModel
{
	[DataContract]
    public class StateSetDTO<TStateDto> : StateSetBaseDTO
        where TStateDto : StateBaseDTO
	{
        public StateSetDTO()
        {
            StateList = new List<TStateDto>();
        }

		[DataMember]
        public List<TStateDto> StateList { get; set; }
	}
}