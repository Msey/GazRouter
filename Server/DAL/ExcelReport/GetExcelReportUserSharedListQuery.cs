using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
    public class GetExcelReportUserSharedListQuery : QueryReader<int, List<DashboardDTO>>
    {
        public GetExcelReportUserSharedListQuery(ExecutionContext context) : base(context){ }
        protected override string GetCommandText(int parameters)
        {
            return
              @"  select * from
                    (select
                        d.dashboard_id,
                        d.dashboard_name,
                        d.user_id AS create_user_id,
                        d.createdate
                    from V_DASHBOARD_REPORTS d
                    left
                    join
                        (select tg.dashboard_id, count(*) as dash_count
                         from v_dashboards_grants tg
                         group by tg.dashboard_id) gr
                    on d.DASHBOARD_ID = gr.DASHBOARD_ID
                    where gr.dash_count > 1) t
                    inner join 
                        (select * from v_dashboards_grants g 
                         where g.user_id = :id) t2 
                         on t.dashboard_id = t2.dashboard_id
                    where t.create_user_id != :id";
        }
        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("id", parameters);
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
                });
            }
            return dashBoardGrant;
        }
    }
}

