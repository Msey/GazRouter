using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dashboards.DashboardContent;
using GazRouter.DTO.ExcelReports;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ExcelReport
{
    public class GetExcelReportContentQuery : QueryReader<int, ExcelReportContentDTO>
    {
        public GetExcelReportContentQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameter)
        {
            return @"Select     DASHBOARD_ID,
                                REPORT_DATA
                    From        rd.V_DASHBOARD_REPORTS
                    Where       DASHBOARD_ID = :DASHBOARD_ID";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter(":DASHBOARD_ID", parameters);
        }

        protected override ExcelReportContentDTO GetResult(OracleDataReader reader, int parameters)
        {
            var result = new ExcelReportContentDTO();
            if (reader.Read())
            {
                result.ReportId = reader.GetValue<int>("DASHBOARD_ID");
                result.Content = reader.GetValue<byte[]>("REPORT_DATA");
            }
            return result;
        }
    }
}