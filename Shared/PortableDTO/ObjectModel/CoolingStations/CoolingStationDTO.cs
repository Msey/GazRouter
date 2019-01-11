
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.CoolingStations
{
    public class CoolingStationDTO : EntityDTO
    {
        public override EntityType EntityType
        {
            get { return EntityType.CoolingStation;}
        }
    }
}
