using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
	public class GetTaskRecordsListQuery : QueryReader<GetTaskRecordsCpddParameterSet, List<TaskRecordDTO>>
	{
		public GetTaskRecordsListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(GetTaskRecordsCpddParameterSet parameters)
		{
            return @"   SELECT      record_id,
                                    tr.task_version_id,
                                    tr.entity_id,
                                    e.entity_type_id,
                                    property_type_id,
                                    period_type_id,
                                    target_value,
                                    completion_date,
                                    orderno,
                                    tr.description,
                                    executed_date,
                                    executed_user_id,
                                    eu.name AS executed_user_name,
                                    eu.description AS executed_user_description,

                                    special_control_user_name,
                                    special_control_user_id,
                                    is_special_control,
                                    is_cpdd_row,
                                    tr.create_date,
                                    tr.create_user_id,
                                    tr.create_user_name,
                                    cu.description AS create_user_description, 
                                    short.entity_name,
                                    property_type_name,
                                    phisical_type_id,
                                    dict_id,
                                    phisical_type_name,
                                    phisical_type_sys_name,
                                    notes_count,
                                    tr.site_id, 
                                    s.site_name,
                                    tv.task_id,

                                    tr.ack_date, 
                                    tr.ack_user_id,
                                    ack_u.name AS ack_user_name,
                                    ack_u.description AS ack_user_descr

                        FROM        v_task_records tr 
                        INNER JOIN  v_entities e ON e.entity_id = tr.entity_id      
                        LEFT JOIN   v_sites s ON tr.site_id = s.site_id
                        LEFT JOIN   v_nm_short_all short ON short.entity_id = tr.entity_id
                        LEFT JOIN   v_task_versions tv ON tr.task_version_id = tv.task_version_id
                        LEFT JOIN   v_users cu ON tr.create_user_id = cu.user_id
                        LEFT JOIN   v_users eu ON tr.executed_user_id = eu.user_id
                        LEFT JOIN   v_users ack_u ON tr.ack_user_id = ack_u.user_id
                        
                        WHERE       1=1
                            AND     tr.task_version_id = :id 
                            AND     is_cpdd_row= :iscpdd";
		}

		protected override void BindParameters(OracleCommand command, GetTaskRecordsCpddParameterSet parameters)
		{

			command.AddInputParameter("id", parameters.TaskVersionId);
			command.AddInputParameter("iscpdd", parameters.IsCpdd);
		}

	    protected override List<TaskRecordDTO> GetResult(OracleDataReader reader, GetTaskRecordsCpddParameterSet parameters)
	    {
	        var result = new List<TaskRecordDTO>();
	        while (reader.Read())
	        {
	            result.Add(new TaskRecordDTO
	            {
	                Id = reader.GetValue<Guid>("record_id"),
	                TaskVersionId = reader.GetValue<Guid>("task_version_id"),
                    Entity = new CommonEntityDTO
                    {
                        Id = reader.GetValue<Guid>("entity_id"),
                        Name = reader.GetValue<string>("entity_name"),
                        ShortPath = reader.GetValue<string>("entity_name"),
                        EntityType = reader.GetValue<EntityType>("entity_type_id")
                    },
	                
	                PropertyTypeId =
	                    reader.IsDBNull(reader.GetOrdinal("property_type_id"))
	                        ? PropertyType.None
	                        : reader.GetValue<PropertyType>("property_type_id"),
	                PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
	                TargetValue = reader.GetValue<string>("target_value"),
	                CompletionDate = reader.GetValue<DateTime>("completion_date"),
	                OrderNo = reader.GetValue<int>("orderno"),
	                Description = reader.GetValue<string>("description"),

	                ExecutedDate = reader.GetValue<DateTime?>("executed_date"),
	                ExecutedUserId = reader.GetValue<int>("executed_user_id"),
                    ExecutedUserName = reader.GetValue<string>("executed_user_name"),
                    ExecutedUserDescription = reader.GetValue<string>("executed_user_description"),

                    IsSpecialControl = reader.GetValue<bool>("is_special_control"),
	                SpecialControlUserName = reader.GetValue<string>("special_control_user_name"),
	                SpecialControlUserId = reader.GetValue<int?>("special_control_user_id"),

	                CreateDate = reader.GetValue<DateTime>("create_date"),
	                CreateUserId = reader.GetValue<int>("create_user_id"),
	                CreateUserName = reader.GetValue<string>("create_user_name"),
                    CreateUserDescription = reader.GetValue<string>("create_user_description"),

                    AckDate = reader.GetValue<DateTime?>("ack_date"),
                    AckUserId = reader.GetValue<int?>("ack_user_id"),
                    AckUserName = reader.GetValue<string>("ack_user_name"),
                    AckUserDescription = reader.GetValue<string>("ack_user_descr"),


                    PropertyTypeName = reader.GetValue<string>("property_type_name"),
	                PhysicalTypeId =
	                    reader.IsDBNull(reader.GetOrdinal("phisical_type_id"))
	                        ? PhysicalType.None
	                        : reader.GetValue<PhysicalType>("phisical_type_id"),
	                DictId = reader.GetValue<int>("dict_id"),
	                PhisicalTypeName = reader.GetValue<string>("phisical_type_name"),
	                PhisicalTypeSysName = reader.GetValue<string>("phisical_type_sys_name"),
	                NoteCount = reader.GetValue<int>("notes_count"),
	                SiteId = reader.GetValue<Guid>("site_id"),
	                SiteName = reader.GetValue<string>("site_name"),
	                TaskId = reader.GetValue<Guid>("task_id"),
	                
	            });
	        }
	        return result;
	    }
	}
}
