using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.PowerUnits;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ObjectModel.PowerUnits
{
    public class AddPowerUnitTypeCommand : CommandScalar<AddPowerUnitTypeParameterSet, int>
    {
        public AddPowerUnitTypeCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddPowerUnitTypeParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_power_unit_type_id");
            command.AddInputParameter("p_power_unit_type_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_engine_type_name", parameters.EngineTypeName);
            command.AddInputParameter("p_rated_power", parameters.RatedPower);
            command.AddInputParameter("p_fuel_consumption_rate", parameters.FuelConsumptionRate);
            command.AddInputParameter("p_engine_group", parameters.EngineGroup);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddPowerUnitTypeParameterSet parameters)
        {
            return "rd.p_Power_Unit_Type.AddF";
        }

    }
}
