using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.PrintForms;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Repairs.PrintForms
{
    public class GetSiteFromQuery : QueryReader<GetSignersSet, List<TargetingUserDTO>>
    {
        public GetSiteFromQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetSignersSet parameters)
        {
            return @"
select 
    l.entity_type_id, l.sortorder, l.agreed_list_id, 
    a.agreed_user_id, a.user_id, a.fio, a.position, a.is_head, 
    en.entity_id, en.entity_name Site_Name, u.SITE_ID
from v_AGREED_LISTS l
    join V_AGREED_USERS a   on l.agreed_user_id = a.agreed_user_id
    join v_users u          on a.user_id = u.user_id
    join v_entities en      on u.SITE_ID = en.entity_id
where 
    l.entity_type_id = :entitytype and u.SITE_ID = :siteid
order by l.sortorder";
        }

        protected override void BindParameters(OracleCommand command, GetSignersSet parameters)
        {
            command.AddInputParameter("entitytype", parameters.EntityTypeId);
            command.AddInputParameter("siteid", parameters.ToId);
        }

        protected override List<TargetingUserDTO> GetResult(OracleDataReader reader, GetSignersSet parameters)
        {
            var result = new List<TargetingUserDTO>();
            while (reader.Read())
            {
                var user = new TargetingUserDTO()
                {
                    FIO = reader.GetValue<string>("fio"),
                    SortOrder = reader.GetValue<int>("sortorder"),
                    Position = reader.GetValue<string>("position"),
                    IsHead = reader.GetValue<bool>("is_head")
                };

                result.Add(user);
            }
            return result;
        }
    }
}
