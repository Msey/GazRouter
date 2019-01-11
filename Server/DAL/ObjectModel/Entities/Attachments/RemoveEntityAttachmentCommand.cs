using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.Attachments
{

	public class RemoveEntityAttachmentCommand : CommandNonQuery<int>
	{
        public RemoveEntityAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_entity_attach_id", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_ENTITY_ATTACH.Remove";
        }

        
    }

}

