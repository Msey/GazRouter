using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.User
{

	public class DeleteUserCommand : CommandNonQuery<int>
	{
		public DeleteUserCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
            IntegrityConstraints.Add("ORA-02292:", "Невозможно удалить пользователя т.к. есть связанные с ним данные");
		}

        protected override void BindParameters(OracleCommand command, int parameters)
	    {
            command.AddInputParameter("p_user_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }

        protected override string GetCommandText(int parameters)
	    {
            return "P_USER.Remove";
	    }
	}

}