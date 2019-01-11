using System;
using GazRouter.DTO.Repairs.Plan;

namespace GazRouter.Flobus.VM.Model
{
    public interface IRepairWorks
    {
        RepairPlanBaseDTO Dto { get; }
        TimeSpan DurationPlan { get; }
        int Id { get; }
        bool IsPipeline { get; }
        string ObjectName { get; }
        string PipelineName { get; }
        string RepairState { get; }
        string StartDateMonthName { get; }
        string WorkflowState { get; }
    }
}