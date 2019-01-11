using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.ManualInput.Hourly.QuickForms
{
    public abstract class QuickForm : LockableViewModel
    {
        private readonly int _serieId;
        protected QuickForm(HourlyData data)
        {
            _serieId = data.Serie.Id;
            PropTypeList =
                data.PropTypes.Where(pt => pt.EntityType == EntityType && pt.IsInput)
                    .Select(pt => pt.PropertyType)
                    .ToList();

            var statusList = data.StatusList.Where(s => s.EntityType == EntityType).ToList();

            WrongEntityCount = statusList.Count;
            OnPropertyChanged(() => WrongEntityCount);

            ValidationStatus = data.IsChecked ? EntityValidationStatus.Good : EntityValidationStatus.NotChecked;
            if (statusList.Any(s => s.Status == EntityValidationStatus.Alarm))
                ValidationStatus = EntityValidationStatus.Alarm;
            if (statusList.Any(s => s.Status == EntityValidationStatus.Error))
                ValidationStatus = EntityValidationStatus.Error;
            OnPropertyChanged(() => ValidationStatus);
        }
        
        public virtual EntityType EntityType { get; set; }

        public List<PropertyType> PropTypeList { get; set; } 
        
        public bool IsReadOnly {get; set; }
        
        public List<ItemBase> Items { get; set; }
        
        public int? WrongEntityCount { get; set; }

        public EntityValidationStatus ValidationStatus { get; set; }

        public virtual List<Guid> CheckList => Items?.Select(i => i.Entity.Id).ToList();

        public virtual void HighlightUpdates(HourlyData newData, VolumeUnits volUnits)
        {

        }

        protected async void UpdatePropertyValue(Guid entityId, PropertyType propType, double value)
        {
            if (entityId != Guid.Empty)
            {
                IsPendingChanges= IsOldRead = true;
               var values = new List<SetPropertyValueParameterSet>
                {
                    new SetPropertyValueParameterSet
                    {
                        SeriesId = _serieId,
                        EntityId = entityId,
                        PropertyTypeId = propType,
                        Value = value
                    }
                };
                await new SeriesDataServiceProxy().SetPropertyValueAsync(values);
                IsPendingChanges = false;
            }
        }

        public bool IsPendingChanges { get; set; }

        public bool IsOldRead { get; set; }

        public void CopyValues()
        {
            Items?.ForEach(i => i.CopyMeasurings());
        }

        protected bool AreNewItemsOk<T>(List<ItemBase> newItems) where T : class
        {
            return
                newItems.Count == Items.Count &&
                (newItems.Union(Items)).All(item => (item as T) != null);
        }
        
        protected void MarkIfUpdated<T>(T oldItem, T newItem, Func<T, DoubleMeasuringEditable> getValue, Action<CellColorTemplate> setter) where T : ItemBase
        {
            if (oldItem == null || newItem == null)
                return;            

            var oldMes = getValue(oldItem);
            var newMes = getValue(newItem);

            if (oldMes != null && newMes != null)
            {
                if (oldMes.EditableValue != newMes.EditableValue)
                {                    
                    oldMes.EditableValue = newMes.EditableValue;
                    setter(oldItem.ItemColorTemplate);
                }
            }
        }
    }
}