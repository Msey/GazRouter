using GazRouter.DTO.Repairs.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Repairs.Workflow
{
    public class ChangeRepairWfParametrSet
    {
        public int RepairId { get; set; }
        public WorkStateDTO.WorkflowStates WFState { get; set; }
        public WorkStateDTO.WorkStates WState { get; set; }
        public bool UpdateDate { get; set; }
        public RepairPlanBaseDTO repair { get; set; }
    }
}
