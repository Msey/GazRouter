using System;

namespace GazRouter.DTO.EventLog.EventRecipient
{
    public class MoveToTrashEventParameterSet
    {
        public int EventId { get; set; }
        public Guid SiteId { get; set; }
    }

	public class RestoreFromTrashEventParameterSet
	{
		public int EventId { get; set; }
		public Guid SiteId { get; set; }
	}
}