using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.BalanceGroups;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.BalanceGroups
{
    public class EditBalanceGroupCommand : CommandNonQuery<EditBalanceGroupParameterSet>
    {
        public EditBalanceGroupCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditBalanceGroupParameterSet parameters)
        {
            command.AddInputParameter("p_group_id", parameters.Id);
            command.AddInputParameter("p_name", parameters.Name);
            command.AddInputParameter("p_system_id", parameters.SystemId);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditBalanceGroupParameterSet parameters)
        {
            return "rd.P_BL_GROUP.Edit";
        }

    }
}
