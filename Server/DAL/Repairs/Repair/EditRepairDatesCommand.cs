using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Repair
{
    public class EditRepairDatesCommand : CommandNonQuery<EditRepairDatesParameterSet>
    {
        public EditRepairDatesCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditRepairDatesParameterSet parameters)
        {
            command.AddInputParameter("p_repair_id", parameters.RepairId);

            switch (parameters.DateType)
            {
                case DateTypes.Plan:
                    command.AddInputParameter("p_date_start_plan", parameters.DateStart);
                    command.AddInputParameter("p_date_end_plan", parameters.DateEnd);
                    break;

                case DateTypes.Complex:
                    command.AddInputParameter("p_date_start_complex", parameters.DateStart);
                    command.AddInputParameter("p_date_end_complex", parameters.DateEnd);
                    break;

                case DateTypes.Sched:
                    command.AddInputParameter("p_date_start_sched", parameters.DateStart);
                    command.AddInputParameter("p_date_end_sched", parameters.DateEnd);
                    break;
            }
            
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditRepairDatesParameterSet parameters)
        {
            return "P_REPAIR.Edit";
        }
    }
}
