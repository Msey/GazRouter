using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations.Parameter;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Parameter
{
    public class EditCalculationParameterCommand : CommandNonQuery<AddEditCalculationParameterParameterSet>
    {
        public EditCalculationParameterCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditCalculationParameterParameterSet parameters)
        {
            command.AddInputParameter("p_parameter_id", parameters.Id);
            command.AddInputParameter("p_alias_name", parameters.Alias);
            command.AddInputParameter("p_parameter_type_id", parameters.ParameterTypeId);
            command.AddInputParameter("p_calculation_id", parameters.CalculationId);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_property_type_id", parameters.PropertyTypeId);
            command.AddInputParameter("p_entity_id", parameters.EntityId);
            command.AddInputParameter("p_value", parameters.Value);
            command.AddInputParameter("p_time_shift_unit", parameters.TimeShiftUnit.ToString().ToUpper());
            command.AddInputParameter("p_time_shift_value", parameters.TimeShiftValue);
            command.AddInputParameter("p_is_nvl", parameters.IsNumeric);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddEditCalculationParameterParameterSet parameters)
        {
            return "P_PARAMETER.Edit";
        }
    }
}