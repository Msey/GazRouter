using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.BalanceGroups;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.BalanceGroups
{
    public class AddBalanceGroupCommand : CommandScalar<AddBalanceGroupParameterSet, int>
    {
        public AddBalanceGroupCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddBalanceGroupParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_group_id");
            command.AddInputParameter("p_name", parameters.Name);
            command.AddInputParameter("p_system_id", parameters.SystemId);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddBalanceGroupParameterSet parameters)
        {
            return "rd.P_BL_GROUP.AddF";
        }

    }

}