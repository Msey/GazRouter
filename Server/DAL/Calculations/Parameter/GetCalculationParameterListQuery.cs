using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Parameter;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Parameter
{
    public class GetCalculationParameterListQuery : QueryReader<int, List<CalculationParameterDTO>>
    {
        public GetCalculationParameterListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(int parameters)
        {
            var sql = @"    WITH var_cnt AS 
                            (
                                SELECT          p.entity_id, 
                                                p.property_type_id, 
                                                cl.period_type_id, 
                                                p.parameter_type_id, 
                                                count(*) AS cnt 
                                FROM            v_parameters p 
                                INNER JOIN      v_calculation_links_ext cl on cl.calculation_id = p.calculation_id
                                WHERE           p.calculation_id <> :id
                                GROUP BY        p.entity_id, p.property_type_id, cl.period_type_id, p.parameter_type_id
                            )

                            SELECT      p.parameter_id,
                                        p.alias_name, 
                                        p.parameter_type_id,
                                        p.calculation_id, 
                                        p.property_type_id, 
                                        p.entity_id, 
                                        n.Entity_name path,
                                        p.time_shift_unit, 
                                        p.time_shift_value,
                                        p.value, 
                                        e.entity_type_id,
                                        cnt.cnt
                            FROM        v_parameters p
                            INNER JOIN  v_calculation_links_ext cl ON cl.calculation_id = p.calculation_id
                            INNER JOIN  v_entities e ON e.entity_id = p.entity_id
                            LEFT JOIN   v_nm_short_all n ON p.entity_id = n.entity_id
                            LEFT JOIN   var_cnt cnt ON cnt.entity_id = p.entity_id 
                                            AND cnt.property_type_id = p.property_type_id 
                                            AND cnt.period_type_id = cl.period_type_id 
                                            AND cnt.parameter_type_id <> p.parameter_type_id
                            WHERE       p.calculation_id = :id
                            ORDER BY    p.alias_name";


            return sql;
        }

        protected override List<CalculationParameterDTO> GetResult(OracleDataReader reader, int calculationId)
        {
            var result = new List<CalculationParameterDTO>();
            while (reader.Read())
            {
                result.Add(
                    new CalculationParameterDTO
                    {
                        Id = reader.GetValue<int>("parameter_id"),
                        Alias = reader.GetValue<string>("alias_name"),
                        EntityTypeId = reader.GetValue<EntityType>("entity_type_id"),
                        ParameterTypeId = reader.GetValue<ParameterType>("parameter_type_id"),
                        CalculationId = reader.GetValue<int>("calculation_id"),
                        PropertyTypeId = reader.GetValue<PropertyType>("property_type_id"),
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        TimeShiftUnit = reader.GetValue<string>("time_shift_unit"),
                        TimeShiftValue = reader.GetValue<int>("time_shift_value"),
                        Value = reader.GetValue<string>("value"),
                        Path = reader.GetValue<string>("path"),
                        UseCount = reader.GetValue<int>("cnt"),
                    });
            }
            return result;
        }

        protected override void BindParameters(OracleCommand command, int calculationId)
        {
            command.AddInputParameter("id", calculationId);
        }
    }
}