using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitTests;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests
{
    public class AddCompUnitTestPointCommand : CommandNonQuery<AddCompUnitTestPointParameterSet>
    {
        public AddCompUnitTestPointCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddCompUnitTestPointParameterSet parameters)
        {
            command.AddInputParameter("p_comp_unit_test_id", parameters.CompUnitTestId);
            command.AddInputParameter("p_type_line", parameters.LineType);
            command.AddInputParameter("p_x", parameters.X);
            command.AddInputParameter("p_y", parameters.Y);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddCompUnitTestPointParameterSet parameters)
        {
            return "P_COMP_UNITS_TESTS_POINT.Add";
        }
    }
}
