using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Repair
{
    public class KilommetrListGetQuery : QueryReader<Guid,List<double>>
    {
		public KilommetrListGetQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(Guid parameter)
        {
            return @"   select distinct KILOMETER from
					(
					select KILOMETER KILOMETER from V_VALVES where PIPELINE_ID= :pipelineId
					union
                    select KILOMETER_START as KILOMETER from V_PIPELINES where PIPELINE_ID= :pipelineId
                    union
                    select (KILOMETER_START + PIPELINE_LENGTH) as KILOMETER from V_PIPELINES where PIPELINE_ID= :pipelineId
                    )
					order by KILOMETER";
//            return @"   select distinct KILOMETER from
//					(
//					select KILOMETER KILOMETER from V_VALVES WHERE PIPELINE_ID= :pipelineId
//					Union all
//					select KILOMETER_CONN KILOMETER from V_COMP_SHOPS WHERE PIPELINE_ID=:pipelineId
//					)
//					order by KILOMETER";
        }

		protected override void BindParameters(OracleCommand command, Guid parameters)
		{
			command.AddInputParameter("pipelineId", parameters);
		}

        protected override List<double> GetResult(OracleDataReader reader, Guid parameters)
        {
            var repairList = new List<double>();
            while (reader.Read())
            {
                repairList.Add(reader.GetValue<double>("KILOMETER"));
            }
			return repairList;
        }
    }
}
