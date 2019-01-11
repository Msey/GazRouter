using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Heaters;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Heaters
{
    public class EditHeaterTypeCommand : CommandNonQuery<EditHeaterTypeParameterSet>
    {
        public EditHeaterTypeCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditHeaterTypeParameterSet parameters)
        {
            command.AddInputParameter("p_heater_type_id", parameters.Id);
            command.AddInputParameter("p_heater_type_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_gas_consumption_rate", parameters.GasConsumptionRate);
            command.AddInputParameter("p_efficiency_factor_rated", parameters.EfficiencyFactorRated);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditHeaterTypeParameterSet parameters)
        {
            return "rd.p_Heater_Type.Edit";
        }
    }
}
