using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.ManualInput.Hourly.QuickForms.DistrStations
{
    public class DistrStationsViewModel : QuickForm
    {
        public DistrStationsViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new DistrStationColumnVisibility(PropTypeList);

            Items = GenerateNewItemList(data);

        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData);

            if (AreNewItemsOk<DistrStationItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as DistrStationItem;
                    var newitem = newItems[i] as DistrStationItem;

                    MarkIfUpdated(item, newitem, it => it.PressureInlet, ct => ct.PressureInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureInlet, ct => ct.TemperatureInletUpdated = true);
                }
            }
        }

        private List<ItemBase> GenerateNewItemList(HourlyData data)
        {
            var itemList = new List<ItemBase>();
            foreach (var station in data.Tree.DistrStations)
            {
                if (data.InputOffEntities.Any(e => e.Id == station.Id)) continue;
                itemList.Add(new DistrStationItem(station, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue,
                    PropTypeList));
            }
            return itemList;
        }

        public override EntityType EntityType => EntityType.DistrStation;

        public DistrStationColumnVisibility ColumnVisibility { get; set; }
    }


    public class DistrStationColumnVisibility : ColumnVisibilityBase
    {
        public DistrStationColumnVisibility(List<PropertyType> propTypes)
            : base(propTypes)
        { }


        public bool PressureInlet => Visibility.GetOrDefault(PropertyType.PressureInlet);
        public bool TemperatureInlet => Visibility.GetOrDefault(PropertyType.TemperatureInlet);
        
    }


    public class DistrStationItem : ItemBase
    {
        public DistrStationItem(DistrStationDTO station, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes)
            : base(station, propValues, date, updateAction, propTypes)
        {
            
        }

        public DoubleMeasuringEditable PressureInlet => Measurings.GetOrDefault(PropertyType.PressureInlet);

        public DoubleMeasuringEditable TemperatureInlet => Measurings.GetOrDefault(PropertyType.TemperatureInlet);
    }
    
}