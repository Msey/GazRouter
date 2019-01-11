using System;

namespace GazRouter.DTO.Repairs.Plan
{

	public class EditRepairDatesParameterSet
    {
        public int RepairId { get; set; }
		public DateTime DateStart { get; set; }
		public DateTime DateEnd { get; set; }
	    public DateTypes DateType { get; set; }
    }


}