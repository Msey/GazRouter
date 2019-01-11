using System;

namespace GazRouter.DTO.DispatcherTasks.Attachments
{
    public class AddTaskAttachmentParameterSet
    {
        public Guid TaskId { get; set; }
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }

        public string UserName { get; set; }
    }
	
}
