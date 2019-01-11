using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations
{
    public class RunAstraCalcSqlCommand : CommandNonQuery<RunAstraCalcParameterSet>
    {
         public RunAstraCalcSqlCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

         protected override void BindParameters(OracleCommand command, RunAstraCalcParameterSet parameterSet)
        {
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            command.AddInputParameter("p_series_id", parameterSet.SeriesId);
            command.AddInputParameter("p_is_clear",parameterSet.IsClearCalcValues);

            command.AddInputParameter("p_is_standart", parameterSet.IsExecTypedCalculation);
            command.AddInputParameter("p_is_calculations", parameterSet.IsExecNonTypedCalculation);
            command.AddInputParameter("p_is_astra", parameterSet.IsExecAstraCalculation);
        }

         protected override string GetCommandText(RunAstraCalcParameterSet parameters)
        {
            return "rd.P_COMPUTE1.cmp_series";
        }
    }
}

//P_COMPUTE1.cmp_series
//(p_series_id                 in  rdi.EVENTS.series_id%type --идентификатор серии
//,p_no_set_change             in  pls_integer := '' --если НЕ вызывать P_VALUE.Set_CHANGE
//,p_is_clear                  in  pls_integer := 1 --если удалить все расчетные значения
//,p_is_standart               in  pls_integer := 1 --если выполнять типовые расчеты
//,p_is_calculations           in  pls_integer := 1 --если выполнять НЕтиповые расчеты
//,p_is_astra                  in  pls_integer := 0 --если выполнять расчеты АСТРА
//,p_is_check                  in  pls_integer := 1 --если выполнять типовые проверки
//,p_is_commit                 in  pls_integer := 1 --если делать commit после каждого шага (вычисления, расчета)
//,p_user_name                 in  varchar2 := '' --пользовтель выполнивший действие
//)
