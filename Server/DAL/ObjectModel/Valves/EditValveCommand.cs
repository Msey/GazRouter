using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Valves;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Valves
{
    public class EditValveCommand : CommandNonQuery<EditValveParameterSet>
	{
        public EditValveCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditValveParameterSet parameters)
	    {
            command.AddInputParameter("P_ENTITY_ID", parameters.Id);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_valve_type_id", parameters.ValveTypeId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer", parameters.Kilometr);
            command.AddInputParameter("p_bypass1_type_id", parameters.Bypass1TypeId);
            command.AddInputParameter("p_bypass2_type_id", parameters.Bypass2TypeId);
            command.AddInputParameter("p_bypass3_type_id", parameters.Bypass3TypeId);
            command.AddInputParameter("p_valve_purpose_id", parameters.ValvePurposeId);
            command.AddInputParameter("p_comp_shop_id", parameters.CompShopId);
            command.AddInputParameter("p_is_control_point", parameters.IsControlPoint);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }

        protected override string GetCommandText(EditValveParameterSet parameters)
	    {
            return "rd.P_VALVE.Edit";
	    }
	}
}