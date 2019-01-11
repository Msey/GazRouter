using System;

namespace GazRouter.DTO.ManualInput.ChemicalTests
{
    public class GetChemicalTestListParameterSet
    {
        public Guid? SiteId { get; set; }

        public Guid? MeasPointId { get; set; }

        public Guid? ParentId { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}