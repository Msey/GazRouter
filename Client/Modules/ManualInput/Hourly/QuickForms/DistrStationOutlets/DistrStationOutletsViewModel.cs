using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.ManualInput.Hourly.QuickForms.DistrStationOutlets
{
    public class DistrStationOutletsViewModel : QuickForm
    {
        public DistrStationOutletsViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new DistrStationOutletColumnVisibility(PropTypeList);

            Items = GenerateNewItemList(data, volumeUnits);

            OnPropertyChanged(() => Items);
        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData, volumeUnits);

            if (AreNewItemsOk<DistrStationOutletItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as DistrStationOutletItem;
                    var newitem = newItems[i] as DistrStationOutletItem;

                    MarkIfUpdated(item, newitem, it => it.PressureOutlet, ct => ct.PressureOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureOutlet, ct => ct.TemperatureOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.Flow, ct => ct.FlowUpdated = true);
                }
            }
        }

        private List<ItemBase> GenerateNewItemList(HourlyData data, VolumeUnits volumeUnits)
        {
            var itemList = new List<ItemBase>();
            foreach (var station in data.Tree.DistrStations)
            {
                foreach (var outlet in data.Tree.DistrStationOutlets.Where(o => o.ParentId == station.Id))
                {
                    if (data.InputOffEntities.Any(e => e.Id == outlet.Id)) continue;
                    itemList.Add(new DistrStationOutletItem(outlet, data.PropValues, data.Serie.KeyDate,
                        UpdatePropertyValue, PropTypeList, volumeUnits)
                    {
                        StationName = station.Name
                    });
                }
            }
            return itemList;
        }

        public override EntityType EntityType => EntityType.DistrStationOutlet;

        public DistrStationOutletColumnVisibility ColumnVisibility { get; set; }
    }


    public class DistrStationOutletColumnVisibility : ColumnVisibilityBase
    {
        public DistrStationOutletColumnVisibility(List<PropertyType> propType)
            : base(propType)
        { }


        public bool PressureOutlet => Visibility.GetOrDefault(PropertyType.PressureOutlet);
        public bool TemperatureOutlet => Visibility.GetOrDefault(PropertyType.TemperatureOutlet);
        public bool Flow => Visibility.GetOrDefault(PropertyType.Flow);
        
    }


    public class DistrStationOutletItem : ItemBase
    {
        public DistrStationOutletItem(DistrStationOutletDTO outlet, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes,
            VolumeUnits volumeUnits)
            : base(outlet, propValues, date, updateAction, propTypes, volumeUnits)
        {
            
        }

        public string StationName { get; set; }

        public DoubleMeasuringEditable PressureOutlet => Measurings.GetOrDefault(PropertyType.PressureOutlet);
        public DoubleMeasuringEditable TemperatureOutlet => Measurings.GetOrDefault(PropertyType.TemperatureOutlet);
        public DoubleMeasuringEditable Flow => Measurings.GetOrDefault(PropertyType.Flow);
    }
    
}