using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.CompUnitFailureCauses
{
    public class GetCompUnitFailureCauseListQuery : QueryReader<List<CompUnitFailureCauseDTO>>
    {
        public GetCompUnitFailureCauseListQuery(ExecutionContext context)
            : base(context)
        {
        }


        protected override string GetCommandText()
        {
            return @"   SELECT      failure_cause_id,
                                    failure_cause_name	                                
                        FROM        v_failure_causes
                        ORDER BY    sort_order";
        }


        protected override List<CompUnitFailureCauseDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<CompUnitFailureCauseDTO>();
            while (reader.Read())
            {
                result.Add(new CompUnitFailureCauseDTO
                {
                    Id = reader.GetValue<int>("failure_cause_id"),
                    Name = reader.GetValue<string>("failure_cause_name"),
                });
                
            }
            return result;
        }

        
    }
}