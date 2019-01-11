using System;

namespace GazRouter.DTO.ManualInput.InputStory
{
    public class GetManualInputStoryParameterSet
    {
        public int SerieId { get; set; }
        public Guid? EntityId { get; set; }
        public Guid? SiteId { get; set; }
    }
}