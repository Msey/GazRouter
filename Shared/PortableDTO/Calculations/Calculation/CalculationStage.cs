namespace GazRouter.DTO.Calculations.Calculation
{
    // Этапы выполнения нетиповых расчетов:
    // 1 - перед типовыми расчетами
    // 2 - после типовых расчетов
    // 3 - после расчета агрегаторов
    public enum CalculationStage
    {
        BeforeStandard = 1,
        AfterStandard = 2,
        AfterAggregators = 3
    }
}