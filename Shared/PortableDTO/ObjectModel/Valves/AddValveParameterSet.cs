using System;
using GazRouter.DTO.Dictionaries.ValvePurposes;

namespace GazRouter.DTO.ObjectModel.Valves
{
    public class AddValveParameterSet
    {
        public string Name { get; set; }
        public int ValveTypeId { get; set; }
        public Guid PipelineId { get; set; }
        public double Kilometr { get; set; }
        public int? Bypass1TypeId { get; set; }
        public int? Bypass2TypeId { get; set; }
        public int? Bypass3TypeId { get; set; }
        public ValvePurpose ValvePurposeId { get; set; }
        public Guid? CompShopId { get; set; }
        public int SortOrder { get; set; }

        public bool IsControlPoint { get; set; }
    
    }
}
