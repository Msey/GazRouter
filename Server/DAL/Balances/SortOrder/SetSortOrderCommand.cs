using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.SortOrder;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.SortOrder
{
    public class SetSortOrderCommand : CommandNonQuery<SetBalSortOrderParameterSet>
    {
        public SetSortOrderCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, SetBalSortOrderParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_bal_item_id", parameters.BalItem);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetBalSortOrderParameterSet parameters)
		{
            return "rd.P_BL_SORT_ORDER.set_sort_order";
		}

    }
}