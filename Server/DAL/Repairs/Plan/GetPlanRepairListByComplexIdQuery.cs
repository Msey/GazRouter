using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Plan
{
    /// <summary>
    /// Эта команда используется в функции сервиса MoveComplex
    /// </summary>
    public class GetPlanRepairListByComplexIdQuery : QueryReader<int, List<RepairPlanBaseDTO>>
    {
		public GetPlanRepairListByComplexIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(int parameter)
        {
			return @"   SELECT	    r.repair_id,
                                    r.date_start_plan,
                                    r.date_end_plan
                                                                        
                        FROM	    V_REPAIRS r
                        WHERE       r.complex_id = :cmplxid";
                        
        }

		protected override void BindParameters(OracleCommand command, int parameters)
		{
            //parameters - год плана
            command.AddInputParameter("cmplxid", parameters);
        }

        protected override List<RepairPlanBaseDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var repairList = new List<RepairPlanBaseDTO>();
            while (reader.Read())
            {
            	repairList.Add(
                    new RepairPlanBaseDTO
            			{
							Id = reader.GetValue<int>("repair_id"),
                            StartDate = reader.GetValue<DateTime>("date_start_plan"),
            				EndDate = reader.GetValue<DateTime>("date_end_plan"),
						});
            }
            return repairList;
        }
    }
}
