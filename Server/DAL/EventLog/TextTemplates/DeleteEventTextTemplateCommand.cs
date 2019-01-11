using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.EventLog.TextTemplates
{
    public class DeleteEventTextTemplateCommand : CommandNonQuery<int>
    {
        public DeleteEventTextTemplateCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_event_text_template_id", parameters);
            
        }

        protected override string GetCommandText(int parameters)
        {
            return "eventlog.P_EVENT_TEXT_TEMPLATE.Remove";
        }
	}
}