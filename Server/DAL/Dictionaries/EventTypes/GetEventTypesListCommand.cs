using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.EventTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.EventTypes
{
    public class GetEventTypesListQuery : QueryReader<List<EventTypeDTO>>
    {
        public GetEventTypesListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return string.Format(@"   SELECT	    event_type_id
                                    ,name
                                    ,description
                        FROM 	    V_EVENT_TYPES "
              
                );
        }

        protected override List<EventTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<EventTypeDTO>();
            while (reader.Read())
            {
                var t = new EventTypeDTO
                    {
                        Id = reader.GetValue<int>("event_type_id"),
                        Name = reader.GetValue<string>("name"),
                        Description = reader.GetValue<string>("description"),
                    };
               
                result.Add(t);
            }
            return result;
        }
    }
}