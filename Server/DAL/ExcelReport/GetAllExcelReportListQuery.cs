using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
    public class GetAllExcelReportListQuery : QueryReader<List<DashboardDTO>>
    {
        public GetAllExcelReportListQuery(ExecutionContext context) : base(context){ }
        protected override string GetCommandText()
        {
            return @"select   t.dashboard_id,
                              t.dashboard_name,
                              t.folder_id,
                              t.cre_user_login,
                              t.createdate,                             
                              t.Period_Type_Id,
                              t.sort_order
				     from     v_dashboard_reports t
                     where    t.is_deleted = 0";
        }
        protected override void BindParameters(OracleCommand command)
        {
        }
        protected override List<DashboardDTO> GetResult(OracleDataReader reader)
        {
            var dashBoardGrant = new List<DashboardDTO>();
            while (reader.Read())
            {
                dashBoardGrant.Add(new DashboardDTO
                {
                    Id = reader.GetValue<int>("dashboard_id"),
                    DashboardName = reader.GetValue<string>("dashboard_name"),
                    FolderId = reader.GetValue<int?>("folder_id"),
                    CreatorUserId = reader.GetValue<string>("cre_user_login"),
                    CreateDate = reader.GetValue<DateTime>("createdate"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"), 
                    SortOrder = reader.GetValue<int?>("sort_order"), 
                });
            }
            return dashBoardGrant;
        }
    }
}
