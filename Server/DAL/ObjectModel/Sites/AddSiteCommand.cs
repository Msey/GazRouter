using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Sites;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Sites
{
	public class AddSiteCommand : CommandScalar<AddSiteParameterSet, Guid>
    {
        public AddSiteCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddSiteParameterSet parameters)
        {
            if (parameters.Id.HasValue)
            {
                command.AddInputParameter("p_entity_id", parameters.Id.Value);
            }
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("p_enterprise_id", parameters.ParentId);
            command.AddInputParameter("p_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_virtual", parameters.IsVirtual);
			command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_system_id", parameters.GasTransportSystemId);
            command.AddInputParameter("p_bal_group_id", parameters.BalanceGroupId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
			
        }

		protected override string GetCommandText(AddSiteParameterSet parameters)
        {
            return "rd.P_SITE.AddF";
        }
    }
}