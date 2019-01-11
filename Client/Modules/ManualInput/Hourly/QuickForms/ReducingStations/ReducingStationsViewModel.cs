using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;


namespace GazRouter.ManualInput.Hourly.QuickForms.ReducingStations
{
    public class ReducingStationsViewModel : QuickForm
    {
        public ReducingStationsViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new ReducingStationColumnVisibility(PropTypeList);

            Items = GenerateNewItemList(data, volumeUnits);

        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData, volumeUnits);

            if (AreNewItemsOk<ReducingStationItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as ReducingStationItem;
                    var newitem = newItems[i] as ReducingStationItem;

                    MarkIfUpdated(item, newitem, it => it.PressureOutlet, ct => ct.PressureOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureOutlet, ct => ct.TemperatureOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.Flow, ct => ct.FlowUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureInlet, ct => ct.TemperatureInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.OpeningPercentage, ct => ct.OpeningPercentageUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.PressureInlet, ct => ct.PressureInletUpdated = true);
                }
            }
        }

        private List<ItemBase> GenerateNewItemList(HourlyData data, VolumeUnits volumeUnits)
        {
            var itemList = new List<ItemBase>();
            foreach (var station in data.Tree.ReducingStations)
            {
                if (data.InputOffEntities.Any(e => e.Id == station.Id)) continue;
                itemList.Add(new ReducingStationItem(station, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue,
                    PropTypeList, volumeUnits));
            }
            return itemList;
        }
        public override EntityType EntityType => EntityType.ReducingStation;
        
        public ReducingStationColumnVisibility ColumnVisibility { get; set; }
    }



    public class ReducingStationColumnVisibility : ColumnVisibilityBase
    {
        public ReducingStationColumnVisibility(List<PropertyType> propTypes )
            : base(propTypes) {}


        public bool PressureInlet => Visibility.GetOrDefault(PropertyType.PressureInlet);
        public bool TemperatureInlet => Visibility.GetOrDefault(PropertyType.TemperatureInlet);
        public bool PressureOutlet => Visibility.GetOrDefault(PropertyType.PressureOutlet);
        public bool TemperatureOutlet => Visibility.GetOrDefault(PropertyType.TemperatureOutlet);
        public bool Flow => Visibility.GetOrDefault(PropertyType.Flow);
        public bool OpeningPercentage => Visibility.GetOrDefault(PropertyType.OpeningPercentage);
    }


    public class ReducingStationItem : ItemBase
    {
        public ReducingStationItem(ReducingStationDTO station, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes,
            VolumeUnits volumeUnits)
            : base (station, propValues, date, updateAction, propTypes, volumeUnits)
        {
            
        }
        
        public DoubleMeasuringEditable PressureInlet => Measurings.GetOrDefault(PropertyType.PressureInlet);
        public DoubleMeasuringEditable TemperatureInlet => Measurings.GetOrDefault(PropertyType.TemperatureInlet);
        public DoubleMeasuringEditable PressureOutlet => Measurings.GetOrDefault(PropertyType.PressureOutlet);
        public DoubleMeasuringEditable TemperatureOutlet => Measurings.GetOrDefault(PropertyType.TemperatureOutlet);
        public DoubleMeasuringEditable Flow => Measurings.GetOrDefault(PropertyType.Flow);
        public DoubleMeasuringEditable OpeningPercentage => Measurings.GetOrDefault(PropertyType.OpeningPercentage);
    }
}