using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates.Failures.Attachment
{
    public class AddFailureAttachmentCommand: CommandScalar<AddAttachmentParameterSet<int>, int>
    {
        public AddFailureAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddAttachmentParameterSet<int> parameters)
		{
            OutputParameter = command.AddReturnParameter<int>("p_failure_act_id");
            command.AddInputParameter("p_comp_unit_failure_detail_id", parameters.ExternalId);
            command.AddInputParameter("p_data", parameters.Data);
            command.AddInputParameter("p_act_file_name", parameters.FileName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
		}

        protected override string GetCommandText(AddAttachmentParameterSet<int> parameters)
        {
            return "P_FAILURE_ACT.AddF";
        }
    }
}