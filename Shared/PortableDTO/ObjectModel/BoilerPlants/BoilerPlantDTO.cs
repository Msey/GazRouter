using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.BoilerPlants
{
    [DataContract]
    public class BoilerPlantDTO : EntityDTO
	{
        public override EntityType EntityType
        {
            get { return EntityType.BoilerPlant; }
        }
    }
}