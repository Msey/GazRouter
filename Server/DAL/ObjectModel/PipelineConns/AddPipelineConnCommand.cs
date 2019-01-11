using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.PipelineConns;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.PipelineConns
{
    public class AddPipelineConnCommand : CommandScalar<AddPipelineConnParameterSet, int>
	{
		public AddPipelineConnCommand(ExecutionContext context) : base(context)
		{
            IsStoredProcedure = true;
		}
		
	    protected override void BindParameters(OracleCommand command, AddPipelineConnParameterSet parameters)
	    {
            OutputParameter = command.AddReturnParameter<int>("p_entity_id");
            command.AddInputParameter("p_end_type_id", parameters.EndTypeId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_dest_entity_id", parameters.DestEntityId);
            command.AddInputParameter("p_kilometer", parameters.Kilometr);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }

        protected override string GetCommandText(AddPipelineConnParameterSet parameters)
	    {
            return "rd.P_PIPELINE_CONNS.AddF";
		}
	}
}

