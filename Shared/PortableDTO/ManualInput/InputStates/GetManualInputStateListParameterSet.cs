using System;

namespace GazRouter.DTO.ManualInput.InputStates
{
    public class GetManualInputStateListParameterSet
    {
        public int SerieId { get; set; }
        public Guid? SiteId { get; set; }
    }
}