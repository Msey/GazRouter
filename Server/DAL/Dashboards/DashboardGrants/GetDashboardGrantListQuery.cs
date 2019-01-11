using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardGrants;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.DashboardGrants
{
	public class GetDashboardGrantListQuery : QueryReader<int, List<DashboardGrantDTO>>
	{
		public GetDashboardGrantListQuery(ExecutionContext context)
			: base(context)
		{ }
		protected override string GetCommandText(int parameter)
		{
			return @"   SELECT      g.dashboard_id,
						            u.user_id,
                                    u.login,
                                    u.name AS user_name,
                                    u.site_id,
                                    e.entity_name AS site_name,
						            g.is_editable,
						            g.is_grantable,
                                    DECODE(g.user_id, NULL, 0, 1) AS is_visible                                   						            
					    FROM        v_users u 
                        INNER JOIN  v_entities e ON e.entity_id = u.site_id
                        LEFT JOIN   v_dashboards_grants g 
                            ON      u.user_id = g.user_id 
                            AND     g.dashboard_id = :id
                        ORDER BY    u.login";
		}
		protected override void BindParameters(OracleCommand command, int parameters)
		{
			command.AddInputParameter("id", parameters);
		}
        protected override List<DashboardGrantDTO> GetResult(OracleDataReader reader, int parameters)
		{
			var result = new List<DashboardGrantDTO>();
			while (reader.Read())
			{
                result.Add(new DashboardGrantDTO
				{
                    UserId = reader.GetValue<int>("user_id"),
                    Login = reader.GetValue<string>("login"),
                    UserName = reader.GetValue<string>("user_name"),
                    SiteId = reader.GetValue<Guid>("site_id"),
                    SiteName = reader.GetValue<string>("site_name"),
                    IsVisible = reader.GetValue<bool>("is_visible"),
                    IsEditable = reader.GetValue<bool>("is_editable"),
					IsGrantable = reader.GetValue<bool>("is_grantable")
				});
			}
			return result;
		}
	}
}
