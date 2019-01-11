using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Regulators;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Regulators
{
    public class AddRegulatorTypeCommand : CommandScalar<AddRegulatorTypeParameterSet, int>
    {
        public AddRegulatorTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddRegulatorTypeParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_regulator_types_id");
            command.AddInputParameter("p_regulator_type_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_gas_consumption_rate", parameters.GasConsumptionRate);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddRegulatorTypeParameterSet parameters)
        {
            return "rd.P_REGULATOR_TYPE.AddF";
        }
    }
}
