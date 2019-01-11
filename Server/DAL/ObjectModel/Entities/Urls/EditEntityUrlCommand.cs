using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.Urls
{

	public class EditEntityUrlCommand : CommandNonQuery<EditEntityUrlParameterSet>
	{
        public EditEntityUrlCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditEntityUrlParameterSet parameters)
        {
            command.AddInputParameter("p_entity_ext_urls_id", parameters.UrlId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_url", parameters.Url);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditEntityUrlParameterSet parameters)
        {
            return "rd.P_ENTITY_EXT_URL.Edit";
        }

        
    }

}

