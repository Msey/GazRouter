using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Entities
{

    public class GetEntityChangeListQuery : QueryReader<Guid, List<EntityChangeDTO>>
    {
		public GetEntityChangeListQuery(ExecutionContext context)
            : base(context)
        { }

		protected override string GetCommandText(Guid parameters)
        {
            return @"   SELECT      t1.versions_time, 
                                    t1.versions_operation,
                                    t2.user_id,
                                    t2.name AS user_name
                        FROM        rd.v_entity_flb t1
                        JOIN        rd.v_users t2 ON t2.login = t1.upd_user_login
                        WHERE       t1.entity_id = :entityId
                        ORDER BY    t1.versions_time ASC";
        }

		protected override void BindParameters(OracleCommand command, Guid parameters)
        {
			command.AddInputParameter("entityId", parameters);
        }

        protected override List<EntityChangeDTO> GetResult(OracleDataReader reader, Guid parameters)
	    {
			var result = new List<EntityChangeDTO>();
	        while (reader.Read())
	        {
				result.Add(
                    new EntityChangeDTO
                    {
                        UserName = reader.GetValue<string>("user_name"),
                        Action = DecodeOperation(reader.GetValue<string>("versions_operation")),
                        Date = reader.GetValue<DateTime>("versions_time")
                    });
	        }
			return result;
	    }

        private static string DecodeOperation(string action)
        {
            switch (action)
            {
                case "I":
                    return "Добавлен";
                case "U":
                    return "Изменен";
                case "D":
                    return "Удален";
                default:
                    return "Не определенно";
            }
        }
    }
}
