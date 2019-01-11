using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Reports
{
    public class GetReportByWorkQuery : QueryReader<int, List<RepairReportDTO>>
    {
        public GetReportByWorkQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameter)
        {
            return
@"select 
  v.repair_reports_id,
  v.repair_id,
  v.creation_date,
  v.report_date,
  v.description,
  v.cre_user_id,
  u.name
from v_repair_reports v
left join v_users u on u.user_id = v.cre_user_id
where v.repair_id = :repairid
order by v.report_date";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("repairid", parameters);
        }

        protected override List<RepairReportDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var reportsList = new List<RepairReportDTO>();
            while (reader.Read())
            {
                reportsList.Add(new RepairReportDTO()
                {
                    Id = reader.GetValue<int>("repair_reports_id"),
                    RepairID = reader.GetValue<int>("repair_id"),
                    CreationDate = reader.GetValue<DateTime>("creation_date"),
                    ReportDate = reader.GetValue<DateTime>("report_date"),
                    Comment = reader.GetValue<string>("description"),
                    CreateUser = reader.GetValue<string>("name")
                });
            }
            return reportsList;
        }
    }
}
