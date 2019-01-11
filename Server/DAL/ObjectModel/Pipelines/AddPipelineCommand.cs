using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Pipelines;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Pipelines
{
    public class AddPipelineCommand : CommandScalar<AddPipelineParameterSet, Guid>
	{
		public AddPipelineCommand(ExecutionContext context) : base(context)
		{
            IsStoredProcedure = true;
		}
		
	    protected override void BindParameters(OracleCommand command, AddPipelineParameterSet parameters)
	    {
            OutputParameter = command.AddReturnParameter<Guid>("p_entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_hidden", parameters.Hidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_pipeline_type_id", parameters.PipelineTypeId);
            command.AddInputParameter("P_KILOMETER_START", parameters.KilometerOfStart);
            command.AddInputParameter("P_KILOMETER_END", parameters.KilometerOfEnd);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
			command.AddInputParameter("P_SYSTEM_ID", parameters.GasTransportSystemId);
	    }
		
		protected override string GetCommandText(AddPipelineParameterSet parameters)
	    {
            return "rd.P_PIPELINE.AddF";
		}
	}
}

