using System;

namespace GazRouter.DTO.ManualInput.InputStates
{
    public class SetManualInputStateParameterSet
    {
        public int SerieId { get; set; }
        public Guid SiteId { get; set; }
        public ManualInputState State { get; set; }
    }
}