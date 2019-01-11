using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.StatesModel;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.CompUnits
{
    public class GetOperatingTimeQuery : QueryReader<DateIntervalParameterSet, Dictionary<Guid, Dictionary<CompUnitState, List<DateIntervalDTO>>>>
    {
        public GetOperatingTimeQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, DateIntervalParameterSet parameter)
        {
            command.AddInputParameter("beginDate", parameter.BeginDate);
            command.AddInputParameter("endDate", parameter.EndDate);
        }

        protected override string GetCommandText(DateIntervalParameterSet parameters)
        {
            return @"   WITH vs as (
                            SELECT  v.series_id, 
                                    v.key_date 
                            FROM    v_value_series v
                            WHERE   v.key_date BETWEEN :beginDate AND :endDate
                                AND v.period_type_id = 5)

                        SELECT      pv.entity_id, 
                                    vs.key_date, 
                                    pv.value_num
                        FROM        vs
                        LEFT JOIN   v_property_values pv ON pv.entity_id IN 
                                    (   SELECT  t1.comp_unit_id 
                                        FROM    rd.v_comp_units t1) 
                            AND     pv.property_type_id = 43 
                            AND     vs.series_id = pv.series_id
                        ORDER BY    pv.entity_id, vs.key_date
";
        }

        protected override Dictionary<Guid, Dictionary<CompUnitState, List<DateIntervalDTO>>> GetResult(OracleDataReader reader, DateIntervalParameterSet parameter)
        {
            var compUnitId = new Guid();
            var compUnits = new Dictionary<Guid, Dictionary<CompUnitState, List<DateIntervalDTO>>>();
            CompUnitState? state = null;
            Dictionary<CompUnitState, List<DateIntervalDTO>> states = null;
            DateTime? beginDate = null;
            while (reader.Read())
            {
                var compUnitId1 = reader.GetValue<Guid>("entity_id");
                if (compUnitId1 != compUnitId)
                {
                    compUnitId = compUnitId1;
                    states = new Dictionary<CompUnitState, List<DateIntervalDTO>>();
                    compUnits.Add(compUnitId, states);
                    state = null;
                    beginDate = null;
                }
                var state1 = reader.GetValue<CompUnitState?>("value_num");
                if (!state1.HasValue) continue;
                if (!state.HasValue || state.Value != state1)
                {
                    state = state1;
                    beginDate = null;

                    List<DateIntervalDTO> intervals;
                    if (!states.ContainsKey(state1.Value))
                    {
                        intervals = new List<DateIntervalDTO>();
                        states.Add(state.Value, intervals);
                    }
                    else
                    {
                        intervals = states[state1.Value];
                    }

                    var interval = CreateInterval(reader, ref beginDate);
                    intervals.Add(interval);
                }
                else
                {
                    var interval = CreateInterval(reader, ref beginDate);
                    var intervals = states[state1.Value];
                    intervals[intervals.Count - 1] = interval;
                }
            }
            return compUnits;
        }

        private static DateIntervalDTO CreateInterval(OracleDataReader reader, ref DateTime? keyDate)
        {
            var date = reader.GetValue<DateTime>("key_date");
            var beginDate = keyDate ?? date;
            keyDate = beginDate;
            return new DateIntervalDTO { BeginDate = beginDate, EndDate = date.AddHours(2) };
        }
    }
}
