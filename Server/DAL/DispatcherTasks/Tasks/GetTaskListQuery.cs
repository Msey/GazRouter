using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DAL.DispatcherTasks.TaskStatuses;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.Tasks
{
    public class GetTaskListQuery : QueryReader<GetTaskListParameterSet, List<TaskDTO>>
    {
        public GetTaskListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetTaskListParameterSet parameters)
        {
            var sql = @"   SELECT       t.task_id, 
                                        t.task_number, 
                                        t.subject, 
                                        t.description, 
                                        t.last_version_id,
                                        t.global_task_id, 
                                        t.status_type_id AS task_status_type_id,
                                        t.status_type_name AS task_status_type_name, 
                                        t.status_type_code AS task_status_type_code,
                                        t.status_set_date AS task_status_type_date,

                                        t.create_date AS task_create_date, 
                                        t.create_user_id AS task_create_user_id, 
                                        t.create_user_name AS task_create_user_name,
                                        tu.description AS task_create_user_description,

                                        v.task_version_id,
                                        v.status_type_id,
                                        v.status_type_name,
                                        v.status_type_code,
                                        v.ack_date,
                                        v.ack_user_id,
                                        v.create_date,
                                        v.create_user_id,
                                        v.create_cpdd_user_name,                                        
                                        v.create_user_name,

                                        r.completion_date,
                                        r.executed_date
                            
                            FROM        v_tasks t
                            INNER JOIN  v_task_versions v ON t.task_id = v.task_id
                            INNER JOIN  v_users tu ON tu.user_id = t.create_user_id
                            LEFT JOIN   v_task_records r ON r.task_version_id = v.task_version_id 
                                AND     v.task_version_id = t.last_version_id
                            
                            WHERE       1=1";

            var sb = new StringBuilder(sql);

            if (parameters.SiteId.HasValue)
                sb.Append(" AND r.site_id = :site");


            if (!parameters.IsArchive)
                sb.Append(" AND t.status_type_id <> 7 AND t.status_type_id <> 8");


            if (parameters.IsArchive && parameters.PeriodStart.HasValue && parameters.PeriodEnd.HasValue)
            {
                sb.Append(" AND (t.status_type_id = 7 OR t.status_type_id = 8)");
                sb.Append(" AND t.status_set_date  BETWEEN :start_date AND :end_date");
            }

            sb.Append(" ORDER BY t.task_id, v.task_version_id");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetTaskListParameterSet parameters)
        {
            if (parameters.SiteId.HasValue)
                command.AddInputParameter("site", parameters.SiteId.Value);

            if (parameters.IsArchive && parameters.PeriodStart.HasValue && parameters.PeriodEnd.HasValue)
            {
                command.AddInputParameter("start_date", parameters.PeriodStart);
                command.AddInputParameter("end_date", parameters.PeriodEnd);
            }

        }

        protected override List<TaskDTO> GetResult(OracleDataReader reader, GetTaskListParameterSet parameters)
        {
            var result = new List<TaskDTO>();

            TaskDTO taskDto = null;
            TaskStatusDTO verDto = null;
            
            while (reader.Read())
            {
                var taskId = reader.GetValue<Guid>("task_id");
                if (taskId != taskDto?.Id)
                {
                    taskDto = new TaskDTO
                    {
                        Id = taskId,
                        TaskNumber = reader.GetValue<int>("task_number"),
                        LastVersionId = reader.GetValue<Guid>("last_version_id"),
                        Subject = reader.GetValue<string>("subject"),
                        Description = reader.GetValue<string>("description"),
                        GlobalTaskId = reader.GetValue<string>("global_task_id"),

                        StatusType = reader.GetValue<StatusType>("task_status_type_id"),
                        StatusSetDate = reader.GetValue<DateTime>("task_status_type_date"),
                        StatusTypeName = reader.GetValue<string>("task_status_type_name"),
                        StatusTypeCode = reader.GetValue<string>("task_status_type_code"),

                        CreateDate = reader.GetValue<DateTime>("task_create_date"),
                        CreateUserId = reader.GetValue<int>("task_create_user_id"),
                        CreateUserName = reader.GetValue<string>("task_create_user_name"),
                        CreateUserDescription = reader.GetValue<string>("task_create_user_description")
                    };

                    if (taskDto.StatusType == StatusType.ApprovedForSite)
                        taskDto.IsComplete = true;

                    result.Add(taskDto);
                }

                var verId = reader.GetValue<Guid>("task_version_id");
                if (verId != verDto?.Id)
                {
                    verDto = new TaskStatusDTO
                    {
                        Id = verId,
                        TaskId = taskId,
                        StatusTypeId = reader.GetValue<StatusType>("status_type_id"),
                        StatusTypeName = reader.GetValue<string>("status_type_name"),
                        CreateDate = reader.GetValue<DateTime>("create_date"),
                        CreateUserId = reader.GetValue<int>("create_user_id"),
                        CreateUserName = reader.GetValue<string>("create_user_name"),
                        GlobalTaskId = reader.GetValue<string>("global_task_id"),
                        IsLastVersion = verId == taskDto.LastVersionId
                        //Reason = reader.GetValue<string>("reason")
                    };
                    taskDto.StatusList.Add(verDto);
                }

                var planDate = reader.GetValue<DateTime?>("completion_date");
                var factDate = reader.GetValue<DateTime?>("executed_date");

                if (planDate.HasValue && !factDate.HasValue)
                    taskDto.IsComplete = false;

                if(taskDto.StatusType == StatusType.ApprovedForSite 
                    && (!factDate.HasValue && planDate < DateTime.Now))
                    taskDto.IsOverdue = true;
            }

            return result;
        }
    }
}
