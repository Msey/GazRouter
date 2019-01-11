namespace GazRouter.DTO.Calculations
{
    public class RunAstraCalcParameterSet
    {
        public int SeriesId { get; set; }   // идентификатор серии
        public bool IsClearCalcValues { get; set; }   // 1- удалить все предыдущие расчетные значения; 0 - перезаписать текущие
        public bool IsExecTypedCalculation { get; set; }   // 1- выполнять типовые расчеты; 0 - не выполнять типовые расчеты
        public bool IsExecNonTypedCalculation { get; set; } // 1- выполнять все нетиповые расчеты; 0 - не выполнять нетиповые расчеты
        public bool IsExecAstraCalculation { get; set; }  //1 - выполнить расчеты на основе данных из астры ; 0 - не выполнять расчета на основе данных из АСТРЫ

    }
}