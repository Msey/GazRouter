using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Pipelines;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Pipelines
{
    public class EditPipelinesCommand : CommandNonQuery<EditPipelineParameterSet>
	{
		public EditPipelinesCommand(ExecutionContext context) : base(context)
		{
            IsStoredProcedure = true;
		}
		
	    protected override void BindParameters(OracleCommand command, EditPipelineParameterSet parameters)
	    {
            command.AddInputParameter("P_ENTITY_ID", parameters.Id);
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
		
		protected override string GetCommandText(EditPipelineParameterSet parameters)
	    {
            return "rd.P_PIPELINE.Edit";
	    }
	}
}