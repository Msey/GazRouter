using System;
using System.Collections.Generic;

namespace GazRouter.DTO.EventLog.EventRecipient
{
    public class EditRecepientsParameterSet
    {
        public List<Guid> RecepientsToDelete { get; set; }
        public List<AddEventRecipientParameterSet> RecepientsToAdd { get; set; }
    }
}