using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Repairs.Workflow
{
    public class GetRepairWorkflowsParameterSet
    {
        public int Year { get; set; }
        public int SystemId { get; set; }
        public Guid? SiteId { get; set; }
        public int? UserId { get; set; }
        public WorkStateDTO.Stages Stage { get; set; }
    }
}
