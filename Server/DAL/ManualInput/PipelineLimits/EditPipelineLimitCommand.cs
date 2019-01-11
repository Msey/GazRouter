using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitTests;
using GazRouter.DTO.ManualInput.PipelineLimits;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ManualInput.PipelineLimits
{
    public class EditPipelineLimitCommand : CommandNonQuery<EditPipelineLimitParameterSet>
    {
        public EditPipelineLimitCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditPipelineLimitParameterSet parameters)
        {
            command.AddInputParameter("p_limit_id", parameters.EntityId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.Begin);
            command.AddInputParameter("p_kilometer_end", parameters.End);
            command.AddInputParameter("p_p_max", parameters.MaxPressure);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditPipelineLimitParameterSet parameters)
        {
            return "P_PIPELINE_LIMIT.Edit";
        }
    }
}
