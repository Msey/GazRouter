using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetChangedValuesConsumer : QueryReader<GasModeChangeParameterSet, List<ConsumerGasFlowChangeDTO>>
    {
        public GetChangedValuesConsumer(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GasModeChangeParameterSet parameter)
        {
            #region query
            return @"
WITH
w_dt1 as
  (select s.series_id      from rd.V_VALUE_SERIES s
    where s.period_type_id = 5       and s.key_date = :KeyDate1
  )
,w_dt2 as
(select s.series_id     from rd.V_VALUE_SERIES s
   where s.period_type_id = 5       and s.key_date = :KeyDate2
)
,w_vals1_cons as
(
    Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 3   
    Where   (v.series_id in (select series_id from W_DT1) )
                 and v.property_type_id = 10                   
)
,w_vals1_ds as
(
    Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 8   
    Where   (v.series_id in (select series_id from W_DT1) )
                 and v.property_type_id = 10                   
)
,w_vals2_cons  as
(
     Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 3   
    Where   (v.series_id in (select series_id from W_DT2) )
                 and v.property_type_id = 10       
)

,w_vals2_ds as
(
    Select v.series_id,v.entity_id, v.property_type_id, v.value_num
    From rd.V_PROPERTY_VALUES v    
    Join rd.v_entities  en on en.entity_id = v.entity_id and en.entity_type_id = 8   
    Where   (v.series_id in (select series_id from W_DT2) )
                 and v.property_type_id = 10                   
)
,w_ConsList  as
(
  Select  lpu.site_id,
            lpu.site_name,
            ds.distr_station_id, 
            ds.distr_station_name,
            cons.gas_consumer_id,
            cons.gas_consumer_name,
            cons.consumer_type_name,
            cons.region_id
From  
            rd.v_gas_consumers cons ,  rd.v_distr_stations ds,  rd.v_sites lpu
Where    cons.distr_station_id = ds.distr_station_id 
              and lpu.site_id  = ds.site_id 
)

SELECT 
            cl.site_id,
            cl.site_name,
            cl.distr_station_id, 
            cl.distr_station_name,
            cl.gas_consumer_id,
            cl.gas_consumer_name,
            cl.consumer_type_name,
 r.region_name
,  vcons1.value_num  potr_value1
,   vcons2.value_num potr_value2
, vds1.value_num       grs_value1
, vds2.value_num       grs_value2

FROM   w_ConsList cl
join rd.v_regions r on r.region_id = cl.region_id
left outer join w_vals1_cons vCons1 on vcons1.entity_id = cl.gas_consumer_id
left outer join w_vals2_cons vCons2 on vcons2.entity_id = cl.gas_consumer_id
left outer join w_vals1_ds  vds1 on vds1.entity_id = cl.distr_station_id
left outer join w_vals2_ds  vds2 on vds2.entity_id = cl.distr_station_id

WHERE (case  when   vcons2.value_num=0 and vcons1.value_num > 0   then 100 
             when    vcons2.value_num=0 and vcons1.value_num = 0 then 0
             else   abs(1 - (vcons1.value_num/vcons2.value_num )) * 100  end     )   >= :DeltaQ

ORDER BY cl.site_name, cl.distr_station_name,cl.gas_consumer_name
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
        protected override List<ConsumerGasFlowChangeDTO> GetResult(OracleDataReader reader, GasModeChangeParameterSet parameters)
        {
            var result = new List<ConsumerGasFlowChangeDTO>();
            while (reader.Read())
            {
                var gasConsumerValues = new ConsumerGasFlowChangeDTO
                {
                    SiteId = reader.GetValue<Guid>("site_id"),
                    SiteName = reader.GetValue<string>("site_name"),
                    ConsumerTypeName = reader.GetValue<string>("consumer_type_name"),
                    DistrStationId = reader.GetValue<Guid>("distr_station_id"),
                    DistrStationName = reader.GetValue<string>("distr_station_name"),
                    GasConsumerId = reader.GetValue<Guid>("gas_consumer_id"),
                    GasConsumerName = reader.GetValue<string>("gas_consumer_name"),
                    RegionName = reader.GetValue<string>("region_name"),
                    Q = new ChangeModeValueDouble
                    {
                        Value = reader.GetValue<double?>("potr_value2"),
                        PreviousValue = reader.GetValue<double?>("potr_value1")
                    },
                    ParentQ = new ChangeModeValueDouble
                    {
                        Value = reader.GetValue<double?>("grs_value2"),
                        PreviousValue = reader.GetValue<double?>("grs_value1")
                    }
                };

                result.Add(gasConsumerValues);
            }
            return result;
        }
        
    }
}





