using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class CreateAsduOutboundCommand : CommandScalar<int, string>
    {
        public CreateAsduOutboundCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            OutputParameter = command.AddReturnParameter<string>("p_xml");
            command.AddInputParameter("p_id", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return "integro.p_md_tree.create_asdu_outbound_xml";
        }
    }
}