using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;


namespace GazRouter.ManualInput.Hourly.QuickForms.Valves
{
    public class ValvesViewModel : QuickForm
    {
        public ValvesViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new ValveColumnVisibility(PropTypeList);
            Items = GenerateNewItemList(data);            
        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData);

            if (AreNewItemsOk<ValveItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as ValveItem;
                    var newitem = newItems[i] as ValveItem;

                    MarkIfUpdated(item, newitem, it => it.PressureOutlet, ct => ct.PressureOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureOutlet, ct => ct.TemperatureOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.OpeningPercentage, ct => ct.OpeningPercentageUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureInlet, ct => ct.TemperatureInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.PressureInlet, ct => ct.PressureInletUpdated = true);
                }
            }
        }

        private List<ItemBase> GenerateNewItemList(HourlyData data)
        {
            var itemList = new List<ItemBase>();
            foreach (var valve in data.Valves)
            {
                if (data.InputOffEntities.Any(e => e.Id == valve.Id)) continue;
                itemList.Add(new ValveItem(valve, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue, PropTypeList));
            }
            return itemList;
        }
        public override EntityType EntityType => EntityType.Valve;

        public ValveColumnVisibility ColumnVisibility { get; set; }
    }



    public class ValveColumnVisibility : ColumnVisibilityBase
    {
        public ValveColumnVisibility(List<PropertyType> propTypes)
            : base(propTypes) {}


        public bool PressureInlet => Visibility.GetOrDefault(PropertyType.PressureInlet);
        public bool TemperatureInlet => Visibility.GetOrDefault(PropertyType.TemperatureInlet);
        public bool PressureOutlet => Visibility.GetOrDefault(PropertyType.PressureOutlet);
        public bool TemperatureOutlet => Visibility.GetOrDefault(PropertyType.TemperatureOutlet);
        public bool OpeningPercentage => Visibility.GetOrDefault(PropertyType.OpeningPercentage);
    }


    public class ValveItem : ItemBase
    {
        public ValveItem(ValveDTO valve,
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes)
            : base(valve, propValues, date, updateAction, propTypes)
        {
            
        }

        public ValveState State { get; set; }

        public DoubleMeasuringEditable PressureInlet => Measurings.GetOrDefault(PropertyType.PressureInlet);
        public DoubleMeasuringEditable TemperatureInlet => Measurings.GetOrDefault(PropertyType.TemperatureInlet);

        public DoubleMeasuringEditable PressureOutlet => Measurings.GetOrDefault(PropertyType.PressureOutlet);
        public DoubleMeasuringEditable TemperatureOutlet => Measurings.GetOrDefault(PropertyType.TemperatureOutlet);
        public DoubleMeasuringEditable OpeningPercentage => Measurings.GetOrDefault(PropertyType.OpeningPercentage);
        

    }
}