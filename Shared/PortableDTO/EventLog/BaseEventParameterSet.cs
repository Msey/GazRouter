using System;

namespace GazRouter.DTO.EventLog
{
	public class BaseEventParameterSet
	{
		public DateTime EventDate { get; set; }
		public string Text { get; set; }
		public Guid? EntityId { get; set; }
        public double? Kilometer { get; set; }
        public int TypeId { get; set; }
    }
}
