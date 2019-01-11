using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ChemicalTests
{
    public class DeleteChemicalTestCommand: CommandNonQuery<int>
    {
        public DeleteChemicalTestCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
		{
            command.AddInputParameter("p_chemical_test_id", parameters);
		}

        protected override string GetCommandText(int parameters)
        {
            return "P_CHEMICAL_TEST.Remove";
        }
    }
}