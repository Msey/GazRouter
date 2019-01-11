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
    
    public class GetTargetListUsersQuery : QueryReader<GetTargetingListParameterSet, List<TargetingListDTO>>
    {
        public GetTargetListUsersQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GetTargetingListParameterSet parameters)
        {
            if (parameters.IsCpdd)
            {
                return @"
select 
    a.agreed_lists_cpdd_id, a.entity_type_id, a.cpdd_department, a.cpdd_fio, a.fax, a.sortorder
from v_AGREED_LISTS_CPDD a
where 1 = 1 " +
(parameters.EntityTypeId.HasValue ? " and l.entity_type_id=:entitytype " : " ")
+ " order by sortorder";
            }
            else
            {
                return
                    @"
select 
    l.entity_type_id, l.sortorder, l.agreed_list_id, 
    a.agreed_user_id, a.user_id, a.fio, a.position, a.start_date, a.end_date, a.is_head, 
    en.entity_id, en.entity_name Site_Name, u.SITE_ID
from v_AGREED_LISTS l
    join V_AGREED_USERS a   on l.agreed_user_id = a.agreed_user_id
    join v_users u          on a.user_id = u.user_id
    join v_entities en      on u.SITE_ID = en.entity_id
where 1=1 "
    + (
    (parameters.SiteId.HasValue ? " and u.SITE_ID=:siteid " : " ")
    +
    (parameters.EntityTypeId.HasValue ? " and l.entity_type_id=:entitytype " : " "));
            }
        }

        protected override void BindParameters(OracleCommand command, GetTargetingListParameterSet parameters)
        {
            if (parameters != null)
            {
                if (parameters.IsCpdd)
                {
                    if (parameters.EntityTypeId.HasValue)
                        command.AddInputParameter("entitytype", parameters.EntityTypeId.Value);
                }
                else
                {
                    if (parameters.SiteId.HasValue)
                        command.AddInputParameter("siteid", parameters.SiteId.Value);
                    if (parameters.EntityTypeId.HasValue)
                        command.AddInputParameter("entitytype", parameters.EntityTypeId.Value);
                }
            }
        }

        protected override List<TargetingListDTO> GetResult(OracleDataReader reader, GetTargetingListParameterSet parameters)
        {
            var users = new List<TargetingListDTO>();
            while (reader.Read())
            {
                if (parameters.IsCpdd)
                {
                    var user = new TargetingListDTO()
                    {
                        Id = reader.GetValue<int>("agreed_lists_cpdd_id"),
                        EntityTypeId = reader.GetValue<int>("entity_type_id"),
                        FIO = reader.GetValue<string>("cpdd_fio"),
                        Position = reader.GetValue<string>("cpdd_department"),
                        Fax = reader.GetValue<string>("fax"),
                        SortOrder = reader.GetValue<int>("sortorder"),
                        IsCpdd = true
                    };

                    //string pos = reader.GetValue<string>("cpdd_department");
                    //if (pos.Contains("#"))
                    //{
                    //    var a = pos.Split('#');
                    //    user.Position = a[0];
                    //    user.Fax = a[1];
                    //}
                    //else
                    //{
                    //    user.Position = pos;
                    //}

                    users.Add(user);
                }
                else
                {
                    var user =
                        new TargetingListDTO
                        {
                            Id = reader.GetValue<int>("agreed_list_id"),
                            AgreedUserId = reader.GetValue<int>("agreed_user_id"),
                            EntityTypeId = reader.GetValue<int>("entity_type_id"),
                            FIO = reader.GetValue<string>("fio"),
                            IsHead = reader.GetValue<bool>("is_head"),
                            Position = reader.GetValue<string>("position"),
                            SiteId = reader.GetValue<Guid>("site_id"),
                            SiteName = reader.GetValue<string>("Site_Name"),
                            SortOrder = reader.GetValue<int>("sortorder"),
                            UserId = reader.GetValue<int>("user_id"),
                            IsCpdd = false
                        };
                    users.Add(user);
                }
                
            }
            return users;
        }
    }
}
