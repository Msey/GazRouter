using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetChangedValuesCompShop : QueryReader<GasModeChangeParameterSet, List<CompressorShopValuesChangeDTO>>
    {
        public GetChangedValuesCompShop(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(GasModeChangeParameterSet parameter)
        {
            return @"
WITH
w_dt1 as
  ( SELECT s.series_id      
    FROM   rd.V_VALUE_SERIES s
    WHERE s.period_type_id = 5       and s.key_date = :KeyDate1
  )
,w_dt2 as
  ( SELECT s.series_id      
    FROM   rd.V_VALUE_SERIES s
    WHERE s.period_type_id = 5       and s.key_date = :KeyDate2
  )  

,w_COMP_SHOPS as
(  SELECT t1.comp_shop_id,t1.comp_shop_name , t1.comp_station_id
    FROM rd.V_COMP_SHOPS t1
)

,w_VALVS20_dt1 as
( SELECT t1.valve_id,t1.valve_name,t1.comp_shop_id ,v1.value_num  valve_Pressure_Inlet ,v2.value_num  valve_Pressure_Outlet  ,v3.value_num valve_Temperature_Inlet  ,v4.value_num valve_Temperature_Outlet  ,v5.value_num  valve_State
 ,v6.value_num Statebypass1 ,  v7.value_num Statebypass2 ,   v7.value_num Statebypass3
  FROM  rd.V_VALVES t1
    join w_COMP_SHOPS t2 on t2.comp_shop_id=t1.comp_shop_id
    left join rd.V_PROPERTY_VALUES_EXT v1 on v1.entity_id=t1.valve_id and v1.series_id in  (select series_id from w_dt1)  and  v1.property_type_id=11
    left join rd.V_PROPERTY_VALUES_EXT v2 on v2.entity_id=t1.valve_id and v2.series_id  in (select series_id from w_dt1)   and v2.property_type_id=12
    left join rd.V_PROPERTY_VALUES_EXT v3 on v3.entity_id=t1.valve_id and v3.series_id  in (select series_id from w_dt1)   and v3.property_type_id=13
    left join rd.V_PROPERTY_VALUES_EXT v4 on v4.entity_id=t1.valve_id and v4.series_id  in (select series_id from w_dt1)   and v4.property_type_id=14
    left join rd.V_PROPERTY_VALUES_EXT v5 on v5.entity_id=t1.valve_id and v5.series_id  in (select series_id from w_dt1)   and v5.property_type_id=45
    
    left join rd.V_PROPERTY_VALUES_EXT v6  on v6.entity_id=t1.valve_id and v6.series_id  in (select series_id from w_dt1)   and v6.property_type_id=46
    left join rd.V_PROPERTY_VALUES_EXT v7  on v7.entity_id=t1.valve_id and v7.series_id  in (select series_id from w_dt1)   and v7.property_type_id=47
    left join rd.V_PROPERTY_VALUES_EXT v8  on v8.entity_id=t1.valve_id and v8.series_id  in (select series_id from w_dt1)   and v8.property_type_id=48
  WHERE t1.valve_purpose_id=20
)
,
w_SHOP_dt1 as
(    SELECT  t1.comp_station_id,
          t1.comp_shop_id,
          t1.comp_shop_name
         ,v1.value_num cshop_Pressure_Inlet
         ,v2.value_num cshop_Pressure_Outlet
         ,v3.value_num cshop_Temperature_Inlet
         ,v4.value_num cshop_Temperature_Outlet
        ,v5.value_num cshop_Temperature_Cooling
        ,v6.value_str cshop_Scheme
        ,t2.valve_id,
       t2.valve_name,
       t2.valve_Pressure_Inlet,
       t2.valve_Pressure_Outlet
      ,t2.valve_Temperature_Inlet
      ,t2.valve_Temperature_Outlet
      ,t2.valve_State
      ,t2.Statebypass1
      ,t2.Statebypass2 
      ,t2.Statebypass3
   FROM w_COMP_SHOPS t1
   left join rd.V_PROPERTY_VALUES_EXT v1 on v1.entity_id=t1.comp_shop_id and v1.series_id  in (select series_id from w_dt1)  and v1.property_type_id=11
   left join rd.V_PROPERTY_VALUES_EXT v2 on v2.entity_id=t1.comp_shop_id and v2.series_id  in (select series_id from w_dt1)  and v2.property_type_id=12
   left join rd.V_PROPERTY_VALUES_EXT v3 on v3.entity_id=t1.comp_shop_id and v3.series_id  in (select series_id from w_dt1)  and v3.property_type_id=13
   left join rd.V_PROPERTY_VALUES_EXT v4 on v4.entity_id=t1.comp_shop_id and v4.series_id  in (select series_id from w_dt1)  and v4.property_type_id=14
   left join rd.V_PROPERTY_VALUES_EXT v5 on v5.entity_id=t1.comp_shop_id and v5.series_id  in (select series_id from w_dt1)  and v5.property_type_id=27
   left join rd.V_PROPERTY_VALUES_EXT v6 on v6.entity_id=t1.comp_shop_id and v6.series_id  in (select series_id from w_dt1)  and v6.property_type_id=38
   left join w_VALVS20_dt1 t2 on t2.comp_shop_id=t1.comp_shop_id
   )
   
   ,w_VALVS20_dt2 as
(SELECT t1.valve_id,t1.valve_name,t1.comp_shop_id ,v1.value_num valve_Pressure_Inlet ,v2.value_num valve_Pressure_Outlet  ,v3.value_num valve_Temperature_Inlet  ,v4.value_num valve_Temperature_Outlet  ,v5.value_num valve_State 
       ,v6.value_num Statebypass1 ,  v7.value_num Statebypass2 ,   v7.value_num Statebypass3
    from rd.V_VALVES t1
    join w_COMP_SHOPS t2 on t2.comp_shop_id=t1.comp_shop_id
    left join rd.V_PROPERTY_VALUES_EXT v1 on v1.entity_id=t1.valve_id and v1.series_id in  (select series_id from w_dt2)  and  v1.property_type_id=11
    left join rd.V_PROPERTY_VALUES_EXT v2 on v2.entity_id=t1.valve_id and v2.series_id  in (select series_id from w_dt2)   and v2.property_type_id=12
    left join rd.V_PROPERTY_VALUES_EXT v3 on v3.entity_id=t1.valve_id and v3.series_id  in (select series_id from w_dt2)   and v3.property_type_id=13
    left join rd.V_PROPERTY_VALUES_EXT v4 on v4.entity_id=t1.valve_id and v4.series_id  in (select series_id from w_dt2)   and v4.property_type_id=14
    left join rd.V_PROPERTY_VALUES_EXT v5 on v5.entity_id=t1.valve_id and v5.series_id  in (select series_id from w_dt2)   and v5.property_type_id=45
      
      left join rd.V_PROPERTY_VALUES_EXT v6  on v6.entity_id=t1.valve_id and v6.series_id  in (select series_id from w_dt2)   and v6.property_type_id=46
      left join rd.V_PROPERTY_VALUES_EXT v7  on v7.entity_id=t1.valve_id and v7.series_id  in (select series_id from w_dt2)   and v7.property_type_id=47
      left join rd.V_PROPERTY_VALUES_EXT v8  on v8.entity_id=t1.valve_id and v8.series_id  in (select series_id from w_dt2)   and v8.property_type_id=48
        
   WHERE  t1.valve_purpose_id=20
)
, w_SHOP_dt2 as
(    SELECT t1.comp_shop_id,
          t1.comp_shop_name
         ,v1.value_num cshop_Pressure_Inlet
         ,v2.value_num cshop_Pressure_Outlet
         ,v3.value_num cshop_Temperature_Inlet
         ,v4.value_num cshop_Temperature_Outlet
        ,v5.value_num cshop_Temperature_Cooling
        ,v6.value_str cshop_Scheme
        ,t2.valve_id,
       t2.valve_name,
       t2.valve_Pressure_Inlet,
       t2.valve_Pressure_Outlet
      ,t2.valve_Temperature_Inlet
      ,t2.valve_Temperature_Outlet
      ,t2.valve_State
      ,t2.Statebypass1
      ,t2.Statebypass2 
      ,t2.Statebypass3
   FROM w_COMP_SHOPS t1
   left join rd.V_PROPERTY_VALUES_EXT v1 on v1.entity_id=t1.comp_shop_id and v1.series_id  in (select series_id from w_dt2)  and v1.property_type_id=11
   left join rd.V_PROPERTY_VALUES_EXT v2 on v2.entity_id=t1.comp_shop_id and v2.series_id  in (select series_id from w_dt2)  and v2.property_type_id=12
   left join rd.V_PROPERTY_VALUES_EXT v3 on v3.entity_id=t1.comp_shop_id and v3.series_id  in (select series_id from w_dt2)  and v3.property_type_id=13
   left join rd.V_PROPERTY_VALUES_EXT v4 on v4.entity_id=t1.comp_shop_id and v4.series_id  in (select series_id from w_dt2)  and v4.property_type_id=14
   left join rd.V_PROPERTY_VALUES_EXT v5 on v5.entity_id=t1.comp_shop_id and v5.series_id  in (select series_id from w_dt2)  and v5.property_type_id=27
   left join rd.V_PROPERTY_VALUES_EXT v6 on v6.entity_id=t1.comp_shop_id and v6.series_id  in (select series_id from w_dt2)  and v6.property_type_id=38
   left join w_VALVS20_dt2 t2 on t2.comp_shop_id=t1.comp_shop_id
   )
 
 
   SELECT
    l.site_id
    ,l.site_name
    ,t1.comp_station_id
		,s.comp_station_name 
		,t1.comp_shop_id
		,t1.comp_shop_name
		,t1.valve_id
		,t1.valve_name
		,t1.cshop_Pressure_Inlet cshop_Pressure_Inlet1
		,t1.cshop_Pressure_Outlet cshop_Pressure_Outlet1
		,t1.cshop_Temperature_Inlet cshop_Temperature_Inlet1
		,t1.cshop_Temperature_Outlet cshop_Temperature_Outlet1
		,t1.cshop_Temperature_Cooling cshop_Temperature_Cooling1
		,t1.cshop_Scheme       cshop_Scheme1
		,t1.valve_Pressure_Inlet valve_Pressure_Inlet1
		,t1.valve_Pressure_Outlet valve_Pressure_Outlet1
		,t1.valve_Temperature_Inlet valve_Temperature_Inlet1
		,t1.valve_Temperature_Outlet valve_Temperature_Outlet1
		,t1.valve_State valve_State1
    ,t1.Statebypass1   valve1_Statebypass1  
    ,t1.Statebypass2   valve1_Statebypass2
    ,t1.Statebypass3   valve1_Statebypass3
    
		,t2.cshop_Pressure_Inlet cshop_Pressure_Inlet2
		,t2.cshop_Pressure_Outlet cshop_Pressure_Outlet2
		,t2.cshop_Temperature_Inlet cshop_Temperature_Inlet2
		,t2.cshop_Temperature_Outlet cshop_Temperature_Outlet2
		,t2.cshop_Temperature_Cooling cshop_Temperature_Cooling2
		,t2.cshop_Scheme       cshop_Scheme2
		,t2.valve_Pressure_Inlet valve_Pressure_Inlet2
		,t2.valve_Pressure_Outlet valve_Pressure_Outlet2
		,t2.valve_Temperature_Inlet valve_Temperature_Inlet2
		,t2.valve_Temperature_Outlet valve_Temperature_Outlet2
		,t2.valve_State valve_State2
    ,t2.Statebypass1   valve2_Statebypass1  
    ,t2.Statebypass2   valve2_Statebypass2
    ,t2.Statebypass3   valve2_Statebypass3
   FROM  w_SHOP_dt1 t1
   JOIN     v_comp_stations s on s.comp_station_id = t1.comp_station_id
   JOIN   v_sites l on l.site_id = s.site_id
   LEFT OUTER JOIN w_SHOP_dt2  t2 on  t1.comp_shop_id = t2.comp_shop_id  and (  t1.valve_id =t2.valve_id or  t1.valve_id is null)
   WHERE  
                         (  ABS(t1.cshop_Pressure_Inlet - t2.cshop_Pressure_Inlet  ) >= :PLimit )
                OR   (  ABS(t1.cshop_Pressure_Outlet - t2.cshop_Pressure_Outlet  )   >= :PLimit )               
                OR   (  ABS(t1.valve_Pressure_Inlet - t2.valve_Pressure_Inlet  )     >= :PLimit )
                OR   (  ABS(t1.valve_Pressure_Outlet - t2.valve_Pressure_Outlet  )   >= :PLimit )
                
                OR   (  ABS(t1.cshop_Temperature_Inlet - t2.cshop_Temperature_Inlet  )     >= :TLimit )
                OR   (  ABS(t1.cshop_Temperature_Outlet - t2.cshop_Temperature_Outlet  )   >= :TLimit )
                OR   (  ABS(t1.cshop_Temperature_Cooling - t2.cshop_Temperature_Cooling  ) >= :TLimit )
                OR   (  ABS(t1.valve_Temperature_Inlet - t2.valve_Temperature_Inlet  )     >= :TLimit )
                OR   (  ABS(t1.valve_Temperature_Outlet - t2.valve_Temperature_Outlet  )   >= :TLimit )
    
 ORDER BY l.site_name,   s.comp_station_name,t1.comp_shop_name
";
                
        }

        protected override void BindParameters(OracleCommand command, GasModeChangeParameterSet parameters)
        {
            command.AddInputParameter("KeyDate1", parameters.KeyDate1);
            command.AddInputParameter("KeyDate2", parameters.KeyDate2);
            command.AddInputParameter("PLimit",   parameters.PLimit);
            command.AddInputParameter("TLimit",   parameters.TLimit);
        }
        protected override List<CompressorShopValuesChangeDTO> GetResult(OracleDataReader reader, GasModeChangeParameterSet parameters)
        {
            
            var result = new List<CompressorShopValuesChangeDTO>();
            while (reader.Read())
            {
                var valve = new Valve
                {
                    Valve20PressureInlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("valve_Pressure_Inlet1"),
                        Value = reader.GetValue<double?>("valve_Pressure_Inlet2")
                    },
                    Valve20PressureOutlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("valve_Pressure_Outlet1"),
                        Value = reader.GetValue<double?>("valve_Pressure_Outlet2")
                    },
                    Valve20TemperatureInlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("valve_Temperature_Inlet1"),
                        Value = reader.GetValue<double?>("valve_Temperature_Inlet2")
                    },
                    Valve20TemperatureOutlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("valve_Temperature_Outlet1"),
                        Value = reader.GetValue<double?>("valve_Temperature_Outlet2")
                    },
                    ValveState = new ChangeModeValue<int?>
                    {
                        PreviousValue = reader.GetValue<int?>("valve_State1"),
                        Value = reader.GetValue<int?>("valve_State2")
                    },
                    StateByPass1 = new ChangeModeValue<int?>
                    {
                        PreviousValue = reader.GetValue<int?>("valve1_Statebypass1"),
                        Value = reader.GetValue<int?>("valve2_Statebypass1")
                    },
                    StateByPass2 = new ChangeModeValue<int?>
                    {
                        PreviousValue = reader.GetValue<int?>("valve1_Statebypass2"),
                        Value = reader.GetValue<int?>("valve2_Statebypass2")
                    },
                    StateByPass3 = new ChangeModeValue<int?>
                    {
                        PreviousValue = reader.GetValue<int?>("valve1_Statebypass3"),
                        Value = reader.GetValue<int?>("valve2_Statebypass3")
                    },

                    ValveId = reader.GetValue<Guid?>("valve_id"),
                    ValveName = reader.GetValue<string>("valve_name")
                };
                

                var gasModeChangeValue = new CompressorShopValuesChangeDTO
                {
                    CompShopId = reader.GetValue<Guid>("comp_shop_id"),
                    CompShopName = reader.GetValue<string>("comp_shop_name"),
                    SiteId = reader.GetValue<Guid>("site_id"),
                    SiteName = reader.GetValue<string>("site_name"),
                    CompStationId = reader.GetValue<Guid>("comp_station_id"),
                    CompStationName = reader.GetValue<string>("comp_station_name"),
                    CompShopPressureInlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("cshop_Pressure_Inlet1"),
                        Value = reader.GetValue<double?>("cshop_Pressure_Inlet2")

                    },
                    CompShopPressureOutlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("cshop_Pressure_Outlet1"),
                        Value = reader.GetValue<double?>("cshop_Pressure_Outlet2")
                    },
                    CompShopScheme = new ChangeModeValue<string>
                    {
                        PreviousValue = reader.GetValue<string>("cshop_Scheme1"),
                        Value = reader.GetValue<string>("cshop_Scheme2")
                    },
                    CompShopTemperatureCooling = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("cshop_Temperature_Cooling1"),
                        Value = reader.GetValue<double?>("cshop_Temperature_Cooling2")
                    },
                    CompShopTemperatureInlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("cshop_Temperature_Inlet1"),
                        Value = reader.GetValue<double?>("cshop_Temperature_Inlet2")
                    },
                    CompShopTemperatureOutlet = new ChangeModeValueDouble
                    {
                        PreviousValue = reader.GetValue<double?>("cshop_Temperature_Outlet1"),
                        Value = reader.GetValue<double?>("cshop_Temperature_Outlet2")
                    },
                     Valve20 = valve
                    
                };
               

                result.Add(gasModeChangeValue);
            }
            return result;
        }



        
    }
}





