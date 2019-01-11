using System;

namespace GazRouter.DTO.EventLog.EventRecipient
{
	public class AckEventParameterSet
	{
		public int EventId { get; set; }
		public Guid SiteId { get; set; }
	}
}
