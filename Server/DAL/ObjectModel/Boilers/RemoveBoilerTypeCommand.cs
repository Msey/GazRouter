using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Boilers
{
    public class RemoveBoilerTypeCommand : CommandNonQuery<int>
    {
        public RemoveBoilerTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_boiler_type_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_BOILER_TYPE.Remove";
        }
    }
}
