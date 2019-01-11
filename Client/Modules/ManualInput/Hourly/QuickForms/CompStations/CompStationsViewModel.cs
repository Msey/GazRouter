using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.ManualInput.Hourly.QuickForms.CompStations
{
    public class CompStationsViewModel : QuickForm
    {
        public CompStationsViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new CompStationColumnVisibility(PropTypeList);

            Items = GenerateNewItemList(data);
        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {            
            var newItems = GenerateNewItemList(newData);

            if (AreNewItemsOk<CompStationItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as CompStationItem;
                    var newitem = newItems[i] as CompStationItem;

                    MarkIfUpdated(item, newitem, it => it.PressureAir, ct => ct.PressureUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureAir, ct => ct.TemperatureAirUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureEarth, ct => ct.TemperatureEarthUpdated = true);
                }
            }
        }

        private List<ItemBase> GenerateNewItemList(HourlyData data)
        {
            var itemList = new List<ItemBase>();
            foreach (var station in data.Tree.CompStations)
            {
                if (data.InputOffEntities.Any(e => e.Id == station.Id)) continue;                
                itemList.Add(new CompStationItem(station, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue,
                    PropTypeList));
            }
            return itemList;
        }

        public override EntityType EntityType => EntityType.CompStation;
        

        public CompStationColumnVisibility ColumnVisibility { get; set; }
    }


    public class CompStationColumnVisibility : ColumnVisibilityBase
    {
        public CompStationColumnVisibility(List<PropertyType> propTypes)
            : base(propTypes)
        { }


        public bool PressureAir => Visibility.GetOrDefault(PropertyType.PressureAir);
        public bool TemperatureAir => Visibility.GetOrDefault(PropertyType.TemperatureAir);
        public bool TemperatureEarth => Visibility.GetOrDefault(PropertyType.TemperatureEarth);

    }


    public class CompStationItem : ItemBase
    {
        public CompStationItem(CompStationDTO station, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes)
            : base(station, propValues, date, updateAction, propTypes)
        {
            
        }

        public DoubleMeasuringEditable PressureAir => Measurings.GetOrDefault(PropertyType.PressureAir);
        public DoubleMeasuringEditable TemperatureAir => Measurings.GetOrDefault(PropertyType.TemperatureAir);
        public DoubleMeasuringEditable TemperatureEarth => Measurings.GetOrDefault(PropertyType.TemperatureEarth);
    }
    
}