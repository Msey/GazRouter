using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.ObjectModel.CompUnitKtAirs;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.CompUnitTypes
{
    public class GetCompUnitTypeListQuery : QueryReader<List<CompUnitTypeDTO>>
	{
        public GetCompUnitTypeListQuery(ExecutionContext context)
            : base(context)
		{
		}

		protected override string GetCommandText()
		{
            return @"   SELECT  t1.comp_unit_type_id, 
                                t1.comp_unit_type_name, 
                                t1.rated_power, 
                                t1.rated_efficiency, 
                                t1.gas_consumption_rate, 
                                t1.electricity_consumption_rate,
                                t1.engine_type,
                                t1.k_tech_state_pow, 
                                t1.k_tech_state_fuel,
                                t1.engine_class_id, 
                                t1.motorisierteefficiencyfactor,
                                t1.reducerefficiencyfactor,
                                airs.T_Min, airs.T_Max, airs.K_T_VALUE
                        FROM    v_Comp_Unit_Types t1
                        left join V_COMP_UNIT_K_T_AIRS airs on t1.COMP_UNIT_TYPE_ID = airs.COMP_UNIT_TYPE_ID";
		}

        protected override List<CompUnitTypeDTO> GetResult(OracleDataReader reader)
		{
            var result = new List<CompUnitTypeDTO>();
            while (reader.Read())
            {
                var id = reader.GetValue<int>("comp_unit_type_id");
                var temp = result.FirstOrDefault(p => p.Id == id);
                if (temp == null)
                {
                    temp = new CompUnitTypeDTO
                               {
                                   Id = id,
                                   Name = reader.GetValue<string>("comp_unit_type_name"),
                                   GasConsumptionRate = reader.GetValue<double>("gas_consumption_rate"),
                                   RatedPower = reader.GetValue<double>("rated_power"),
                                   RatedEfficiency = reader.GetValue<double>("rated_efficiency"),
                                   ElectricityConsumptionRate = reader.GetValue<double>("electricity_consumption_rate"),
                                   EngineClassName = reader.GetValue<string>("engine_type"),
                                   EngineClassId = reader.GetValue<EngineClass>("engine_class_id"),
                                   KTechStatePow = reader.GetValue<double?>("k_tech_state_pow"),
                                   KTechStateFuel = reader.GetValue<double?>("k_tech_state_fuel"),
                                   MotorisierteEfficiencyFactor = reader.GetValue<double?>("motorisierteefficiencyfactor"),
                                   ReducerEfficiencyFactor = reader.GetValue<double?>("reducerefficiencyfactor")
                               };

                    result.Add(temp);
                }
              
                temp.CompUnitKtAirs.Add(new CompUnitKtAirDTO
                                            {
                                                TMin = reader.GetValue<double?>("T_Min"),
                                                TMax = reader.GetValue<double?>("T_Max"),
                                                KtValue = reader.GetValue<double?>("K_T_VALUE")
                                            });
            }
            return result;
		}
	}
}