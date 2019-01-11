using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class DeleteSchemeVersionCommand : CommandNonQuery<int>
    {
        public DeleteSchemeVersionCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int id)
        {
            command.AddInputParameter("p_scheme_version_id", id);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int id)
        {
            return "P_ENTITY_SDO.Remove_SCHEME_VERSION";
        }
    }
}