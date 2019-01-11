using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.Attachments
{

	public class AddEntityAttachmentCommand : CommandScalar<AddAttachmentParameterSet<Guid>, int>
    {
        public AddEntityAttachmentCommand(ExecutionContext context)
			: base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddAttachmentParameterSet<Guid> parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_entity_attach_id");

            command.AddInputParameter("p_entity_id", parameters.ExternalId);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_attach_data", parameters.Data);
            command.AddInputParameter("p_attach_file_name", parameters.FileName);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddAttachmentParameterSet<Guid> parameters)
        {
            return "rd.P_ENTITY_ATTACH.AddF";
        }

        
    }

}

