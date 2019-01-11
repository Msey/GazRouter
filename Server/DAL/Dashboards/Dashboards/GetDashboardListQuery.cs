using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.Dashboards
{

	public class GetDashboardListQuery : QueryReader<int,List<DashboardDTO>>
	{
		public GetDashboardListQuery(ExecutionContext context)
			: base(context)
		{ }
        /// <summary> v_folders => (select * from V_Folder_Reports union select * from v_folders) f </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
		protected override string GetCommandText(int parameters)

		{
		    return @"SELECT      d.dashboard_id,
                                    d.dashboard_name,
                                    d.user_id AS create_user_id,
                                    d.createdate,
                                    g.user_id,
                                    g.is_editable,
                                    g.is_grantable,
                                    f.folder_id,
                                    u.name AS create_user_name

                        FROM        v_dashboards d
                        INNER JOIN  v_dashboards_grants g ON g.dashboard_id = d.dashboard_id

                        LEFT JOIN ( SELECT      df.folder_id, df.dashboard_id 
                                    FROM        v_dashboards_folders df
LEFT JOIN (select * from V_Folder_Reports union select * from v_folders) f on f.folder_id = df.folder_id
                                    WHERE       f.user_id = :userid) f 
                                ON f.dashboard_id = d.dashboard_id

                        INNER JOIN  v_users u ON u.user_id = d.user_id
                        WHERE       1=1
                        ORDER BY    d.dashboard_name";
		}
        //  AND     g.user_id = :userid
        protected override void BindParameters(OracleCommand command, int parameters)
		{
			command.AddInputParameter("userid", parameters);
		}

        protected override List<DashboardDTO> GetResult(OracleDataReader reader, int parameters)
		{
			var dashBoardGrant = new List<DashboardDTO>();
			while (reader.Read())
			{
				dashBoardGrant.Add(new DashboardDTO
				{
					Id = reader.GetValue<int>("dashboard_id"),
					DashboardName = reader.GetValue<string>("dashboard_name"),
					CreateDate = reader.GetValue<DateTime>("createdate"),
					CreatorUserId = reader.GetValue<string>("create_user_id"),
					CreatorUserName = reader.GetValue<string>("create_user_name"),
					IsEditable = reader.GetValue<bool>("is_editable"),
					IsGrantable = reader.GetValue<bool>("is_grantable"),
					FolderId = reader.GetValue<int?>("folder_id")
				});
			}
			return dashBoardGrant;
		}
	}
}
