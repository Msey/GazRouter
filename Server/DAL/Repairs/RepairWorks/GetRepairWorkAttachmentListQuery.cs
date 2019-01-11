using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairWorks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.RepairWorks
{
    public class GetRepairWorkAttachmentListQuery : QueryReader<int, List<RepairWorkAttachmentDTO>>
    {
        public GetRepairWorkAttachmentListQuery(ExecutionContext context)
            : base(context)
        {
        }

		protected override string GetCommandText(int parameters)
		{
		    var sql = @"    SELECT      a.repair_attachment_id,
                                        a.repair_id,
                                        a.blob_id,
                                        a.description,
                                        a.date_create,
                                        a.cre_user_id,
                                        u.name AS user_name,
                                        u.description AS user_description,
                                        a.file_name,
                                        a.data_length

                            FROM        v_repair_attachments a 
                            JOIN        v_users u ON u.user_id = a.cre_user_id
                            JOIN        v_blob_rows br ON br.blob_id = a.blob_id
                            WHERE       a.repair_id = :repairid";


            return sql;
        }

		protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("repairid", parameters);
        }

        protected override List<RepairWorkAttachmentDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<RepairWorkAttachmentDTO>();
            while (reader.Read())
            {
                result.Add(
					new RepairWorkAttachmentDTO
                    {
						Id = reader.GetValue<int>("repair_attachment_id"),
						ExternalId = reader.GetValue<int>("repair_id"),
                        Description = reader.GetValue<string>("description"),
                        BlobId = reader.GetValue<Guid>("blob_id"),
                        CreationDate = reader.GetValue<DateTime>("date_create"),
                        FileName = reader.GetValue<string>("file_name"),
                        UserId = reader.GetValue<int>("cre_user_id"),
                        UserName = reader.GetValue<string>("user_name"),
                        UserDescription = reader.GetValue<string>("user_description"),
                        DataLength = reader.GetValue<int>("data_length"),

                    });
            }
            return result;
        }
    }
}