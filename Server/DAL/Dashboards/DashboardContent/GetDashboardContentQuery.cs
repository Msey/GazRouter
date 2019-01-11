using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardContent;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dashboards.DashboardContent
{
    public class GetDashboardContentQuery : QueryReader<int, DashboardContentDTO>
    {
        public GetDashboardContentQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameter)
        {
            return @"   SELECT      dashboard_id,
                                    dashboard_content
                        FROM        rd.v_dashboards
                        WHERE       dashboard_id = :dashboard_id";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("dashboard_id", parameters);
        }

        protected override DashboardContentDTO GetResult(OracleDataReader reader, int parameters)
        {
            var result = new DashboardContentDTO();
            if (reader.Read())
            {
                result.DashboardId = reader.GetValue<int>("dashboard_id");
                result.Content = reader.GetValue<string>("dashboard_content");
            }
            return result;
        }
    }
}