using System;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.ValueMessages
{
    public class AcceptMessageCommand : CommandNonQuery<Guid>
    {
        public AcceptMessageCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("p_value_message_id", parameters);
            command.AddInputParameter("p_raise", 1);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(Guid parameters)
        {
            return "rd.P_VALUE_MESSAGE.Accept";
        }

    }

}