namespace GazRouter.DTO.Calculations.Calculation
{
    // ����� ���������� ��������� ��������:
    // 1 - ����� �������� ���������
    // 2 - ����� ������� ��������
    // 3 - ����� ������� �����������
    public enum CalculationStage
    {
        BeforeStandard = 1,
        AfterStandard = 2,
        AfterAggregators = 3
    }
}