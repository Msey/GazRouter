using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dashboards.Folders;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Dashboards.Dashboards
{
    public class GetDashboardFoldersQuery : QueryReader<int, List<DashboardFoldersDTO>>
    {
        public GetDashboardFoldersQuery(ExecutionContext context) : base(context){ }

        protected override string GetCommandText(int parameter)
        {
            return @"   SELECT      df.DASHBOARD_ID,
                                    df.FOLDER_ID,
                                    df.SORT_ORDER
                        FROM        v_dashboards_folders df";
        }
        protected override void BindParameters(OracleCommand command, int parameters)
        {
        }
        protected override List<DashboardFoldersDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<DashboardFoldersDTO>();
            while (reader.Read())
            {
                result.Add(new DashboardFoldersDTO
                {
                    DashboardId = reader.GetValue<int>("dashboard_id"),
                    FolderId = reader.GetValue<int>("folder_id"),
                    SortOrder = reader.GetValue<int>("sort_order")
                });
            }
            return result;
        }
    }
}
