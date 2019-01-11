using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Repairs.Agreed;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Authorization.User
{
    public class GetAgreedUsersAllQuery : QueryReader<GetAgreedUsersAllParameterSet, List<AgreedUserDTO>>
    {
        public GetAgreedUsersAllQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetAgreedUsersAllParameterSet parameters)
        {
            var sb = new StringBuilder(
@"select a.agreed_user_id,a.user_id,a.fio,a.position, u.site_id, en.entity_name as site_name, a.start_date,a.end_date,a.agreed_user_id_ref,r.USER_ID as USER_ID_REF, a.is_head, a.fio_r, a.position_r
from V_AGREED_USERS a
left join v_agreed_users r on a.agreed_user_id_ref = r.agreed_user_id
left join v_users u on u.user_id = a.user_id
left join v_entities en on u.site_id = en.entity_id
where 1=1");

            if (parameters.TargetDate.HasValue)
                sb.Append(" and a.start_date<=:targetdate and a.end_date>=:targetdate");

            if (parameters.SiteId.HasValue)
                sb.Append(" and u.site_id=:site_id");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetAgreedUsersAllParameterSet parameters)
        {
            if (parameters.TargetDate.HasValue)
                command.AddInputParameter("targetdate", parameters.TargetDate);
            if (parameters.SiteId.HasValue)
                command.AddInputParameter("site_id", parameters.SiteId);
        }

        protected override List<AgreedUserDTO> GetResult(OracleDataReader reader, GetAgreedUsersAllParameterSet parameters)
        {
            var users = new List<AgreedUserDTO>();
            while (reader.Read())
            {
                var user =
                    new AgreedUserDTO
                    {
                        AgreedUserId = reader.GetValue<int>("agreed_user_id"),
                        UserID = reader.GetValue<int>("user_id"),
                        FIO = reader.GetValue<string>("fio"),
                        Position = reader.GetValue<string>("position"),
                        SiteId = reader.GetValue<Guid?>("site_id"),
                        SiteName = reader.GetValue<string>("site_name"),
                        StartDate = reader.GetValue<DateTime>("start_date"),
                        EndDate = reader.GetValue<DateTime>("end_date"),
                        ActingAgreedUserId = reader.GetValue<int>("agreed_user_id_ref"),
                        ActingUserID = reader.GetValue<int>("user_id_ref"),
                        FIO_R = reader.GetValue<string>("fio_r"),
                        Position_r = reader.GetValue<string>("position_r"),
                        IsHead = reader.GetValue<bool>("is_head")
                    };

                users.Add(user);
            }
            return users;
        }
    }
}
