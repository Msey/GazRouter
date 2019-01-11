using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.SysEventTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.SysEventTypes
{
    public class GetSysEventTypesListQuery : QueryReader<List<SysEventTypeDTO>>
    {
        public GetSysEventTypesListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return string.Format(@"   SELECT	    event_type_id
                                    ,event_type_name
                                    ,description
                                    ,sys_name
                                    ,sort_order

                        FROM 	    V_SYS_EV_TYPES "
                );
        }

        protected override List<SysEventTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<SysEventTypeDTO>();
            while (reader.Read())
            {
                var t = new SysEventTypeDTO
                {
                    Id = reader.GetValue<int>("event_type_id"),
                    Name = reader.GetValue<string>("event_type_name"),
                    SysName = reader.GetValue<string>("sys_name"),
                    Description = reader.GetValue<string>("description"),
                };
                result.Add(t);
            }
            return result;
        }
    }
}