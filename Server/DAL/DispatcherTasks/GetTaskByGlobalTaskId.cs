using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks
{
    public class GetTaskByGlobalTaskId : QueryReader<string, TaskDTO>
    {
        public GetTaskByGlobalTaskId(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, string parameters)
        {
            command.AddInputParameter("p1", parameters);
        }

        protected override string GetCommandText(string parameters)
        {
            return @"select t.task_id, t.task_number, t.subject, t.description, t.last_version_id,
                     t.global_task_id, t.status_type_id, t.status_set_date, 
                     t.create_date, t.create_user_id, t.create_user_name, t.status_type_name, t.status_type_code
                     from v_tasks t
                     where t.global_task_id = :p1";
        }

        protected override TaskDTO GetResult(OracleDataReader reader, string parameters)
        {
            TaskDTO result = null;
            while (reader.Read())
            {
                result = new TaskDTO
                             {
                                 Id = reader.GetValue<Guid>("task_id"),
                                 TaskNumber = reader.GetValue<int>("task_number"),
                                 LastVersionId = reader.GetValue<Guid>("last_version_id"),
                                 Subject = reader.GetValue<string>("subject"),
                                 Description = reader.GetValue<string>("description"),
                                 GlobalTaskId = reader.GetValue<string>("global_task_id"),
                                 StatusType = (StatusType)reader.GetValue<int>("status_type_id"),
                                 StatusSetDate = reader.GetValue<DateTime>("status_set_date"),
                                 CreateDate = reader.GetValue<DateTime>("create_date"),
                                 CreateUserId = reader.GetValue<int>("create_user_id"),
                                 CreateUserName = reader.GetValue<string>("create_user_name"),
                                 StatusTypeName = reader.GetValue<string>("status_type_name"),
                                 StatusTypeCode = reader.GetValue<string>("status_type_code")
                             };
            }
            return result;
        }
    }
}