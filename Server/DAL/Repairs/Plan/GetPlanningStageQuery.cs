using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Plan
{
    public class GetPlanningStageQuery : QueryReader<GetRepairPlanParameterSet, PlanningStageDTO>
    {
        public GetPlanningStageQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetRepairPlanParameterSet parameter)
        {
            return @"   SELECT	    lock_status,
                                    upd_user_name,
                                    upd_date             
                        FROM	    V_REPAIR_LOCKS
                        WHERE       year = :year
                        AND         system_id = :system_id";

        }

        protected override void BindParameters(OracleCommand command, GetRepairPlanParameterSet parameters)
		{
            //parameters - год плана
            command.AddInputParameter(":system_id", parameters.SystemId);
            command.AddInputParameter(":year", parameters.Year);
		}

        protected override PlanningStageDTO GetResult(OracleDataReader reader, GetRepairPlanParameterSet parameters)
        {
            var result = new PlanningStageDTO();
            if (reader.Read())
                return new PlanningStageDTO
                {
                    Stage = (PlanningStage)reader.GetValue<int>("lock_status"),
                    UserName = reader.GetValue<string>("upd_user_name"),
                    UpdateDate = reader.GetValue<DateTime>("upd_date")
                };


            return result;

        }
    }
}
