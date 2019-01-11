using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitTests;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests
{
    public class AddCompUnitTestCommand : CommandScalar<AddCompUnitTestParameterSet, int>
    {
        public AddCompUnitTestCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddCompUnitTestParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("comp_unit_test_id");

            command.AddInputParameter("p_comp_unit_id", parameters.CompUnitId);
            command.AddInputParameter("p_comp_unit_test_date", parameters.CompUnitTestDate);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_q_min", parameters.Qmin);
            command.AddInputParameter("p_q_max", parameters.Qmax);
            command.AddInputParameter("p_density", parameters.Density);
            command.AddInputParameter("p_temperature", parameters.TemperatureIn);
            command.AddInputParameter("p_pressurein", parameters.PressureIn);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddCompUnitTestParameterSet parameters)
        {
            return "P_COMP_UNITS_TEST.AddF";
        }
    }
}
