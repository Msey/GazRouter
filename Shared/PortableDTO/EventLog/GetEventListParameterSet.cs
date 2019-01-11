using System;

namespace GazRouter.DTO.EventLog
{
	public class GetEventListParameterSet
	{
	    public Guid? EntityId { get; set; }

	    public Guid SiteId { get; set; }

        public EventListType QueryType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int ArchivingDelay { get; set; }
	}
}
