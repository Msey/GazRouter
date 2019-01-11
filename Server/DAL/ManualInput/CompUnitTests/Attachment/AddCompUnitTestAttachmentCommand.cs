using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests.Attachment
{
    public class AddCompUnitTestAttachmentCommand : CommandScalar<AddAttachmentParameterSet<int>, int>
    {
        public AddCompUnitTestAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddAttachmentParameterSet<int> parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_comp_unit_test_att_id");
            command.AddInputParameter("p_comp_unit_test_id", parameters.ExternalId);
            command.AddInputParameter("p_file_data", parameters.Data);
            command.AddInputParameter("p_file_name", parameters.FileName);
            command.AddInputParameter("p_description",parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            
        }

        protected override string GetCommandText(AddAttachmentParameterSet<int> parameters)
        {
            return "P_COMP_UNITS_TEST.AddF_ATT";
        }
    }
}
