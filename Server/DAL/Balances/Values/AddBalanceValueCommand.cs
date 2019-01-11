﻿using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Values;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Values
{
    public class AddBalanceValueCommand : CommandScalar<SetBalanceValueParameterSet, int>
    {
		public AddBalanceValueCommand(ExecutionContext context)
            : base(context)
		{
		    IsStoredProcedure = true;
		}

        protected override void BindParameters(OracleCommand command, SetBalanceValueParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_value_id");
            command.AddInputParameter("p_contract_id", parameters.ContractId);
            command.AddInputParameter("p_gas_owner_id", parameters.GasOwnerId);

            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_item_id", parameters.BalanceItem);

            command.AddInputParameter("p_value_base", parameters.BaseValue);
            command.AddInputParameter("p_value_correct", parameters.Correction);
			command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(SetBalanceValueParameterSet parameters)
		{
			return "P_BL_VALUE.AddF";
		}
    }
}