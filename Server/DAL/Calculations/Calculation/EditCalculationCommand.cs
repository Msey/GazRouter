using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Calculation
{
    public class EditCalculationCommand : CommandNonQuery<EditCalculationParameterSet>
    {
        public EditCalculationCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditCalculationParameterSet parameters)
        {
            command.AddInputParameter("p_calculation_id", parameters.CalculationId);
            command.AddInputParameter("p_sys_name", parameters.SysName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_expression", parameters.Expression);
            command.AddInputParameter("p_expression_src", parameters.ExpressionOriginal);
            command.AddInputParameter("p_exec_sql", parameters.Sql);
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_sort_order", parameters.SortOrder);
            command.AddInputParameter("p_calc_stage", parameters.CalcStage);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditCalculationParameterSet parameters)
        {
            return "P_CALCULATION.Edit";
        }
    }
}