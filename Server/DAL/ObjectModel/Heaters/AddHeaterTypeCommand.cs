using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Heaters;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Heaters
{
    public class AddHeaterTypeCommand : CommandScalar<AddHeaterTypeParameterSet, int>
    {
        public AddHeaterTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddHeaterTypeParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_heater_type_id");
            command.AddInputParameter("p_heater_type_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_gas_consumption_rate", parameters.GasConsumptionRate);
            command.AddInputParameter("p_efficiency_factor_rated", parameters.EfficiencyFactorRated);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddHeaterTypeParameterSet parameters)
        {
            return "rd.P_HEATER_TYPE.AddF";
        }
    }
}
