using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairReport;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Repairs.Reports
{
    public class GetReportAttachementQuery : QueryReader<int, List<RepairReportAttachmentDTO>>
    {
        public GetReportAttachementQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameter)
        {
            return
@"select 
  v.repair_report_attachment_id,
  v.repair_reports_id,
  v.description,
  v.blob_id,
  v.cre_user_id,
  v.file_name,
  v.data_length,
  v.date_create,
  u.name
from v_repair_report_attachments v
left join v_users u on u.user_id = v.cre_user_id
where v.repair_reports_id = :reportid
order by v.date_create";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("reportid", parameters);
        }

        protected override List<RepairReportAttachmentDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var reportsList = new List<RepairReportAttachmentDTO>();
            while (reader.Read())
            {
                reportsList.Add(new RepairReportAttachmentDTO()
                {
                    Id = reader.GetValue<int>("repair_report_attachment_id"),
                    RepairReportID = reader.GetValue<int>("repair_reports_id"),
                    BlobId = reader.GetValue<Guid>("blob_id"),
                    CreationDate = reader.GetValue<DateTime>("date_create"),
                    Description = reader.GetValue<string>("description"),
                    FileName = reader.GetValue<string>("file_name"),
                    DataLength = reader.GetValue<int>("data_length"),
                    CreateUser = reader.GetValue<string>("name")
                });
            }
            return reportsList;
        }
    }
}
