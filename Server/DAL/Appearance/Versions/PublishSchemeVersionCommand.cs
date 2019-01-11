using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class PublishSchemeVersionCommand : CommandNonQuery<int>
    {
        public PublishSchemeVersionCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_scheme_version_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_ENTITY_SDO.Published_SCHEME_VERSION";
        }

    }
}