using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Sites;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Sites
{
	public class EditSiteCommand : CommandNonQuery<EditSiteParameterSet>
    {
        public EditSiteCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditSiteParameterSet parameters)
        {
            command.AddInputParameter("p_entity_id", parameters.Id);
            command.AddInputParameter("p_enterprise_id", parameters.ParentId);
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
            command.AddInputParameter("p_system_id", parameters.GasTransportSystemId);
            command.AddInputParameter("p_bal_group_id", parameters.BalanceGroupId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);

        }

		protected override string GetCommandText(EditSiteParameterSet parameters)
        {
            return "rd.P_SITE.Edit";
        }
    }
}