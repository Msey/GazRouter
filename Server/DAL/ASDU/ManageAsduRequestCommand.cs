using GazRouter.DAL.Core;
using GazRouter.DTO.ASDU;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class ManageAsduRequestCommand : CommandScalar<ManageRequestParams, int>
    {
        public ManageAsduRequestCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, ManageRequestParams parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("dummy");
            command.AddInputParameter("p_id", parameters.Key);
            command.AddInputParameter("p_name", parameters.Name);
            command.AddInputParameter("p_xml", parameters.Xml);
            command.AddInputParameter("p_action", parameters.Action);
            command.AddInputParameter("p_userid", parameters.UserId);
            command.AddInputParameter("p_enterpriseid", parameters.EnterpriseId);
        }

        protected override string GetCommandText(ManageRequestParams parameters)
        {
            return "integro.p_md_loaddata.manage_loaded_file";
        }
    }
}