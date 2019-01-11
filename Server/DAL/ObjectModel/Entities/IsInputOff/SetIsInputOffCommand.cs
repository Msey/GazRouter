using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Entities.IsInputOff;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.IsInputOff
{

	public class SetIsInputOffCommand : CommandNonQuery<SetIsInputOffParameterSet>
    {
		public SetIsInputOffCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, SetIsInputOffParameterSet parameters)
        {
			command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_is_input_off", parameters.IsInputOff);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

		protected override string GetCommandText(SetIsInputOffParameterSet parameters)
        {
			return "rd.P_ENTITY.Set_INPUT_OFF";
        }
    }

}

