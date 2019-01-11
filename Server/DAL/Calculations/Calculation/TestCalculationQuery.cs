using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Calculations.Parameter;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Calculation
{
    public class TestCalculationQuery : QueryReader<TestCalculationParameterSet, List<CalculationParameterDTO>>
    {
        public TestCalculationQuery(ExecutionContext context)
            : base(context)
        {}


        protected override string GetCommandText(TestCalculationParameterSet parameters)
        {
            return @"   SELECT      p.parameter_id, 
                                    p.alias_name, 
                                    p.parameter_type_id, 
                                    p.calculation_id, 
                                    p.sort_order, 
                                    p.property_type_id, 
                                    p.entity_id, 
                                    e.entity_type_id,
                                    p.time_shift_unit, 
                                    p.time_shift_value,
                                    n.Entity_name AS pth,
                                    sys.anydata.AccessVarchar2(p.value) AS value1
                        FROM        TABLE(rd.p_calculation.executesqltestf(:id, :showerror)) p, v_entities e
                        LEFT JOIN   v_nm_all n ON e.entity_id = n.entity_id
                        WHERE       p.entity_id = e.entity_id";
        }

        protected override List<CalculationParameterDTO> GetResult(OracleDataReader reader, TestCalculationParameterSet parameterSet)
        {
            var result = new List<CalculationParameterDTO>();
            while (reader.Read())
            {
                result.Add(
                    new CalculationParameterDTO
                    {
                        Id = reader.GetValue<int>("parameter_id"),
                        Alias = reader.GetValue<string>("alias_name"),
                        ParameterTypeId = reader.GetValue<ParameterType>("parameter_type_id"),
                        CalculationId = reader.GetValue<int>("calculation_id"),
                        PropertyTypeId = reader.GetValue<PropertyType>("property_type_id"),
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        EntityTypeId = reader.GetValue<EntityType>("entity_type_id"),
                        TimeShiftUnit = reader.GetValue<string>("time_shift_unit"),
                        TimeShiftValue = reader.GetValue<int>("time_shift_value"),
                        Value = reader.GetValue<string>("value1"),
                        Path = reader.GetValue<string>("pth"),
                    });
            }
            return result;
        }

        protected override void BindParameters(OracleCommand command, TestCalculationParameterSet parameterSet)
        {
            
            command.AddInputParameter("id", parameterSet.CalculationId);
            command.AddInputParameter("showerror", parameterSet.HideError ? 0 : 1);
        }
    }
}