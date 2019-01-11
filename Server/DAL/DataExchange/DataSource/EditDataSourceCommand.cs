using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.DataSource;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.DataSource
{
    public class EditDataSourceCommand : CommandNonQuery<EditDataSourceParameterSet>
    {
        public EditDataSourceCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditDataSourceParameterSet parameters)
        {
            command.AddInputParameter("p_source_id", parameters.Id);
            command.AddInputParameter("p_source_name", parameters.Name);
            command.AddInputParameter("p_system_name", parameters.SysName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(EditDataSourceParameterSet parameters)
        {
            return "rd.P_SOURCE.Edit";
        }

    }
}