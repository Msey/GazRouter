
using System;

namespace GazRouter.DTO.EventLog.Attachments
{
    public class EditEventAttachmentParameterSet : AddEventAttachmentParameterSet
    {
        public Guid EventAttachmentId { get; set; }
    }
}
