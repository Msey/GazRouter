using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.ContractPressures;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ContractPressures
{
    public class AddEditContractPressureCommand : CommandNonQuery<AddEditContractPressureParameterSet>
    {
        public AddEditContractPressureCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditContractPressureParameterSet parameters)
        {
            command.AddInputParameter("p_distr_station_outlet_id", parameters.distr_station_outlet_id);
            command.AddInputParameter("p_input_date", DateTime.Now);
            command.AddInputParameter("p_contract_pressure", parameters.contract_pressure);
            command.AddInputParameter("p_input_user", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEditContractPressureParameterSet parameters)
        {
            return "rd.P_CONTRACT_PRESSURE.Set_DATA";
        }
    }
}
