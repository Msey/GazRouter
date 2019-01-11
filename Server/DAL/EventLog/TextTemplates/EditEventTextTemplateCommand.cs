using GazRouter.DAL.Core;
using GazRouter.DTO.EventLog.EventTextTemplates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.TextTemplates
{
    public class EditEventTextTemplateCommand : CommandNonQuery<EditEventTextTemplateParameterSet>
    {
        public EditEventTextTemplateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditEventTextTemplateParameterSet parameters)
        {
            command.AddInputParameter("p_event_text_template_id", parameters.Id);
            command.AddInputParameter("p_template_name", parameters.Name);
            command.AddInputParameter("p_template_text", parameters.Text);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditEventTextTemplateParameterSet parameters)
        {
            return "eventlog.P_EVENT_TEXT_TEMPLATE.Edit";
        }
	}
}