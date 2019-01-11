using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.RepairWorks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.RepairWorks
{
    public class AddRepairWorkCommand : CommandScalar<RepairWorkParameterSet, int>
    {
		public AddRepairWorkCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, RepairWorkParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_repair_work_id");
            command.AddInputParameter("p_work_type_id", parameters.WorkTypeId);
            command.AddInputParameter("p_repair_id", parameters.RepairId);
            if (parameters.KilometerStart.HasValue)
                command.AddInputParameter("p_kilometer_start", parameters.KilometerStart);
            if (parameters.KilometerEnd.HasValue)
                command.AddInputParameter("p_kilometer_end", parameters.KilometerEnd);

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(RepairWorkParameterSet parameters)
        {
            return "P_REPAIR_WORK.AddF";
        }
    }
}
