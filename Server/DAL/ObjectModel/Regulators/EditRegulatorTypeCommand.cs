using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Regulators;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.ObjectModel.Regulators
{
    public class EditRegulatorTypeCommand : CommandNonQuery<EditRegulatorTypeParameterSet>
    {
        public EditRegulatorTypeCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditRegulatorTypeParameterSet parameters)
        {
            command.AddInputParameter("p_regulator_types_id", parameters.Id);
            command.AddInputParameter("p_regulator_type_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_gas_consumption_rate", parameters.GasConsumptionRate);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditRegulatorTypeParameterSet parameters)
        {
            return "rd.p_REGULATOR_Type.Edit";
        }
    }
}
