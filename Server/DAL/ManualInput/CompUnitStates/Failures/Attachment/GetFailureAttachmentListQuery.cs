using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates.Failures.Attachment
{
    public class GetFailureAttachmentListQuery : QueryReader<List<int>, List<AttachmentDTO<int, int>>>
    {
        public GetFailureAttachmentListQuery(ExecutionContext context)
            : base(context)
        {
        }


        protected override string GetCommandText(List<int> parameters)
        {
            var q = @"  SELECT      a.failure_act_id,
                                    a.comp_unit_failure_detail_id,
                                    a.description,
                                    a.blob_id,
                                    a.data_length,
                                    a.act_file_name,
                                    a.date_create
                        FROM        v_failure_acts a";
                        
            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                sb.Append(" WHERE a.comp_unit_failure_detail_id IN ");
                sb.Append(CreateInClause(parameters.Count));
            }

            sb.Append(" ORDER BY a.date_create");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, List<int> parameters)
        {
            if (parameters == null) return;

            for (var i = 0; i < parameters.Count; i++)
                command.AddInputParameter(string.Format("p{0}", i), parameters[i]);
            
        }

        protected override List<AttachmentDTO<int, int>> GetResult(OracleDataReader reader, List<int> parameters)
        {
            var result = new List<AttachmentDTO<int, int>>();
            while (reader.Read())
            {
                var dto = new AttachmentDTO<int, int>
                {
                    Id = reader.GetValue<int>("failure_act_id"),
                    ExternalId = reader.GetValue<int>("comp_unit_failure_detail_id"),
                    Description = reader.GetValue<string>("description"),
                    BlobId = reader.GetValue<Guid>("blob_id"),
                    DataLength = reader.GetValue<int>("data_length"),
                    FileName = reader.GetValue<string>("act_file_name")
                };
                
                result.Add(dto);
            }
            return result;
        }
    }
}
