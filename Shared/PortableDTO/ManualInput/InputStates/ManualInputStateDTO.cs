using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ManualInput.InputStates
{
    [DataContract]
    public class ManualInputStateDTO
    {
        public ManualInputStateDTO()
        {
            State = ManualInputState.Input;
        }

        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public ManualInputState State { get; set; }
        
        [DataMember]
        public DateTime? ChangeDate { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}