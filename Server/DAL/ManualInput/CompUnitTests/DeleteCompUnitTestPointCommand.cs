using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitTests;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests
{
    public class DeleteCompUnitTestPointCommand : CommandNonQuery<DeleteCompUnitTestPointParameterSet>
    {
        public DeleteCompUnitTestPointCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, DeleteCompUnitTestPointParameterSet parameters)
        {
            command.AddInputParameter("p_comp_unit_test_id", parameters.CompUnitTestId);
            command.AddInputParameter("p_type_line", parameters.LineType);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(DeleteCompUnitTestPointParameterSet parameters)
        {
            return "P_COMP_UNITS_TESTS_POINT.Remove";
        }
    }
}
