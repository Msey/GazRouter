using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.DependantSites;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.DependantSites
{
    public class AddDependantSiteCommand : CommandNonQuery<AddRemoveDependantSiteParameterSet>
    {
        public AddDependantSiteCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddRemoveDependantSiteParameterSet parameters)
        {
            command.AddInputParameter("p_site_id", parameters.SiteId);
            command.AddInputParameter("p_dependant_site_id", parameters.DependantSiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRemoveDependantSiteParameterSet parameters)
        {
            return "rd.P_INPUT_DEPENDANT_SITE.Add";
        }
    }
}