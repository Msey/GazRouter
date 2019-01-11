using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Heaters
{
    public class RemoveHeaterTypeCommand : CommandNonQuery<int>
    {
        public RemoveHeaterTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_heater_type_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_heater_TYPE.Remove";
        }
    }
}
