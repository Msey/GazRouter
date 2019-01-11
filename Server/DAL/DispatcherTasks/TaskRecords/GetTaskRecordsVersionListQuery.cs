using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.TaskRecords
{
	public class GetTaskRecordsVersionListQuery : QueryReader<Guid, List<TaskRecordDTO>>
	{
		public GetTaskRecordsVersionListQuery(ExecutionContext context)
			: base(context)
		{
		}

		protected override string GetCommandText(Guid parameters)
		{


			return @"select RECORD_ID,tr.TASK_VERSION_ID,ENTITY_ID,PROPERTY_TYPE_ID,PERIOD_TYPE_ID,
					TARGET_VALUE,COMPLETION_DATE,ORDERNO ,tr.DESCRIPTION ,EXECUTED_DATE,EXECUTED_USER_ID,us.Name EXECUTED_USER_Name,SPECIAL_CONTROL_USER_NAME,special_control_user_id,
					IS_SPECIAL_CONTROL,IS_CPDD_ROW,tr.CREATE_DATE,tr.CREATE_USER_ID,tr.CREATE_USER_NAME, OBJECT_NAME EntityName,
					PROPERTY_TYPE_NAME,PHISICAL_TYPE_ID,DICT_ID,PHISICAL_TYPE_NAME,PHISICAL_TYPE_SYS_NAME,NOTES_COUNT,tr.SITE_ID, s.site_name,tv.Task_id
                     from V_TASK_RECORDS tr left join V_SITES s on tr.SITE_ID = s.SITE_ID
left join V_TASK_versions tv on TR.TASK_VERSION_ID = tv.task_version_id
left join v_users us on tr.EXECUTED_USER_ID = us.User_id
where tr.TASK_VERSION_ID = (SELECT TASK_VERSION_ID FROM V_TASK_RECORDS WHERE RECORD_ID = :id)";
		}

		protected override void BindParameters(OracleCommand command, Guid parameters)
		{

			command.AddInputParameter("id", parameters);
		}

        protected override List<TaskRecordDTO> GetResult(OracleDataReader reader, Guid parameters)
		{
			var result = new List<TaskRecordDTO>();
			while (reader.Read())
			{
				result.Add(new TaskRecordDTO
					           {
						           Id = reader.GetValue<Guid>("RECORD_ID"),
                                   TaskVersionId = reader.GetValue<Guid>("TASK_VERSION_ID"),
                                   Entity = new CommonEntityDTO
                                   {
                                       Id = reader.GetValue<Guid>("ENTITY_ID"),
                                       Name = reader.GetValue<string>("EntityName"),
                                   },
						           
                                   PropertyTypeId = reader.IsDBNull(reader.GetOrdinal("PROPERTY_TYPE_ID")) ? PropertyType.None : reader.GetValue<PropertyType>("PROPERTY_TYPE_ID"),
                    //reader.GetValue<PropertyType>("PROPERTY_TYPE_ID"),
								   PeriodTypeId = reader.GetValue<PeriodType>("PERIOD_TYPE_ID"),
						           TargetValue = reader.GetValue<string>("TARGET_VALUE"),
						           CompletionDate = reader.GetValue<DateTime>("COMPLETION_DATE"),
						           OrderNo = reader.GetValue<int>("ORDERNO"),
						           Description = reader.GetValue<string>("description"),
						           ExecutedDate = reader.GetValue<DateTime?>("EXECUTED_DATE"),
						           ExecutedUserId = reader.GetValue<int>("EXECUTED_USER_ID"),
                                   SpecialControlUserName = reader.GetValue<string>("SPECIAL_CONTROL_USER_NAME"),
                                   SpecialControlUserId = reader.GetValue<int?>("special_control_user_id"),
                                   IsSpecialControl = reader.GetValue<bool>("IS_SPECIAL_CONTROL"),
						           CreateDate = reader.GetValue<DateTime>("create_date"),
						           CreateUserId = reader.GetValue<int>("create_user_id"),
						           CreateUserName = reader.GetValue<string>("create_user_name"),
								   
						           PropertyTypeName = reader.GetValue<string>("PROPERTY_TYPE_NAME"),
                                   PhysicalTypeId = reader.IsDBNull(reader.GetOrdinal("PHISICAL_TYPE_ID")) ? PhysicalType.None : reader.GetValue<PhysicalType>("PHISICAL_TYPE_ID"),
                                   DictId = reader.GetValue<int>("DICT_ID"),
						           PhisicalTypeName = reader.GetValue<string>("PHISICAL_TYPE_NAME"),
						           PhisicalTypeSysName = reader.GetValue<string>("PHISICAL_TYPE_SYS_NAME"),
						           NoteCount = reader.GetValue<int>("NOTES_COUNT"),
								   SiteId = reader.GetValue<Guid>("SITE_ID"),
                                   SiteName = reader.GetValue<string>("site_name"),
								   TaskId = reader.GetValue<Guid>("Task_id"),
								   ExecutedUserName = reader.GetValue<string>("EXECUTED_USER_Name"),
					           });
			}
			return result;
		}
	}
}
