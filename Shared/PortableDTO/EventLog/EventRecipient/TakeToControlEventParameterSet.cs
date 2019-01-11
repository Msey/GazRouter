using System;

namespace GazRouter.DTO.EventLog.EventRecipient
{
    public class TakeToControlEventParameterSet
	{
		public int EventId { get; set; }
        public Guid SiteId { get; set; }
	}

	public class BackToNormalEventParameterSet
	{
		public int EventId { get; set; }
		public Guid SiteId { get; set; }
	}
}
