using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Alarms;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Alarms
{
    public class GetAlarmEventListQuery : QueryReader<int, List<AlarmEventDTO>>
    {
        public GetAlarmEventListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(int parameters)
        {
            return @"   SELECT      alarm_id,
                                    key_date,
                                    value,
                                    status
                        FROM        v_alr_events
                        WHERE       alarm_id = :alarmid
                        ORDER BY    key_date DESC";
                
        }


        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("alarmid", parameters);
        }


        protected override List<AlarmEventDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<AlarmEventDTO>();
            while (reader.Read())
            {
                var strVal = reader.GetValue<string>("value");
                var val = !string.IsNullOrEmpty(strVal) ? double.Parse(strVal) : 0;

                result.Add(new AlarmEventDTO
                {
                    Id = reader.GetValue<int>("alarm_id"),
                    Timestamp = reader.GetValue<DateTime>("key_date"),
                    PropertyValue = val,
                    Status = reader.GetValue<bool>("status")
                });
            }
            return result;
        }
    }
}
