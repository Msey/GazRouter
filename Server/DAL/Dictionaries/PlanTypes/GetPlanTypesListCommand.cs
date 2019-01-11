using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PlanTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.PlanTypes
{
    public class GetPlanTypesListQuery : QueryReader<List<PlanTypeDTO>>
    {
        public GetPlanTypesListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"select 
  plan_type_id,
  plan_type_name
 from V_PLAN_TYPES";
        }

        protected override List<PlanTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<PlanTypeDTO>();
            while (reader.Read())
            {
                result.Add(new PlanTypeDTO { Id = reader.GetValue<int>("plan_type_id"), Name = reader.GetValue<string>("plan_type_name") });
            }
            return result;
        }
    }
}