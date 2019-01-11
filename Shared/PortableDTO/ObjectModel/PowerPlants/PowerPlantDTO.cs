using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.PowerPlants
{
    [DataContract]
    public class PowerPlantDTO : EntityDTO
	{
        public override EntityType EntityType
        {
            get { return EntityType.PowerPlant; }
        }
    }
}