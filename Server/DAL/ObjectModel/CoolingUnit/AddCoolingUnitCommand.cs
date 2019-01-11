using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.CoolingUnit
{
    public class AddCoolingUnitCommand : CommandScalar<AddCoolingUnitParameterSet, Guid>
    {
        public AddCoolingUnitCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddCoolingUnitParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<Guid>("entity_id");
            command.AddInputParameter("p_entity_name", parameters.Name);
            command.AddInputParameter("P_COOLING_STATION_ID", parameters.ParentId);
			command.AddInputParameter("P_SORT_ORDER", parameters.SortOrder);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
			command.AddInputParameter("P_COOLING_UNIT_TYPE_ID", parameters.CoolintUnitType);
        }

        protected override string GetCommandText(AddCoolingUnitParameterSet parameters)
        {
			return "P_COOLING_UNIT.ADDF";
        }
    }
}