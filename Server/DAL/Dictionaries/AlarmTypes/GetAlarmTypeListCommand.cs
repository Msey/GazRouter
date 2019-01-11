using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.AlarmTypes
{
    public class GetAlarmTypeListQuery : QueryReader<List<AlarmTypeDTO>>
    {
        public GetAlarmTypeListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return string.Format(@" SELECT      alarm_type_id,
                                                alarm_type_name,
                                                description
                                    FROM 	    v_alr_alarm_types
                                    ORDER BY    sort_order");
        }

        protected override List<AlarmTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<AlarmTypeDTO>();
            while (reader.Read())
            {
                var t = new AlarmTypeDTO
                {
                    Id = reader.GetValue<int>("alarm_type_id"),
                    Name = reader.GetValue<string>("alarm_type_name"),
                    Description = reader.GetValue<string>("description"),
                };
               
                result.Add(t);
            }
            return result;
        }
    }
}