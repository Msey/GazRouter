using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Versions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class EditSchemeVersionCommentCommand : CommandNonQuery<CommentSchemeVersionParameterSet>
    {
        public EditSchemeVersionCommentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, CommentSchemeVersionParameterSet parameters)
        {
            command.AddInputParameter("p_scheme_version_id", parameters.SchemeVersionId);
            command.AddInputParameter("P_REM", parameters.Comment);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(CommentSchemeVersionParameterSet parameters)
        {
            return "P_ENTITY_SDO.EDIT_SCHEME_VERSION_REM";
        }
    }
}