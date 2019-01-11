using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskStatuses
{
    public class GetTaskStatusListQuery : QueryReader<Guid, List<TaskStatusDTO>>
	{
        public GetTaskStatusListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(Guid parameters)
		{
			return @"   SELECT      tv.task_version_id,
                                    tv.status_type_id,
                                    tv.task_id,
                                    tv.ack_date,
                                    tv.ack_user_id,
                                    tv.create_date,
                                    tv.create_user_id,
                                    tv.create_cpdd_user_name,
                                    tv.status_type_name,
                                    tv.status_type_code,
                                    tv.create_user_name ,
                                    tk.global_task_id,
                                    CASE 
                                        WHEN   tk.last_version_id = tv.task_version_id 
                                        THEN    1 
                                        ELSE    0 
                                    END AS  is_last_version,
                                    ta.reason

                        FROM        v_task_versions tv
                        INNER JOIN  v_tasks tk ON tk.task_id = tv.task_id
                        LEFT JOIN   v_tasks_annuled ta ON ta.task_id = tv.task_id 
                            AND     ta.status_type_id = tv.status_type_id
                        
                        WHERE       tv.task_id = :id
                        ORDER BY    tv.create_date DESC";
		}

        protected override void BindParameters(OracleCommand command, Guid parameters)
		{
            command.AddInputParameter(":id", parameters);
		}

        protected override List<TaskStatusDTO> GetResult(OracleDataReader reader, Guid parameters)
		{
            var result = new List<TaskStatusDTO>();
			while (reader.Read())
			{
                result.Add(
                    new TaskStatusDTO
					{
						Id = reader.GetValue<Guid>("task_version_id"),
						TaskId = reader.GetValue<Guid>("task_id"),
						StatusTypeId = reader.GetValue<StatusType>("status_type_id"),
						StatusTypeName = reader.GetValue<string>("status_type_name"),
						CreateDate = reader.GetValue<DateTime>("create_date"),
						CreateUserId = reader.GetValue<int>("create_user_id"),
						CreateUserName = reader.GetValue<string>("create_user_name"),
						GlobalTaskId = reader.GetValue<string>("global_task_id"),
						IsLastVersion = reader.GetValue<bool>("is_last_version"),
                        Reason = reader.GetValue<string>("reason")
                    });
			}
			return result;
		}
	}
}
