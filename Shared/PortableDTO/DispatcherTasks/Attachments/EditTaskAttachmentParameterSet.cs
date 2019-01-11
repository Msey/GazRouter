using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.DispatcherTasks.Attachments
{
    public class EditTaskAttachmentParameterSet
    {
        public Guid AttachmentId { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
    }
}
