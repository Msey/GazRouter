using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitStates;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates.Failures.RelatedUnitStart
{
    public class GetFailureRelatedUnitStartListQuery : QueryReader<List<int>, List<FailureRelatedUnitStartDTO>>
    {
        public GetFailureRelatedUnitStartListQuery(ExecutionContext context)
            : base(context)
        {
        }


        protected override string GetCommandText(List<int> parameters)
        {
            var q = @"  SELECT      rc.comp_unit_failure_detail_id,
                                    rc.comp_unit_state_change_id,
                                    sc.comp_unit_id,
                                    sc.state_change_date,
                                    e.entity_name,
                                    u.comp_unit_type_id
                        FROM        v_failure_related_changes rc
                        INNER JOIN  v_comp_unit_state_changes sc ON rc.comp_unit_state_change_id = sc.comp_unit_state_change_id
                        INNER JOIN  v_comp_units u ON u.comp_unit_id = sc.comp_unit_id
                        INNER JOIN  v_nm_short_all e ON e.entity_id = sc.comp_unit_id";

            var sb = new StringBuilder(q);

            if (parameters != null)
            {
                sb.Append(" WHERE rc.comp_unit_failure_detail_id IN ");
                sb.Append(CreateInClause(parameters.Count));
            }

            sb.Append(" ORDER BY sc.state_change_date");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, List<int> parameters)
        {
            if (parameters == null) return;

            for (var i = 0; i < parameters.Count; i++)
                command.AddInputParameter(string.Format("p{0}", i), parameters[i]);
            
        }

        protected override List<FailureRelatedUnitStartDTO> GetResult(OracleDataReader reader, List<int> parameters)
        {
            var result = new List<FailureRelatedUnitStartDTO>();
            while (reader.Read())
            {
                var dto = new FailureRelatedUnitStartDTO
                {
                    FailureId = reader.GetValue<int>("comp_unit_failure_detail_id"),
                    StateChangeId = reader.GetValue<int>("comp_unit_state_change_id"),
                    CompUnitId = reader.GetValue<Guid>("comp_unit_id"),
                    StateChangeDate = reader.GetValue<DateTime>("state_change_date"),
                    CompUnitName = reader.GetValue<string>("entity_name"),
                    CompUnitTypeId = reader.GetValue<int>("comp_unit_type_id")
                };
                
                result.Add(dto);
            }
            return result;
        }
    }
}
