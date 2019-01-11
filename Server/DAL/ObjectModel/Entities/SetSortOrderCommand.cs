using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities
{

	public class SetSortOrderCommand : CommandNonQuery<SetSortOrderParameterSet>
    {
		public SetSortOrderCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, SetSortOrderParameterSet parameters)
        {
			command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(SetSortOrderParameterSet parameters)
        {
			return "rd.p_entity.EDIT";
        }
    }

}

