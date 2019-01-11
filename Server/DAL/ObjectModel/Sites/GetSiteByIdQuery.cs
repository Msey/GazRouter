using System;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Sites;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Sites
{
    public class GetSiteByIdQuery : QueryReader<Guid, SiteDTO>
    {
        public GetSiteByIdQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(Guid parameters)
        {
            var sql = @"    SELECT      s.site_id, 
                                        s.site_name, 
                                        s.enterprise_id, 
                                        s.status, 
                                        n.entity_name AS full_name, 
                                        n1.entity_name AS short_name, 
                                        s.system_id, 
                                        s.sort_order, 
                                        s.description,
                                        s.bal_group_id,
                                        bg.name AS bal_group_name                
                            FROM        rd.v_sites s
                            LEFT JOIN   v_nm_all n ON s.site_id = n.entity_id
                            LEFT JOIN   v_nm_short_all n1 ON s.site_id = n1.entity_id
                            LEFT JOIN   v_bl_groups bg ON s.bal_group_id = bg.group_id
                            WHERE       s.site_id = :siteId";


            return sql;
        }

        protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("siteId", parameters);
        }

        protected override SiteDTO GetResult(OracleDataReader reader, Guid parameters)
        {
            if (reader.Read())
            {
                return new SiteDTO
                {
                    Id = reader.GetValue<Guid>("site_id"),
                    Name = reader.GetValue<string>("site_name"),
                    ParentId = reader.GetValue<Guid>("enterprise_id"),
                    Path = reader.GetValue<string>("full_name"),
                    ShortPath = reader.GetValue<string>("short_name"),
                    SystemId = reader.GetValue<int>("system_id"),
                    SortOrder = reader.GetValue<int>("sort_order"),
                    Status = reader.GetValue<EntityStatus?>("status"),
                    Description = reader.GetValue<string>("description"),
                    BalanceGroupId = reader.GetValue<int>("bal_group_id"),
                    BalanceGroupName = reader.GetValue<string>("bal_group_name")
                };
            }

            return null;
        }
    }
}