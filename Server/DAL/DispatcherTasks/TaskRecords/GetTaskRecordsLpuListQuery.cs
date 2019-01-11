using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
	public class GetTaskRecordsLpuListQuery : QueryReader<GetTaskRecordsPdsParameterSet, List<TaskRecordPdsDTO>>
	{
		public GetTaskRecordsLpuListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(GetTaskRecordsPdsParameterSet parameters)
        {
            var sql = @"    SELECT      t.task_id, 
                                        t.task_number,
            
                                        tr.record_id, 
                                        tr.entity_id, 
                                        short.entity_name AS entity_name,
                                        e.entity_type_id,
                                        tr.property_type_id,
                                        tr.property_type_name,
                                        tr.target_value,
                                        tr.description,
                                        tr.completion_date,
                                    
                                        tr.ack_date, 
                                        tr.ack_user_id,
                                        ack_u.name AS ack_user_name,
                                        ack_u.description AS ack_user_descr,
            
                                        tr.executed_date,
                                        tr.executed_user_id,
                                        ex_u.name AS executed_user_name,
                                        ex_u.description AS executed_user_descr,
                        
                                        tv.create_user_name,
                                        cr_u.description AS create_user_descr,
            
                                        t.status_type_code,
                                        tr.is_special_control,
                                        tr.special_control_user_id,
                                        tr.special_control_user_name,
                                        sc_u.description AS special_control_user_descr
                              
                            FROM        v_tasks t
                            JOIN        v_task_records tr ON tr.task_version_id = t.last_version_id
                            JOIN        v_entities e ON e.entity_id = tr.entity_id
                            LEFT JOIN   v_nm_short_all short ON short.entity_id = tr.entity_id
                            LEFT JOIN   v_users ack_u ON ack_u.user_id = tr.ack_user_id
                            LEFT JOIN   v_users cr_u ON cr_u.user_id = tr.create_user_id
                            LEFT JOIN   v_users ex_u ON ex_u.user_id = tr.executed_user_id
                            LEFT JOIN   v_users sc_u ON sc_u.user_id = tr.special_control_user_id

                            JOIN        v_task_versions tv ON t.last_version_id = tv.task_version_id

                            WHERE       1=1
                                AND       tr.is_cpdd_row = 0
                                AND       t.status_type_id = 6";

            var sb = new StringBuilder(sql);

            if (parameters != null)
            {
                sb.Append(" AND tr.site_id = :siteid");
                sb.Append(parameters.IsArchive ? " AND tr.executed_date IS NOT NULL" : " AND tr.executed_date IS NULL");

                if (parameters.BeginDate.HasValue && parameters.EndDate.HasValue)
                    sb.Append(" AND (tr.completion_date BETWEEN :begin_date AND :end_date)");
                
            }
            return sb.ToString();

        }

	    protected override void BindParameters(OracleCommand command, GetTaskRecordsPdsParameterSet parameters)
		{
			command.AddInputParameter("siteid", parameters.SiteId);

	        if (parameters.BeginDate.HasValue && parameters.EndDate.HasValue)
	        {
	            command.AddInputParameter("begin_date", parameters.BeginDate);
	            command.AddInputParameter("end_date", parameters.EndDate);
	        }
		}

        protected override List<TaskRecordPdsDTO> GetResult(OracleDataReader reader, GetTaskRecordsPdsParameterSet parameters)
		{
			var result = new List<TaskRecordPdsDTO>();
			while (reader.Read())
			{
				result.Add(
                    new TaskRecordPdsDTO
				    {
                        TaskId = reader.GetValue<Guid>("task_id"),
                        TaskNum = reader.GetValue<int>("task_number"),
                        Id = reader.GetValue<Guid>("record_id"),

                        Entity = new CommonEntityDTO
                        {
                            Id = reader.GetValue<Guid>("entity_id"),
                            Name = reader.GetValue<string>("entity_name"),
                            ShortPath = reader.GetValue<string>("entity_name"),
                            EntityType = reader.GetValue<EntityType>("entity_type_id"),
                        },

                        PropertyTypeId = reader.IsDBNull(reader.GetOrdinal("property_type_id")) ? PropertyType.None : reader.GetValue<PropertyType>("property_type_id"),
                        PropertyTypeName = reader.GetValue<string>("property_type_name"),
                        TargetValue = reader.GetValue<string>("target_value"),
                        Description = reader.GetValue<string>("description"),
                        CompletionDate = reader.GetValue<DateTime?>("completion_date"),

                        AckDate = reader.GetValue<DateTime?>("ack_date"),
                        AckUserId = reader.GetValue<int>("ack_user_id"),
                        AckUserName = reader.GetValue<string>("ack_user_name"),
                        AckUserDescription = reader.GetValue<string>("ack_user_descr"),

                        ExecutedDate = reader.GetValue<DateTime?>("executed_date"),
                        ExecutedUserName = reader.GetValue<string>("executed_user_name"),
                        ExecutedUserDescription = reader.GetValue<string>("executed_user_descr"),

                        AproveUserName = reader.GetValue<string>("create_user_name"),
                        AproveUserDescription = reader.GetValue<string>("create_user_descr"),
					    
					    Status = reader.GetValue<string>("status_type_code"),
					    
					    IsSpecialControl = reader.GetValue<bool>("is_special_control"),
                        SpecialControlUserId = reader.GetValue<int?>("special_control_user_id"),
                        SpecialControlUserName = reader.GetValue<string>("special_control_user_name"),
                        SpecialControlUserDescription = reader.GetValue<string>("special_control_user_descr"),
                    });
                
            }
            return result;
        }
    }
}
