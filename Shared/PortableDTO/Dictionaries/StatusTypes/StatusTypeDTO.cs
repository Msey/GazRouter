using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Dictionaries.StatusTypes
{
	[DataContract]
	public class StatusTypeDTO : BaseDictionaryDTO
	{
	    public StatusTypeDTO()
	    {
	        AllowedStatusList = new List<StatusType>();
	    }


        [DataMember]
		public string Code { get; set; }

		public StatusType StatusType => (StatusType)Id;


        [DataMember]
        public List<StatusType> AllowedStatusList { get; set; }

    }
}