using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventTextTemplates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.TextTemplates
{
    public class AddEventTextTemplateCommand : CommandScalar<AddEventTextTemplateParameterSet, int>
    {
        public AddEventTextTemplateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEventTextTemplateParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_event_text_template_id");
            command.AddInputParameter("p_template_name", parameters.Name);
            command.AddInputParameter("p_template_text", parameters.Text);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEventTextTemplateParameterSet parameters)
        {
            return "eventlog.P_EVENT_TEXT_TEMPLATE.AddF";
        }
    }
}
