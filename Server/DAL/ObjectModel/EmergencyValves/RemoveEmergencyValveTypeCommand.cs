using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.EmergencyValves
{
    public class RemoveEmergencyValveTypeCommand : CommandNonQuery<int>
    {
        public RemoveEmergencyValveTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_Emergency_Valve_types_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_Emergency_Valve_TYPE.Remove";
        }
    }
}
