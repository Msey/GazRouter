using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DispatcherTasks.RecordNotes
{
    public class GetRecordNoteListQuery : QueryReader<GetRecordNoteListParameterSet, List<RecordNoteDTO>>
	{
        public GetRecordNoteListQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(GetRecordNoteListParameterSet parameters)
        {
            var sql = @"    SELECT      rn.record_notes_id, 
                                        rn.task_id, 
                                        rn.note, 
                                        rn.create_date, 
                                        rn.create_user_id, 
                                        rn.create_user_name,
                                        u.description AS create_user_description, 
                                        rn.entity_id, 
                                        rn.property_type_id

                            FROM        v_record_notes rn
                            JOIN        v_users u ON u.user_id = rn.create_user_id 
                            WHERE       1=1
                                AND     rn.task_id = :taskId
                                AND     rn.entity_id = :entityId";

            var sb = new StringBuilder(sql);

            if (parameters.PropertyTypeId != DTO.Dictionaries.PropertyTypes.PropertyType.None)
                sb.Append(" AND rn.property_type_id = :propertyTypeId");

            return sb.ToString();


//            if (parameters.PropertyTypeId == DTO.Dictionaries.PropertyTypes.PropertyType.None)
//                return @"
//select rn.record_notes_id, rn.task_id, rn.note, rn.create_date, rn.create_user_id, rn.create_user_name, rn.entity_id, rn.property_type_id
//  from tasks.V_RECORD_NOTES rn 
// where (rn.entity_id,rn.task_id) in
//       (
//        select tr.entity_id,tv.task_id
//          from tasks.V_TASK_RECORDS tr
//          join tasks.V_TASK_VERSIONS tv on tv.task_version_id=tr.task_version_id
//             where tv.task_id = :taskId
//             and tr.entity_id = :entityId
            
//       )";
//            else
//                return @"
//            select rn.record_notes_id, rn.task_id, rn.note, rn.create_date, rn.create_user_id, rn.create_user_name, rn.entity_id, rn.property_type_id
//              from tasks.V_RECORD_NOTES rn 
//             where (rn.entity_id,rn.property_type_id,rn.task_id) in
//                   (
//                    select tr.entity_id,tr.property_type_id,tv.task_id
//                      from tasks.V_TASK_RECORDS tr
//                      join tasks.V_TASK_VERSIONS tv on tv.task_version_id=tr.task_version_id
//                         where tv.task_id = :taskId
//                         and tr.entity_id = :entityId
//                         and tr.property_type_id = :propertyTypeId 
//                   )";


        }

        protected override void BindParameters(OracleCommand command, GetRecordNoteListParameterSet parameters)
		{
            command.AddInputParameter("taskId", parameters.TaskId);
            command.AddInputParameter("entityId", parameters.EntityId);
            command.AddInputParameter("propertyTypeId", parameters.PropertyTypeId);
		}

        protected override List<RecordNoteDTO> GetResult(OracleDataReader reader, GetRecordNoteListParameterSet parameters)
		{
            var result = new List<RecordNoteDTO>();
			while (reader.Read())
			{
                result.Add(new RecordNoteDTO
                {
                    Id = reader.GetValue<Guid>("record_notes_id"),
                    Note = reader.GetValue<string>("note"),
					CreateDate = reader.GetValue<DateTime>("create_date"),
					CreateUserId = reader.GetValue<int>("create_user_id"),
					CreateUserName = reader.GetValue<string>("create_user_name"),
					CreateUserDescription = reader.GetValue<string>("create_user_description"),
                    TaskId = reader.GetValue<Guid>("task_id"),
                    EntityId = reader.GetValue<Guid>("entity_id"),
                    PropertyTypeId = reader.IsDBNull(reader.GetOrdinal("property_type_id")) ? -1 : reader.GetValue<int>("property_type_id")
                    //PropertyTypeId = reader.GetValue<int>("property_type_id")
                });
			}
			return result;
		}
	}
}
