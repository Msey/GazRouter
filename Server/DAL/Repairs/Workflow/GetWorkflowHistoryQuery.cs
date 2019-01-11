using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Workflow
{
    public class GetWorkflowHistoryQuery : QueryReader<int, List<WorkflowHistoryDTO>>
    {
        public GetWorkflowHistoryQuery(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = false;
        }

        protected override string GetCommandText(int parameters)
        {
            return @"
select 
  h.repair_workflow_history_id, 
  h.repair_id, 
  h.workflow_state, 
  h.repair_state, 
  h.change_date, 
  h.change_user_id,
  u.name,
  u.description,
  u.site_id,
  (case when u.site_level = 0 then 
        (select enterprise_name  from v_sites where v_sites.enterprise_id = u.site_id and rownum = 1)
        else 
        (select site_name  from v_sites where v_sites.site_id = u.site_id  and rownum = 1)
   end) as site_name
 
from 
    V_REPAIR_WORKFLOW_HISTORY h
    left join v_users u on h.change_user_id = u.user_id
where  
    h.repair_id = :repairid
order by change_date";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("repairid", parameters);
        }

        protected override List<WorkflowHistoryDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var repairList = new List<WorkflowHistoryDTO>();
            while (reader.Read())
            {
                repairList.Add(
                    new WorkflowHistoryDTO
                    {
                        
                        ID = reader.GetValue<int>("repair_workflow_history_id"),
                        RepairID = reader.GetValue<int>("repair_id"),
                        WFStateCode = reader.GetValue<int>("workflow_state"),
                        WFState = WorkStateDTO.GetState((WorkStateDTO.WorkflowStates)reader.GetValue<int>("workflow_state")),
                        WStateCode = reader.GetValue<int>("repair_state"),
                        WState = WorkStateDTO.GetState((WorkStateDTO.WorkStates)reader.GetValue<int>("repair_state")),
                        EventDate = reader.GetValue<DateTime>("change_date"),
                        UserID = reader.GetValue<int>("change_user_id"),
                        UserName = reader.GetValue<string>("name"),
                        UserDescription = reader.GetValue<string>("description"),
                        UserSiteID = reader.GetValue<Guid>("site_id"),
                        SiteName = reader.GetValue<string>("site_name"),
                    });
            }
            return repairList;
        }
    }
}
