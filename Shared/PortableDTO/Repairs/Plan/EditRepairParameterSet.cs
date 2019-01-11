using System;

namespace GazRouter.DTO.Repairs.Plan
{

    public class EditRepairParameterSet : AddRepairParameterSet
    {
        public int Id { get; set; }

        public int? RepairState { get; set; }
        public int? WorkflowState { get; set; }

        public DateTime? DateStartFact { get; set; }
        public DateTime? DateEndFact { get; set; }

        public DateTime? ResolutionDate { get; set; }
        public DateTime? ResolutionDateCpdd { get; set; }
        public string ResolutionNum { get; set; }
        public string ResolutionNumCpdd { get; set; }
    }
}