using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.PlanTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Plan
{
    public class GetPlanTypesQuery : QueryReader<List<PlanTypeDTO>>
    {
        public GetPlanTypesQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText()
        {
            return @"select plan_type_id, plan_type_name, description, sort_order from v_plan_types"; 
        }

        protected override List<PlanTypeDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<PlanTypeDTO>();
            while (reader.Read())
            {
                result.Add(
                    new PlanTypeDTO
                    {
                        Id = reader.GetValue<int>("plan_type_id"),
                        Name = reader.GetValue<string>("plan_type_name")
                    });
            }
            return result;
        }
    }
}
