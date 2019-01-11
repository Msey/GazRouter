using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Entities;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities
{

	public class AddDescriptionCommand : CommandNonQuery<AddDescriptionParameterSet>
    {
        public AddDescriptionCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddDescriptionParameterSet parameters)
        {
			command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddDescriptionParameterSet parameters)
        {
            return "rd.p_entity.Set_DESCRIPTION";
        }
    }

}

