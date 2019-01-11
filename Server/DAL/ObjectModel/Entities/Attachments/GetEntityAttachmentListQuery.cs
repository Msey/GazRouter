using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities.Attachments
{
    public class GetEntityAttachmentListQuery : QueryReader<Guid?, List<AttachmentDTO<int, Guid>>>
    {
        public GetEntityAttachmentListQuery(ExecutionContext context) : base(context)
        {
        }


        protected override void BindParameters(OracleCommand command, Guid? parameters)
        {
           command.AddInputParameter("entid", parameters);
        }

        protected override string GetCommandText(Guid? parameters)
        {

			var q = @"  SELECT  entity_attach_id,
                                entity_id,
                                description,
                                blob_id,
                                attach_file_name,
                                attach_data_length
 
                        FROM    v_entity_attachs
                        WHERE   1=1";

            var sb = new StringBuilder(q);
            if (parameters.HasValue)
            {
                sb.Append(" AND entity_id = :entid");
            }

            return sb.ToString();
        }

        protected override List<AttachmentDTO<int, Guid>> GetResult(OracleDataReader reader, Guid? parameters)
        {
            var result = new List<AttachmentDTO<int, Guid>>();
            while (reader.Read())
            {
                result.Add(
                    new AttachmentDTO<int, Guid>
                    {
                        Id = reader.GetValue<int>("entity_attach_id"),
                        ExternalId = reader.GetValue<Guid>("entity_id"),
						Description = reader.GetValue<string>("description"),
                        BlobId = reader.GetValue<Guid>("blob_id"),
                        FileName = reader.GetValue<string>("attach_file_name"),
                        DataLength = reader.GetValue<int>("attach_data_length")
                    });
            }

            return result;
        }
    }
}
