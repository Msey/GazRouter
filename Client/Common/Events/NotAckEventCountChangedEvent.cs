using GazRouter.DTO.EventLog.EventRecipient;
using Microsoft.Practices.Prism.Events;

namespace GazRouter.Common.Events
{
    public sealed class NotAckEventCountChangedEvent : CompositePresentationEvent<NonAckEventCountDTO>
    {
         
    }
}