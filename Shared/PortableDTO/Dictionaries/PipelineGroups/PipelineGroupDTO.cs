using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.Dictionaries.PipelineGroups
{
    [DataContract]
	public class PipelineGroupDTO : CommonEntityDTO
    {
        public override EntityType EntityType
        {
            get { return EntityType.PipelineGroup; }
        }

       
        
    }
}