using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.MeasLine;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.MeasLine
{
    public class EditMeasLineCommand : CommandNonQuery<EditMeasLineParameterSet>
	{
		public EditMeasLineCommand(ExecutionContext context) : base(context)
		{
            IsStoredProcedure = true;
		}
		
	    protected override void BindParameters(OracleCommand command, EditMeasLineParameterSet parameters)
	    {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_status", parameters.Status);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_meas_station_id", parameters.ParentId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_conn", parameters.KmOfConn);
            command.AddInputParameter("p_bal_name", parameters.BalanceName);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }
		
		protected override string GetCommandText(EditMeasLineParameterSet parameters)
	    {
            return "rd.P_MEAS_LINE.Edit";
	    }
	}
}