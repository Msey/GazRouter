
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.EmergencyValves;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.EmergencyValves
{
    public class EditEmergencyValveTypeCommand : CommandNonQuery<EditEmergencyValveTypeParameterSet>
    {
        public EditEmergencyValveTypeCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditEmergencyValveTypeParameterSet parameters)
        {
            command.AddInputParameter("p_emergency_valve_types_id", parameters.Id);
            command.AddInputParameter("p_emergency_valve_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_inner_diameter", parameters.InnerDiameter);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditEmergencyValveTypeParameterSet parameters)
        {
            return "rd.P_EMERGENCY_VALVE_TYPE.Edit";
        }
    }
}