using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Repairs.RepairWorks;

namespace GazRouter.DTO.Repairs.Plan
{

	public class AddRepairParameterSet
    {
	    public AddRepairParameterSet()
	    {
            RepairWorks = new List<RepairWorkParameterSet>();
	    }

        public int RepairType { get; set; }
	    public ExecutionMeans ExecutionMeans { get; set; }
	    public int? ComplexId { get; set; }
		public Guid EntityId { get; set; }
		public double BleedAmount { get; set; }
        public double SavingAmount { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Description { get; set; }
		public string DescriptionGtp { get; set; }
		public string DescriptionCPDD { get; set; }
        public DateTime? DateStartShed { get; set; }
        public DateTime? DateEndShed { get; set; }
        public double MaxTransferWinter { get; set; }
		public double MaxTransferSummer { get; set; }
		public double MaxTransferTransition { get; set; }
		public double CapacityWinter { get; set; }
		public double CapacitySummer { get; set; }
		public double CapacityTransition { get; set; }
		public double CalculatedTransfer { get; set; }
		public bool IsCritical { get; set; }
        public PlanType? PlanType { get; set; }
	    public bool IsExternalCondition { get; set; }
        public DateTime PartsDeliveryDate { get; set; }


        public int Duration { get; set; }
        public string GazpromPlanID { get; set; }
        public DateTime? GazpromPlanDate { get; set; }
        public string ConsumersState { get; set; }

        public List<RepairWorkParameterSet> RepairWorks
	    {
	        get; set;
        }

        public int FireworkType { get; set; } = 4;
    }
}