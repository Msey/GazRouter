using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance.Versions;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance.Versions
{
    public class AddSchemeVersionCommand : CommandScalar<SchemeVersionParameterSet, int>
    {
        public AddSchemeVersionCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SchemeVersionParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("scheme_id");
            command.AddInputParameter("p_scheme_id", parameters.SchemeId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_content", parameters.Content);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SchemeVersionParameterSet parameters)
        {
            return "P_ENTITY_SDO.AddF_SCHEME_VERSION";
        }

    }
    
}