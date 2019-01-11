using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Authorization.User
{
    public class EditAgreedUserCommand : CommandNonQuery<EditAgreedUserParameterSet>
    {
        public EditAgreedUserCommand(ExecutionContext context) : base(context)
		{
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditAgreedUserParameterSet parameters)
        {
            command.AddInputParameter("p_agreed_user_id", parameters.AgreedUserId);
            command.AddInputParameter("p_user_id", parameters.UserId);
            command.AddInputParameter("p_fio", parameters.UserName);
            command.AddInputParameter("p_position", parameters.Position);
            command.AddInputParameter("p_start_date", parameters.StartDate);
            command.AddInputParameter("p_end_date", parameters.EndtDate);
            command.AddInputParameter("p_agreed_user_id_ref", parameters.ActingId);

            command.AddInputParameter("p_fio_r", parameters.UserName_R);
            command.AddInputParameter("p_position_r", parameters.Position_R);
            command.AddInputParameter("p_is_head", parameters.IsHead);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditAgreedUserParameterSet parameters)
        {
            return "P_AGREED_USER.Edit";
        }
    }
}
