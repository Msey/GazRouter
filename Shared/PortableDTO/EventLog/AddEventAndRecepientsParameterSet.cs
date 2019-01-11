using System.Collections.Generic;
using GazRouter.DTO.EventLog.EventRecipient;

namespace GazRouter.DTO.EventLog
{
    public class AddEventAndRecepientsParameterSet
    {
        public RegisterEventParameterSet Event { get; set; }
        public List<AddEventRecipientParameterSet> Recepients { get; set; }
    }
}