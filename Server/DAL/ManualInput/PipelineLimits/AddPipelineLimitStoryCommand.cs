using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.PipelineLimits;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ManualInput.PipelineLimits
{
    public class AddPipelineLimitStoryCommand : CommandNonQuery<AddPipelineLimitStoryParameterSet>
    {
        public AddPipelineLimitStoryCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddPipelineLimitStoryParameterSet parameters)
        {
            command.AddInputParameter("p_limit_id", parameters.EntityId);
            command.AddInputParameter("p_change_date", parameters.ChangeDate);
            command.AddInputParameter("p_status", parameters.Status);
            command.AddInputParameter("p_user_name", parameters.UserName);
        }

        protected override string GetCommandText(AddPipelineLimitStoryParameterSet parameters)
        {
            return "rd.P_PIPELINE_LIMIT_STORY.Add";
        }
    }
}
