using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{

    public class EntityElementModel : BoxedElementModel
    {
        public EntityElementModel()
        {
            IsTimestampVisible = true;
            SerieCount = 12;
            FontSize = 11;
            VisiblePropertyTypeList = new List<PropertyTypeDisplaySettings>();
        }


        /// <summary>
        /// Сущность
        /// </summary>
        public Guid EntityId { get; set; }


        /// <summary>
        /// Имя сущности
        /// </summary>
        public string EntityName { get; set; }


        /// <summary>
        /// Тип сущности
        /// </summary>
        public EntityType EntityType { get; set; }


        /// <summary>
        /// Кол-во отображаемых серий
        /// </summary>
        public int SerieCount { get; set; }


        /// <summary>
        /// Отображать ли столбец с метками времени
        /// </summary>
        public bool IsTimestampVisible { get; set; }


        /// <summary>
        /// Список видимых типов свойств
        /// </summary>
        public List<PropertyTypeDisplaySettings> VisiblePropertyTypeList { get; set; }


        /// <summary>
        /// Список индетификаторов сущностей, которые связаны с данным элементом. 
        /// Необходимо для формирования перечня сущностей для запроса получения данных.
        /// </summary>
        public override List<Guid> GetRelatedEntityList()
        {
            return new List<Guid> {EntityId};
        }

        private void CopyStyle(EntityElementModel other)
        {
            this.SerieCount = other.SerieCount;
            this.IsTimestampVisible = other.IsTimestampVisible;

            if (this.EntityType == other.EntityType)
            {
                this.VisiblePropertyTypeList = new List<PropertyTypeDisplaySettings>(other.VisiblePropertyTypeList);
            }
        }

        public override void CopyStyle(ElementModelBase other)
        {
            if (other is EntityElementModel)
                CopyStyle((EntityElementModel)other);

            base.CopyStyle(other);
        }
    }

    public class PropertyTypeDisplaySettings
    {
        public int PropertyTypeId { get; set; }
        public bool IsVisible { get; set; }
        public bool HideDoubles { get; set; }
        public bool CheckForBadValues { get; set; }

        public PropertyTypeDTO PropertyType
        {
            get
            {
                return ClientCache.DictionaryRepository.PropertyTypes.SingleOrDefault(pt => pt.Id == PropertyTypeId);
            }
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

    }
    public static class DefaultDisplaySettings
    {
        public static List<PropertyTypeDisplaySettings> GetDefaultPropertyTypeDisplaySettingsList(EntityType eType)
        {
            return GetDefaultPropertyTypeIdList(eType).Select(pt =>
                new PropertyTypeDisplaySettings
                {
                    PropertyTypeId = pt,
                    IsVisible = true
                }).ToList();
        }
        private static List<int> GetDefaultPropertyTypeIdList(EntityType eType)
        {
            switch (eType)
            {
                case EntityType.CompShop:
                    return new List<int> {11, 12, 39};
                case EntityType.DistrStation:
                    return new List<int> { 11, 13, 10 };
                case EntityType.CompUnit:
                    return new List<int> { 61, 62, 63, 64, 65 };
                case EntityType.ReducingStation:
                    return new List<int> { 11, 12 };
                case EntityType.MeasStation:
                    return new List<int> { 10 };
                case EntityType.MeasLine:
                    return new List<int> { 11, 13, 10 };
                case EntityType.DistrStationOutlet:
                    return new List<int> { 12, 14, 10 };
                case EntityType.MeasPoint:
                    return new List<int> { 15, 16, 23 };
                default:
                    return new List<int>();
            }
        }
    }
}