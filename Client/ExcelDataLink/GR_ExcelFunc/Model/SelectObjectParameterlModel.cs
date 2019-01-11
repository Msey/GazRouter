using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.Sites;
using DataProviders;
using DataProviders.Dictionaries;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.EventLog;

namespace GR_ExcelFunc.Model
{
    public interface ISelectObjectParameterData
    {
        IEnumerable<SiteDTO> GetSiteList();
        IEnumerable<CommonEntityDTO> GetEntityList();
        IEnumerable<EntityTypeDTO> GetEntityTypeList();
        IEnumerable<CommonEntityDTO> GetEntityListBySearch(string serchText, EntityType? entityType);
        IEnumerable<CommonEntityDTO> GetEntityListByEntityType(EntityType entityType);
        IEnumerable<CommonEntityDTO> GetEntityListBySite(SiteDTO site);

        IEnumerable<PropertyTypeDTO> GetPropertyTypeListByEntityType(EntityType entityType);
        IEnumerable<PeriodTypeDTO> GetPeriodTypeList();

    }
    public class SelectObjectParameterData: BaseSync,ISelectObjectParameterData
    {
        private readonly DictionaryRepositoryDTO _dictRepository;
       
        private readonly List<SiteDTO> _siteList;
        private readonly List<CommonEntityDTO> _entityList;
        public SelectObjectParameterData()
        {
            
            _dictRepository = ExecuteSync(new DictionaryServiceProxy().GetDictionaryRepositoryAsync);
            
            _siteList = ExecuteSync(new ObjectModelServiceProxy().GetSiteListAsync, new GetSiteListParameterSet { }); 
            _entityList = ExecuteSync(new ObjectModelServiceProxy().GetEntityListAsync, new GetEntityListParameterSet { });
         }

        public IEnumerable<SiteDTO> GetSiteList()
        {
           return _siteList;
        }

        public IEnumerable<CommonEntityDTO> GetEntityList()
        {
            return _entityList;
        }

        public IEnumerable<EntityTypeDTO> GetEntityTypeList()
        {

            return _dictRepository.EntityTypes;
        }

        public IEnumerable<CommonEntityDTO> GetEntityListBySearch(string searchText,EntityType? entityType)
        {
            
            var entList = from entity in _entityList
                where Regex.IsMatch(entity.Name, searchText, RegexOptions.IgnoreCase)
                select entity;
            if  (entityType == null)
                return entList;
            var ret = from ent in entList
                where ent.EntityType == entityType.Value 
                select  ent;
            return ret;

        }

        public IEnumerable<CommonEntityDTO> GetEntityListByEntityType(EntityType entityType)
        {
            var entList = from e in _entityList
                where e.EntityType == entityType
                select e;
            return entList;
        }

        public IEnumerable<CommonEntityDTO> GetEntityListBySite(SiteDTO site)
        {
            var entityList = ExecuteSync(new ObjectModelServiceProxy().GetEntityListAsync, new GetEntityListParameterSet() {SiteId = site.Id});
            return entityList;
        }
        public IEnumerable<PropertyTypeDTO> GetPropertyTypeListByEntityType(EntityType entityType)
        {
            var o = (from et in _dictRepository.EntityTypes
                where et.EntityType == entityType
                select et).First().EntityProperties;
            return o;
        }

        public IEnumerable<PeriodTypeDTO> GetPeriodTypeList()
        {
            return _dictRepository.PeriodTypes;
        }
    }
}
