using System;
using System.Runtime.Serialization;
using GazRouter.DTO.ManualInput.InputStates;

namespace GazRouter.DTO.Balances.InputStates
{
    [DataContract]
    public class InputStateDTO
    {
        public InputStateDTO()
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