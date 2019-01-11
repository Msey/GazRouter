using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.ManualInput.Settings.EntityProperties
{
    [RegionMemberLifetime(KeepAlive = true)]
    public class EntityPropertiesViewModel : LockableViewModel
    {
        public EntityPropertiesViewModel(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
            Load();
        }


        public bool IsReadOnly { get; set; }

        public List<EntityTypeItem> EntityTypeList { get; set; }

        private async void Load()
        {
            Lock();
          
            var propTypes = await new ObjectModelServiceProxy().GetEntityTypePropertiesAsync(null);
            
            // Загрузка типов свойств
            EntityTypeList =
                ClientCache.DictionaryRepository.EntityTypes.Where(et => et.EntityProperties.Count > 0)
                    .Select(et => new EntityTypeItem(et, propTypes.Where(pt => pt.EntityType == et.EntityType).ToList(), !IsReadOnly))
                    .ToList();
            OnPropertyChanged(() => EntityTypeList);
            
            Unlock();
        }
    }
}