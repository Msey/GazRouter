using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.PipelineLimits;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.PipelineLimits
{
    public class AddPipelineLimitCommand : CommandScalar<AddPipelineLimitParameterSet, int>
    {
        public AddPipelineLimitCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddPipelineLimitParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("comp_unit_test_id");

            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.Begin);
            command.AddInputParameter("p_kilometer_end", parameters.End);
            command.AddInputParameter("p_p_max", parameters.MaxPressure);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", parameters.UserName);
        }

        protected override string GetCommandText(AddPipelineLimitParameterSet parameters)
        {
            return "P_PIPELINE_LIMIT.AddF";
        }
    }
}
