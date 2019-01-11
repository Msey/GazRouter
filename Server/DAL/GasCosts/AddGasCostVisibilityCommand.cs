using GazRouter.DAL.Core;
using GazRouter.DTO.GasCosts;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.GasCosts
{
    public class AddGasCostVisibilityCommand : CommandNonQuery<AddGasCostVisibilityParameterSet>
    {
        public AddGasCostVisibilityCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }
        protected override string GetCommandText(AddGasCostVisibilityParameterSet parameters)
        {
            return "P_AUX_ITEM.Set_VISIBLE";
        }
        protected override void BindParameters(OracleCommand command, AddGasCostVisibilityParameterSet parameters)
        {
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_aux_item_id", parameters.CostType);
            command.AddInputParameter("p_visible", parameters.Visibility);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
    }
}
