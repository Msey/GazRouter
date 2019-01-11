using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.MiscTab;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.MiscTab
{
    public class AddMiscTabEntityCommand : CommandNonQuery<AddRemoveMiscTabEntityParameterSet>
    {
        public AddMiscTabEntityCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddRemoveMiscTabEntityParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_system_id", parameters.SystemId);
            command.AddInputParameter("p_bal_group_id", parameters.BalGroupId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRemoveMiscTabEntityParameterSet parameters)
        {
            return "P_BL_MISC_TAB_ENTITY.Add";
        }

    }
}