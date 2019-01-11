using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.EmergencyValves;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.EmergencyValves
{
    public class AddEmergencyValveTypeCommand : CommandScalar<AddEmergencyValveTypeParameterSet, int>
    {
        public AddEmergencyValveTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEmergencyValveTypeParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_emergency_valve_types_id");
            command.AddInputParameter("p_emergency_valve_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_inner_diameter", parameters.InnerDiameter);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEmergencyValveTypeParameterSet parameters)
        {
            return "rd.P_EMERGENCY_VALVE_TYPE.AddF";
        }
    }
}
