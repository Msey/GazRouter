using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Authorization.Action
{
    [DataContract]
    public class ActionGroupDTO
    {
        public ActionGroupDTO()
        {
            Actions = new List<ActionDTO>();
        }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public List<ActionDTO> Actions { get; set; }
    }

}