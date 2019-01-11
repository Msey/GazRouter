using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
    public class GetExcelReportUsersSharedListQuery : QueryReader<int, List<DashboardDTO>>
    {
        public GetExcelReportUsersSharedListQuery(ExecutionContext context) : base(context){ }
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
                    from V_DASHBOARD_REPORTS d 
                    left join
                        (select tg.dashboard_id, count(*) as dash_count  
                         from v_dashboards_grants tg  
                         group by tg.dashboard_id) gr
                    on d.DASHBOARD_ID = gr.DASHBOARD_ID
                    where gr.dash_count > 1";
        }
        protected override void BindParameters(OracleCommand command, int parameters) { }
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
