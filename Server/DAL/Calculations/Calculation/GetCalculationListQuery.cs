using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Calculation
{
    public class GetCalculationListQuery : QueryReader<GetCalculationListParameterSet, List<CalculationDTO>>
    {
        public GetCalculationListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetCalculationListParameterSet parameters)
        {
            var sql = @"    SELECT      c.calculation_id, 
                                        c.sys_name, 
                                        c.description, 
                                        c.expression, 
                                        c.exec_sql, 
                                        c.is_invalid, 
                                        c.errm, 
                                        cl.period_type_id,
                                        cl.sort_order,
                                        c.calc_stage
                            
                            FROM        v_calculations c 
                            INNER JOIN  v_calculation_links_ext cl ON c.calculation_id = cl.calculation_id
                            WHERE 1=1";

            var sb = new StringBuilder(sql);

            if (parameters != null)
            {
                if (parameters.CalculationId.HasValue) sb.Append(" AND c.calculation_id = :id");
                if (parameters.PeriodType.HasValue) sb.Append(" AND cl.period_type_id = :period");

                if (parameters.EntityId.HasValue)
                {
                    sb.Append(@" AND c.calculation_id IN 
                                (   SELECT      calculation_id
                                    FROM        v_parameters
                                    WHERE       entity_id = :entityId");
                                       
                    if (parameters.PropertyType.HasValue)
                        sb.Append(" AND property_type_id = :propType");

                    if (parameters.ParameterType.HasValue)
                        sb.Append(" AND parameter_type_id <> :parameterType");

                    sb.Append(")");
                }
            }

            

            sb.Append(" ORDER BY cl.sort_order, c.sys_name");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GetCalculationListParameterSet parameters)
        {
            if (parameters.CalculationId.HasValue)
                command.AddInputParameter("id", parameters.CalculationId);

            if (parameters.PeriodType.HasValue)
                command.AddInputParameter("period", parameters.PeriodType);

            if (parameters.ParameterType.HasValue)
                command.AddInputParameter("parameterType", parameters.ParameterType);

            if (parameters.EntityId.HasValue)
                command.AddInputParameter("entityId", parameters.EntityId);

            if (parameters.PropertyType.HasValue)
                command.AddInputParameter("propType", parameters.PropertyType);
        }

        protected override List<CalculationDTO> GetResult(OracleDataReader reader, GetCalculationListParameterSet parameters)
        {
            var result = new List<CalculationDTO>();
            while (reader.Read())
            {
                result.Add(
                    new CalculationDTO
                    {
                        Id = reader.GetValue<int>("calculation_id"),
                        SysName = reader.GetValue<string>("sys_name"),
                        Description = reader.GetValue<string>("description"),
                        Expression = reader.GetValue<string>("expression"),
                        Sql = reader.GetValue<string>("exec_sql"),
                        IsInvalid = reader.GetValue<bool>("is_invalid"),
                        Errm = reader.GetValue<string>("errm"),
                        PerionTypeId = reader.GetValue<PeriodType>("period_type_id"),
                        SortOrder = reader.GetValue<int>("sort_order"),
                        CalcStage = reader.GetValue<CalculationStage>("calc_stage")
                    });
            }
            return result;
        }
    }
}