using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.Urls
{

	public class AddEntityUrlCommand : CommandScalar<AddEntityUrlParameterSet, int>
    {
        public AddEntityUrlCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEntityUrlParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_entity_ext_urls_id");
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_url", parameters.Url);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEntityUrlParameterSet parameters)
        {
            return "rd.P_ENTITY_EXT_URL.AddF";
        }

        
    }

}

