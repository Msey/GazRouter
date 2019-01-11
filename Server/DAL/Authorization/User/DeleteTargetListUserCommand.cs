using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.TargetingList;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Authorization.User
{
    public class DeleteTargetListUserCommand : CommandNonQuery<DeleteTargetingListParametersSet>
    {
        public DeleteTargetListUserCommand(ExecutionContext context) : base(context)
		{
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, DeleteTargetingListParametersSet parameters)
        {
            if (parameters.IsCpdd)
                command.AddInputParameter("p_agreed_lists_cpdd_id", parameters.Id);
            else
                command.AddInputParameter("p_agreed_list_id", parameters.Id);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(DeleteTargetingListParametersSet parameters)
        {
            return parameters.IsCpdd ? "P_AGREED_LIST_CPDD.Remove" : "P_AGREED_LIST.Remove";
        }
    }
}
