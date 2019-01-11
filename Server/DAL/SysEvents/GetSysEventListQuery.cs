using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SysEvents;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SysEvents
{
    public class GetSysEventListQuery : QueryReader<GetSysEventListParameters, List<SysEventDTO>>
    {
        public GetSysEventListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetSysEventListParameters parameters)
        {
            string query = "";
            if(parameters.EventTypeId.HasValue)
                query += string.Format(" and e.event_type_id = :eventTypeId");
            if(parameters.EventStatusId.HasValue)
                query += string.Format(" and e.status_id_mii = :statusId");
            if (parameters.CreateDate.HasValue)
                query += string.Format(" and e.createdate >= :createDate");
            return string.Format(@"   
                    select e.event_id, e.event_type_id, e.series_id, ser.period_type_id, ser.key_date, e.entity_id, e.description, 
                e.createuser, e.createdate, e.status_id, e.result_id, e.status_id_mii, 
                e.result_id_mii from v_sys_ev e 
                
                join v_value_series ser on e.series_id = ser.series_id
                where 1=1 {0}", query);
        }

        protected override void BindParameters(OracleCommand command, GetSysEventListParameters parameters)
        {
            if (parameters.EventTypeId.HasValue)
            {
                command.AddInputParameter("eventTypeId", (int) parameters.EventTypeId);
            }
            if (parameters.EventStatusId.HasValue)
            {
                command.AddInputParameter("statusId", (int) parameters.EventStatusId);
            }
            if (parameters.CreateDate.HasValue)
            {
                command.AddInputParameter("createDate", (DateTime) parameters.CreateDate);
            }

        }

        protected override List<SysEventDTO> GetResult(OracleDataReader reader, GetSysEventListParameters parameters)
        {
            var result = new List<SysEventDTO>();
            while (reader.Read())
            {
                var t = new SysEventDTO
                        {
                            Id = reader.GetValue<Guid>("event_id"),
                            EventTypeId = reader.GetValue<int>("event_type_id"),
                            StatusId = reader.GetValue<SysEventStatus?>("status_id_mii"),
                            ResultId = reader.GetValue<SysEventResult?>("result_id_mii"),
                            SeriesId = reader.GetValue<int>("series_id"),
                            PeriodTypeId = reader.GetValue<PeriodType?>("period_type_id"),
                            KeyDate = reader.GetValue<DateTime>("key_date"),
                            EntityId = reader.GetValue<Guid>("entity_id"),
                            CreateDate = reader.GetValue<DateTime>("createdate"),
                            Description = reader.GetValue<string>("description"),
                        };
                result.Add(t);
            }
            return result;
        }
    }
}