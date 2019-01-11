using System;
using GazRouter.DTO.Dictionaries.EngineClasses;

namespace GazRouter.DTO.ObjectModel.CompShops
{
    public class EditCompShopParameterSet : EditEntityParameterSet
    {
        public Guid PipelineId { get; set; }

        public double KmOfConn { get; set; }

        public EngineClass EngineClassId { get; set; }
        public double PipingVolume { get; set; }

        public double PipingVolumeIn { get; set; }
        public double PipingVolumeOut { get; set; }
    }
}
