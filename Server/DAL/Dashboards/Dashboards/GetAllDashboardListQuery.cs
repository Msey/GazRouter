using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.Dashboards
{
    public class GetAllDashboardListQuery : QueryReader<List<DashboardDTO>>
    {
        public GetAllDashboardListQuery(ExecutionContext context) : base(context) { }

        protected override string GetCommandText()
        {
            return @"SELECT         d.dashboard_id,   
                                    d.dashboard_name, 
                                    d.folder_id, 
                                    d.cre_user_login,                                                                                   
                                    d.createdate,               
                                    d.period_type_id,
                                    d.sort_order,
                                    d.row_type     
                     FROM           v_dashboards d
                     WHERE          d.is_deleted = 0";
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
                    RowType = reader.GetValue<int>("row_type"),
                });
            }
            return dashBoardGrant;
        }
    }
}
#region trash
// is_deleted
// g.user_id,     
// IsEditable = reader.GetValue<bool>("is_editable"),
// IsGrantable = reader.GetValue<bool>("is_grantable"),
// FolderId = reader.GetValue<int?>("folder_id")
#endregion