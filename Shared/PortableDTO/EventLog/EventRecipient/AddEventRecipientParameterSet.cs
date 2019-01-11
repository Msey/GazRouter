using System;
using GazRouter.DTO.Dictionaries.EventPriorities;

namespace GazRouter.DTO.EventLog.EventRecipient
{
    public class AddEventRecipientParameterSet
    {
	    public int EventId { get; set; }
		public EventPriority Priority { get; set; }
		public Guid SiteId { get; set; }
    }
}