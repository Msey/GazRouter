using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
    public class GetNonAckRecordsQuery : QueryReader<GetTaskListParameterSet, List<TaskRecordPdsDTO>>
    {
        public GetNonAckRecordsQuery(ExecutionContext context)
			: base(context)
		{
        }

        protected override string GetCommandText(GetTaskListParameterSet parameters)
        {

            var sql = @"    SELECT      t3.record_id, 
                                        t1.Task_id,
                                        t1.task_number, 
                                        short.entity_name AS object_name,
                                        t3.description, 
                                        t3.completion_date, 
                                        t3.property_type_name, 
                                        t3.target_value, 
                                        t3.executed_date,
                                        t4.create_user_name, 
                                        t3.entity_id, 
                                        t3.property_type_id,
                                        t3.ack_date, 
                                        t3.ack_user_id,
                                        t1.status_type_code,
                                        t3.is_special_control,
                                        t3.special_control_user_name,
                                        t3.special_control_user_id 

  from V_TASKS t1
  join V_TASK_RECORDS t3 on t3.task_version_id=t1.last_version_id
left join V_NM_SHORT_ALL short on short.entity_id=t3.entity_id
join V_TASK_versions t4 on t1.last_version_id = t4.task_version_id
	where 1=1
	   and t3.is_cpdd_row=0 --строка НЕ ЦПДД
       and t3.ack_date is null
	   and t1.status_type_code in ('APPROVEDFORSITE')
	   and t3.site_id= :site_id
	";



            if (parameters.IsEnterprise)
            {
                return
                @"  select
        t3.Record_id, 
        t1.Task_id, t1.task_number, 
        short.ENTITY_NAME object_name,
        t3.description, t3.COMPLETION_DATE, t3.property_type_name, t3.target_value, t3.executed_date,
        t4.Create_user_name, 
        t3.entity_id, t3.property_type_id,
        t3.ack_date, t3.ack_user_id

,null perf_annul_date
,'' perf_annul_user_name
,t1.status_type_code
,IS_SPECIAL_CONTROL
,SPECIAL_CONTROL_USER_NAME
,special_control_user_id

  from V_TASKS t1
  join V_TASK_RECORDS t3 on t3.task_version_id = t1.last_version_id
left join V_NM_SHORT_ALL short on short.entity_id = t3.entity_id
join V_TASK_versions t4 on t1.last_version_id = t4.task_version_id
  where 1 = 1
     and t3.is_cpdd_row = 0--строка НЕ ЦПДД
      and t3.ack_date is null
      and t1.status_type_code in ('PERFORMED')
   ";
            }
            else

                return
           @"select 
        t3.Record_id, 
        t1.Task_id, t1.task_number, 
        short.ENTITY_NAME object_name,
        t3.description, t3.COMPLETION_DATE, t3.property_type_name, t3.target_value, t3.executed_date,
        t4.Create_user_name, 
        t3.entity_id, t3.property_type_id,
        t3.ack_date, t3.ack_user_id

,null perf_annul_date
,'' perf_annul_user_name
,t1.status_type_code
,IS_SPECIAL_CONTROL
,SPECIAL_CONTROL_USER_NAME
,special_control_user_id 

  from V_TASKS t1
  join V_TASK_RECORDS t3 on t3.task_version_id=t1.last_version_id
left join V_NM_SHORT_ALL short on short.entity_id=t3.entity_id
join V_TASK_versions t4 on t1.last_version_id = t4.task_version_id
	where 1=1
	   and t3.is_cpdd_row=0 --строка НЕ ЦПДД
       and t3.ack_date is null
	   and t1.status_type_code in ('APPROVEDFORSITE')
	   and t3.site_id= :site_id
	";
        }

        protected override void BindParameters(OracleCommand command, GetTaskListParameterSet parameters)
        {
            command.AddInputParameter("site_id", parameters.SiteId);
        }

        protected override List<TaskRecordPdsDTO> GetResult(OracleDataReader reader, GetTaskListParameterSet parameters)
        {
           var result = new List<TaskRecordPdsDTO>();
            while (reader.Read())
            {
                result.Add(new TaskRecordPdsDTO
                {
                    Id = reader.GetValue<Guid>("RECORD_ID"),
                    TaskId = reader.GetValue<Guid>("Task_id"),
                    Entity = new CommonEntityDTO
                    {
                        Name = reader.GetValue<string>("object_name"),
                        ShortPath = reader.GetValue<string>("object_name"),
                        Id = reader.GetValue<Guid>("entity_id"),
                        EntityType = EntityType.Aggregator
                    },
                    Description = reader.GetValue<string>("description"),
                    PropertyTypeName = reader.GetValue<string>("property_type_name"),
                    TargetValue = reader.GetValue<string>("target_value"),
                    PerfDateTime = reader.GetValue<DateTime?>("perf_annul_date"),
                    PerfUser = reader.GetValue<string>("perf_annul_user_name"),
                    ExecutedDate = reader.GetValue<DateTime?>("executed_date"),
                    AproveUserName = reader.GetValue<string>("Create_user_name"),
                    TaskNum = reader.GetValue<int>("task_number"),
                    Status = reader.GetValue<string>("status_type_code"),
                    
                    PropertyTypeId = reader.IsDBNull(reader.GetOrdinal("property_type_id")) ? PropertyType.None : reader.GetValue<PropertyType>("property_type_id"),
                    CompletionDate = reader.GetValue<DateTime?>("COMPLETION_DATE"),
                    IsSpecialControl = reader.GetValue<bool>("IS_SPECIAL_CONTROL"),
                    SpecialControlUserName = reader.GetValue<string>("SPECIAL_CONTROL_USER_NAME"),
                    SpecialControlUserId = reader.GetValue<int?>("special_control_user_id"),

                    AckDate = reader.GetValue<DateTime?>("ack_date"),
                    AckUserId = reader.GetValue<int>("ack_user_id"),
                });
            }
            return result;
        }
    }
}
