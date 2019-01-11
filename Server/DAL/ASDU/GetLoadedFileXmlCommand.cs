using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class GetLoadedFileXmlCommand : CommandScalar<string, string>
    {
        public GetLoadedFileXmlCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, string parameters)
        {
            OutputParameter = command.AddReturnParameter<string>("p_xml", OracleDbType.Clob);
            command.AddInputParameter("p_id", parameters);
        }

        protected override string GetCommandText(string parameters)
        {
            return "integro.p_md_loaddata.load_packet_xml";
        }
    }
}