using GazRouter.DAL.Core;
using GazRouter.DTO.Appearance;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Appearance
{
    public class AddSchemeCommand : CommandScalar<SchemeParameterSet, int>
    {
        public AddSchemeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, SchemeParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("scheme_type_id");
            command.AddInputParameter("p_scheme_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_system_id", parameters.SystemId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SchemeParameterSet parameters)
        {
            return "P_ENTITY_SDO.AddF_SCHEME";
        }

    }
    
}