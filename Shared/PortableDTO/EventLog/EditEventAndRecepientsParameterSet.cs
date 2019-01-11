using GazRouter.DTO.EventLog.EventRecipient;

namespace GazRouter.DTO.EventLog
{
    public class EditEventAndRecepientsParameterSet
    {
        public EditEventParameterSet Event { get; set; }
        public EditRecepientsParameterSet Recepients { get; set; }
    }
}