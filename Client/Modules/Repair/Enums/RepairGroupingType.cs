namespace GazRouter.Repair.Enums
{
    public enum RepairGroupingType
    {
        ByPipeline = 1,         // по газопроводу
        BySite = 2,             // по ЛПУ
        ByRepairType = 3,       // по виду работ
        ByComplex = 4,          // по комплексу
        ByPipelineGroup = 5,    // по технологическому коридору
        ByExecutionMeans = 6,   // по способу ведения работ
        ByMonth = 7            // по месяцам
    }
}