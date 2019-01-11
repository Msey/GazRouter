using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.ManualInput.Settings.EntityProperties
{
    public class EntityTypeItem
    {
        public EntityTypeItem(EntityTypeDTO entityType, IEnumerable<EntityTypePropertyDTO> propTypes, bool isEnabled)
        {
            Dto = entityType;
            PropertyTypeList =
                entityType.EntityProperties.Select(
                    pt => new PropertyTypeItem(entityType.Id, propTypes.Single(i => i.PropertyType == pt.PropertyType))
                    {
                        Name = pt.Name,
                        IsEnabled = isEnabled,
                    })
                    .ToList();
        }

        public EntityTypeDTO Dto { get; set; }


        public List<PropertyTypeItem> PropertyTypeList { get; set; }
    }
}