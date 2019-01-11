using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.Dashboards
{
    public class GetDashboardUsersSharedListQuery : QueryReader<int, List<DashboardDTO>>
    {
        public GetDashboardUsersSharedListQuery(ExecutionContext context) : base(context){ }
        /// <summary> v_folders => (select * from V_Folder_Reports union select * from v_folders) f </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
		protected override string GetCommandText(int parameters)
        {
            return
                    @"select     
                        d.dashboard_id,
                        d.dashboard_name,
                        d.user_id AS create_user_id,
                        d.createdate    
                    from v_dashboards d 
                    left join
                        (select tg.dashboard_id, count(*) as dash_count  
                         from v_dashboards_grants tg  
                         group by tg.dashboard_id) gr
                    on d.DASHBOARD_ID = gr.DASHBOARD_ID
                    where gr.dash_count > 1";
        }
        protected override void BindParameters(OracleCommand command, int parameters){}
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
                });
            }
            return dashBoardGrant;
        }
    }
}
#region trash
//"select
//                d.DASHBOARD_ID,
//                d.dashboard_name,
//                d.user_id AS create_user_id,
//                d.createdate
//
//            from         v_dashboards d
//            inner join(select tg.DASHBOARD_ID, min(tg.USER_ID) as user_id_min
//        from         v_dashboards_grants tg group by tg.DASHBOARD_ID order by tg.DASHBOARD_ID) g
//        on           d.DASHBOARD_ID = g.DASHBOARD_ID
//        inner join v_users u ON u.user_id = d.user_id
//      order        by d.DASHBOARD_ID"


//            @"select
//                d.DASHBOARD_ID,
//                d.dashboard_name,
//                d.user_id AS create_user_id,
//                d.createdate
//
//            from         v_dashboards d
//            inner join   (select tg.DASHBOARD_ID,  min(tg.USER_ID) as user_id_min
//            from         v_dashboards_grants tg group by tg.DASHBOARD_ID order by tg.DASHBOARD_ID) g
//            on           d.DASHBOARD_ID = g.DASHBOARD_ID
//            inner join   v_users u ON u.user_id = d.user_id
//            order        by d.DASHBOARD_ID";


//            --inner join v_dashboards_grants gr ON gr.dashboard_id = d.dashboard_id
//
//            --u.user_id
//            --gr.is_editable,--
//            --gr.is_grantable,--
//            --f.folder_id,
//            --u.name AS create_user_name

//                    UserName = reader.GetValue<string>("create_user_name"),
//                    IsEditable = reader.GetValue<bool>("is_editable"),
//                    IsGrantable = reader.GetValue<bool>("is_grantable"),
//                    FolderId = reader.GetValue<int?>("folder_id")
#endregion
