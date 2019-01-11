using GazRouter.DAL.Core;
using GazRouter.DTO.GasLeaks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.GasLeaks
{
    public class EditLeakCommand : CommandNonQuery<EditLeakParameterSet>
    {
        public EditLeakCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditLeakParameterSet parameters)
        {
            command.AddInputParameter("p_leak_id", parameters.LeakId);
            command.AddInputParameter("p_leak_number", 0);
            command.AddInputParameter("p_leak_place", parameters.Place);
            command.AddInputParameter("p_leak_place_km", parameters.Kilometer);
            command.AddInputParameter("p_leak_reason", parameters.Reason);
            command.AddInputParameter("p_volume_day", parameters.VolumeDay);
            command.AddInputParameter("p_repair_activity", parameters.RepairActivity);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_contact_name", parameters.ContactName);
            command.AddInputParameter("p_discovered_date", parameters.DiscoverDate);
            command.AddInputParameter("p_repair_plan_date", parameters.RepairPlanDate);
            command.AddInputParameter("p_repair_plan_fact", parameters.RepairPlanFact);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditLeakParameterSet parameters)
        {
            return "P_LEAK.Edit";
        }
    }
}