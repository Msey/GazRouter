using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.Attachments;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.Attachments
{
    public class GetTaskAttachmentListQuery : QueryReader<Guid, List<TaskAttachmentDTO>>
    {
        public GetTaskAttachmentListQuery(ExecutionContext context)
            : base(context)
        {
        }

		protected override string GetCommandText(Guid parameters)
		{
		    var sql = @"    SELECT      a.task_attachment_id,
                                        a.task_id,
                                        a.blob_id,
                                        a.description,
                                        a.create_date,
                                        a.create_user_id,
                                        u.name AS user_name,
                                        u.description AS user_description,
                                        a.attachment_file_name,
                                        br.data_length

                            FROM        v_task_attachments a 
                            JOIN        v_users u ON u.user_id = a.create_user_id
                            JOIN        v_blob_rows br ON br.blob_id = a.blob_id
                            WHERE       task_id = :taskid";


            return sql;
        }

		protected override void BindParameters(OracleCommand command, Guid parameters)
        {
            command.AddInputParameter("taskid", parameters);
        }

        protected override List<TaskAttachmentDTO> GetResult(OracleDataReader reader, Guid parameters)
        {
            var result = new List<TaskAttachmentDTO>();
            while (reader.Read())
            {
                result.Add(
					new TaskAttachmentDTO
                    {
						Id = reader.GetValue<Guid>("task_attachment_id"),
						ExternalId = reader.GetValue<Guid>("task_id"),
                        Description = reader.GetValue<string>("description"),
                        BlobId = reader.GetValue<Guid>("blob_id"),
						FileName = reader.GetValue<string>("attachment_file_name"),
                        CreateDate = reader.GetValue<DateTime>("create_date"),
                        UserId = reader.GetValue<int>("create_user_id"),
                        UserName = reader.GetValue<string>("user_name"),
                        UserDescription = reader.GetValue<string>("user_description"),
                        DataLength = reader.GetValue<int>("data_length")

                    });
            }
            return result;
        }
    }
}