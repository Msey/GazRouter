using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Repair
{
    public class EditRepairCommand : CommandNonQuery<EditRepairParameterSet>
    {
		public EditRepairCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

		protected override void BindParameters(OracleCommand command, EditRepairParameterSet parameters)
        {
            command.AddInputParameter("p_repair_id", parameters.Id);
            command.AddInputParameter("p_repair_type_id", parameters.RepairType);
            command.AddInputParameter("p_execution_means_id", parameters.ExecutionMeans);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_bleed_amount", parameters.BleedAmount);
            command.AddInputParameter("p_gas_saving", parameters.SavingAmount);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_description_gtp", parameters.DescriptionGtp);
            command.AddInputParameter("p_description_cpdd", parameters.DescriptionCPDD);
            command.AddInputParameter("p_date_start_sched", parameters.DateStartShed);
            command.AddInputParameter("p_date_end_sched", parameters.DateEndShed);
            command.AddInputParameter("p_date_start_plan", parameters.StartDate);
            command.AddInputParameter("p_date_end_plan", parameters.EndDate);
            command.AddInputParameter("p_transfer_fact_w", parameters.MaxTransferWinter);
            command.AddInputParameter("p_transfer_fact_s", parameters.MaxTransferSummer);
            command.AddInputParameter("p_transfer_fact_x", parameters.MaxTransferTransition);
            command.AddInputParameter("p_capacity_plan_w", parameters.CapacityWinter);
            command.AddInputParameter("p_capacity_plan_s", parameters.CapacitySummer);
            command.AddInputParameter("p_capacity_plan_x", parameters.CapacityTransition);
            command.AddInputParameter("p_transfer_work", parameters.CalculatedTransfer);
            command.AddInputParameter("p_is_transfer_relation", parameters.IsCritical);
            command.AddInputParameter("p_is_ext_condition", parameters.IsExternalCondition);
            command.AddInputParameter("p_parts_delivery_date", parameters.PartsDeliveryDate);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);

            command.AddInputParameter("p_resolution_date", parameters.ResolutionDate);
            command.AddInputParameter("p_resolution_num", parameters.ResolutionNum);
            command.AddInputParameter("p_resolution_date_cpdd", parameters.ResolutionDateCpdd);
            command.AddInputParameter("p_resolution_num_cpdd", parameters.ResolutionNumCpdd);

            command.AddInputParameter("p_fireworks_type", parameters.FireworkType);

            command.AddInputParameter("p_date_start_fact", parameters.DateStartFact);
            command.AddInputParameter("p_date_end_fact", parameters.DateEndFact);

            command.AddInputParameter("p_duration", parameters.Duration);
            command.AddInputParameter("p_gazprom_plan_id", parameters.GazpromPlanID);
            command.AddInputParameter("p_consumers_state", parameters.ConsumersState);
            command.AddInputParameter("p_plan_reg_date", parameters.GazpromPlanDate);

            command.AddInputParameter("P_PLAN_TYPE_ID", parameters.PlanType == null ? DTO.Dictionaries.PlanTypes.PlanType.Planned : parameters.PlanType);
        }

		protected override string GetCommandText(EditRepairParameterSet parameters)
        {
            return "P_REPAIR.Edit";
        }
    }
}
