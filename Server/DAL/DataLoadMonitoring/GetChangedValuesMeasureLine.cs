using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetChangedValuesMeasureLine : QueryReader<GasModeChangeParameterSet, List<MeasureLineGasFlowChangeDTO>>
    {
        public GetChangedValuesMeasureLine(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GasModeChangeParameterSet parameter)
        {
            #region query
            return @"
WITH
w_dt1 as
  ( select s.series_id      from rd.V_VALUE_SERIES s
    where s.period_type_id = 5       and s.key_date = :KeyDate1
  )
,w_dt2 as
( select s.series_id     from rd.V_VALUE_SERIES s
   where s.period_type_id = 5       and s.key_date = :KeyDate2
)
,w_vals1_gis as
(
    Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 27  
    Where   (v.series_id in (select series_id from W_DT1) )      and v.property_type_id = 10                   
)
,w_vals1_line as
(
    Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 16  
    Where   (v.series_id in (select series_id from W_DT1) )      and v.property_type_id = 10                   
)
,w_vals2_gis  as
(
     Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 27   
    Where   (v.series_id in (select series_id from W_DT2) )     and v.property_type_id = 10       
)

,w_vals2_line as
(
    Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 16   
    Where   (v.series_id in (select series_id from W_DT2) )  and v.property_type_id = 10                   
)
,w_line_list as
(
    Select  s.site_id,s.site_name ,ms.meas_station_id , ms.meas_station_name , ml.MEAS_LINE_ID, ml.MEAS_LINE_NAME         
    From RD.V_MEAS_LINES ml 
    Join  RD.v_meas_stations ms on ms.meas_station_id = ml.meas_station_id        
    Join rd.v_sites s on s.site_id = ms.site_id
)
  SELECT 
   ll.site_id,
   ll.site_name ,
   ll.meas_station_id ,
   ll.meas_station_name , 
   ll.MEAS_LINE_ID, 
   ll.MEAS_LINE_NAME        
  , v_gis1.value_num     gis1_value
  , v_gis2.value_num     gis2_value
  , v_line1.value_num   line1_value
  , v_line2.value_num   line2_value
 FROM w_line_list ll
    left outer join w_vals1_gis    v_gis1 on v_gis1.entity_id = ll.meas_station_id
    left outer join w_vals2_gis    v_gis2 on v_gis2.entity_id = ll.meas_station_id
    left outer join w_vals1_line  v_line1 on v_line1.entity_id = ll.MEAS_LINE_ID
    left outer join w_vals2_line  v_line2 on v_line2.entity_id = ll.MEAS_LINE_ID
 WHERE  (case  when   v_line2.value_num=0 and v_line1.value_num > 0   then 100 
                          when    v_line2.value_num=0 and v_line1.value_num = 0 then 0
                          else   abs(1 - (v_line1.value_num/v_line2.value_num )) * 100  end )               >= :DeltaQ
ORDER BY ll.site_name ,ll.meas_station_name ,  ll.MEAS_LINE_NAME 
"
            #endregion
;
        }

        protected override void BindParameters(OracleCommand command, GasModeChangeParameterSet parameters)
        {
            command.AddInputParameter("KeyDate1", parameters.KeyDate1);
            command.AddInputParameter("KeyDate2", parameters.KeyDate2);
            command.AddInputParameter("DeltaQ", parameters.QLimit);
        }
        protected override List<MeasureLineGasFlowChangeDTO> GetResult(OracleDataReader reader, GasModeChangeParameterSet parameters)
        {
            var result = new List<MeasureLineGasFlowChangeDTO>();
            while (reader.Read())
            {
                var gasConsumerValues = new MeasureLineGasFlowChangeDTO
                {
                    SiteId = reader.GetValue<Guid>("site_id"),
                    SiteName = reader.GetValue<string>("site_name"),
                    MeasLineId = reader.GetValue<Guid>("MEAS_LINE_ID"),
                    MeasLineName = reader.GetValue<string>("MEAS_LINE_NAME"),
                    MeasStationId = reader.GetValue<Guid>("meas_station_id"),
                    MeasStationName = reader.GetValue<string>("meas_station_name"),
                    Q = new ChangeModeValueDouble
                    {
                        Value = reader.GetValue<double?>("line2_value"),
                        PreviousValue = reader.GetValue<double?>("line1_value")
                    },
                    ParentQ = new ChangeModeValueDouble
                    {
                        Value = reader.GetValue<double?>("gis2_value"),
                        PreviousValue = reader.GetValue<double?>("gis1_value")
                    }
                };

                result.Add(gasConsumerValues);
            }
            return result;
        }
    }
}