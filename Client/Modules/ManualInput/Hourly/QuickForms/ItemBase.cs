using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.ObjectBuilder2;


namespace GazRouter.ManualInput.Hourly.QuickForms
{
    public class ItemBase : ViewModelBase
    {
        public ItemBase(CommonEntityDTO entity,
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues, 
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes,
            VolumeUnits volumeUnits = VolumeUnits.Km)
        {
            Entity = entity;
            Measurings = new Dictionary<PropertyType, DoubleMeasuringEditable>();
            
            var eType = ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == Entity.EntityType);
            foreach (var propType in eType.EntityProperties.Where(p => propTypes.Contains(p.PropertyType)))
            {
                var coef = propType.PhysicalType.PhysicalType == PhysicalType.Volume && volumeUnits == VolumeUnits.M
                    ? 1000
                    : 1;
                var meas = new DoubleMeasuringEditable(Entity.Id, propType.PropertyType, updateAction, coef);
                meas.Extract(propValues, date);
                Measurings.Add(propType.PropertyType, meas);
            }

            ItemColorTemplate = new CellColorTemplate();
        }
        
        public CommonEntityDTO Entity { get; set; }

        public CellColorTemplate ItemColorTemplate { get; set; }

        public Dictionary<PropertyType, DoubleMeasuringEditable> Measurings { get; set; }

        // Копирует значения параметров с предыдущего сеанса
        // Если введено текущение значение, оно не перезаписывается
        public void CopyMeasurings()
        {
            Measurings.Where(m => m.Value.PrevValue.HasValue && !m.Value.Value.HasValue)
                .ForEach(
                    m =>
                        m.Value.EditableValue =
                            UserProfile.ToUserUnits(m.Value.PrevValue.Value, m.Value.PropertyType.PropertyType));
        }
        
    }

    public enum VolumeUnits
    {
        M = 1, // м3
        Km = 2 // тыс.м3
    }
}