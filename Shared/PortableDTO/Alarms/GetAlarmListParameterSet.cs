using System;

namespace GazRouter.DTO.Alarms
{
	public class GetAlarmListParameterSet
	{
		public Guid SiteId { get; set; }
	    public int? UserId { get; set; }
	    public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
	}
}
