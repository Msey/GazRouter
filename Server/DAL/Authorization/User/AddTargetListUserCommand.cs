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
    public class AddTargetListUserCommand : CommandScalar<AddEditTargetListUserParameterSet, int>
    {
        public AddTargetListUserCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditTargetListUserParameterSet parameters)
        {

            if (parameters.IsCpdd)
            {
                OutputParameter = command.AddReturnParameter<int>("p_agreed_lists_cpdd_id");
                command.AddInputParameter("p_cpdd_department", parameters.Department);
                command.AddInputParameter("p_fax", parameters.Fax);
                command.AddInputParameter("p_cpdd_fio", parameters.FIO);
            }
            else
            {
                OutputParameter = command.AddReturnParameter<int>("p_agreed_list_id");
                command.AddInputParameter("p_agreed_user_id", parameters.AgreedUserId);
            }
            command.AddInputParameter("p_entity_type_id", parameters.EntityTypeId);
            command.AddInputParameter("p_sortorder", parameters.SortNum);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEditTargetListUserParameterSet parameters)
        {
            return parameters.IsCpdd ? "P_AGREED_LIST_CPDD.AddF" : "P_AGREED_LIST.AddF";
        }
    }
}
