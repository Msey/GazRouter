using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Complexes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Complexes
{
    public class AddRepairToComplexCommand : CommandNonQuery<AddRepairToComplexParameterSet>
    {
        public AddRepairToComplexCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddRepairToComplexParameterSet parameters)
        {
            command.AddInputParameter("p_complex_id", parameters.ComplexId);
            command.AddInputParameter("p_repair_id", parameters.RepairId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRepairToComplexParameterSet parameters)
        {
            return "P_REPAIR.Edit";
        }
    }
}
