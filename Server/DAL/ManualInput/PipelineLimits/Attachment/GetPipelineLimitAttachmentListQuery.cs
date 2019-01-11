using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ManualInput.PipelineLimits.Attachment
{
    public class GetPipelineLimitAttachmentListQuery : QueryReader<List<int>, List<AttachmentDTO<int, int>>>
    {
        public GetPipelineLimitAttachmentListQuery(ExecutionContext context)
            : base(context)
        {

        }


        protected override string GetCommandText(List<int> parameters)
        {
            var q = string.Format(
                @"  SELECT  t1.limit_attachment_id,
                            t1.limit_id,
                            t1.attach_file_name,
                            t1.blob_id,
                            t1.description,
                            t1.attach_data_length
                    FROM    V_PIPLINE_LIMIT_ATTACHMENTS t1
                    WHERE   t1.limit_id IN {0}", CreateInClause(parameters.Count));

            return q;
        }

        protected override List<AttachmentDTO<int, int>> GetResult(OracleDataReader reader, List<int> parameters)
        {
            var attachments = new List<AttachmentDTO<int, int>>();
            while (reader.Read())
            {
                var att =
                    new AttachmentDTO<int, int>
                    {
                        Id = reader.GetValue<int>("limit_attachment_id"),
                        ExternalId = reader.GetValue<int>("limit_id"),
                        FileName = reader.GetValue<string>("attach_file_name"),
                        BlobId = reader.GetValue<Guid>("blob_id"),
                        Description = reader.GetValue<string>("description"),
                        DataLength = reader.GetValue<int>("attach_data_length"),
                    };
                attachments.Add(att);
            }
            return attachments;
        }



        protected override void BindParameters(OracleCommand command, List<int> parameters)
        {
            if (parameters == null) return;

            for (var i = 0; i < parameters.Count; i++)
                command.AddInputParameter(string.Format("p{0}", i), parameters[i]);
        }

    }
}
