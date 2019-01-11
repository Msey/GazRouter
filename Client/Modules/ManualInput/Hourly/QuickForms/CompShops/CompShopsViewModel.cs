using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;


namespace GazRouter.ManualInput.Hourly.QuickForms.CompShops
{
    public class CompShopsViewModel : QuickForm
    {
        public CompShopsViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new CompShopColumnVisibility(PropTypeList);

            Items = GenerateNewItemList(data, volumeUnits);
        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData, volumeUnits);

            if (AreNewItemsOk<CompShopItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as CompShopItem;
                    var newitem = newItems[i] as CompShopItem;

                    MarkIfUpdated(item, newitem, it => it.Pumping, ct => ct.PumpingUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.CompressionStageCount, ct => ct.CompressionStageCountUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.CoolingUnitsInReserve, ct => ct.CoolingUnitsInReserveUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.CoolingUnitsInUse, ct => ct.CoolingUnitsInUseUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.CoolingUnitsUnderRepair, ct => ct.CoolingUnitsUnderRepairUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.DustCatchersInReserve, ct => ct.DustCatchersInReserveUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.DustCatchersInUse, ct => ct.DustCatchersInUseUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.DustCatchersUnderRepair, ct => ct.DustCatchersUnderRepairUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.FuelGasConsumption, ct => ct.FuelGasConsumptionUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.GroupCount, ct => ct.GroupCountUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.PressureInlet, ct => ct.PressureInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.PressureOutlet, ct => ct.PressureOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureCooling, ct => ct.TemperatureCoolingUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureInlet, ct => ct.TemperatureInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureOutlet, ct => ct.TemperatureOutletUpdated = true);    
                }
            }
        }

        public List<ItemBase> GenerateNewItemList(HourlyData data, VolumeUnits volumeUnits)
        {
            var tempItems = new List<ItemBase>();
            foreach (var station in data.Tree.CompStations)
            {
                foreach (var shop in data.Tree.CompShops.Where(s => s.ParentId == station.Id))
                {
                    if (data.InputOffEntities.Any(e => e.Id == shop.Id)) continue;
                    var item = new CompShopItem(shop, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue, PropTypeList, volumeUnits)
                    {
                        StationName = station.Name
                    };
                    tempItems.Add(item);
                }
            }
            return tempItems;
        }

        public override EntityType EntityType => EntityType.CompShop;
        
        public CompShopColumnVisibility ColumnVisibility { get; set; }
    }



    public class CompShopColumnVisibility : ColumnVisibilityBase
    {
        public CompShopColumnVisibility(List<PropertyType> propTypes)
            : base(propTypes) {}


        public bool GroupCount => Visibility.GetOrDefault(PropertyType.GroupCount);
        public bool CompressionStageCount => Visibility.GetOrDefault(PropertyType.CompressionStageCount);


        public bool PressureInlet => Visibility.GetOrDefault(PropertyType.PressureInlet);
        public bool PressureOutlet => Visibility.GetOrDefault(PropertyType.PressureOutlet);

        public bool TemperatureInlet => Visibility.GetOrDefault(PropertyType.TemperatureInlet);
        public bool TemperatureOutlet => Visibility.GetOrDefault(PropertyType.TemperatureOutlet);
        public bool TemperatureCooling => Visibility.GetOrDefault(PropertyType.TemperatureCooling);


        public bool FuelGasConsumption => Visibility.GetOrDefault(PropertyType.FuelGasConsumption);
        public bool Pumping => Visibility.GetOrDefault(PropertyType.Pumping);


        public bool CoolingUnitsInUse => Visibility.GetOrDefault(PropertyType.CoolingUnitsInUse);
        public bool CoolingUnitsInReserve => Visibility.GetOrDefault(PropertyType.CoolingUnitsInReserve);
        public bool CoolingUnitsUnderRepair => Visibility.GetOrDefault(PropertyType.CoolingUnitsUnderRepair);


        public bool DustCatchersInUse => Visibility.GetOrDefault(PropertyType.DustCatchersInUse);
        public bool DustCatchersInReserve => Visibility.GetOrDefault(PropertyType.DustCatchersInReserve);
        public bool DustCatchersUnderRepair => Visibility.GetOrDefault(PropertyType.DustCatchersUnderRepair);
    }


    public class CompShopItem : ItemBase
    {
        public CompShopItem(CompShopDTO shop, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes, 
            VolumeUnits volumeUnits)
            : base (shop, propValues, date, updateAction, propTypes, volumeUnits)
        {
            
        }

        public string StationName { get; set; }

        public DoubleMeasuringEditable GroupCount => Measurings.GetOrDefault(PropertyType.GroupCount);
        public DoubleMeasuringEditable CompressionStageCount => Measurings.GetOrDefault(PropertyType.CompressionStageCount);


        public DoubleMeasuringEditable PressureInlet => Measurings.GetOrDefault(PropertyType.PressureInlet);
        public DoubleMeasuringEditable PressureOutlet => Measurings.GetOrDefault(PropertyType.PressureOutlet);

        public DoubleMeasuringEditable TemperatureInlet => Measurings.GetOrDefault(PropertyType.TemperatureInlet);
        public DoubleMeasuringEditable TemperatureOutlet => Measurings.GetOrDefault(PropertyType.TemperatureOutlet);
        public DoubleMeasuringEditable TemperatureCooling => Measurings.GetOrDefault(PropertyType.TemperatureCooling);


        public DoubleMeasuringEditable FuelGasConsumption => Measurings.GetOrDefault(PropertyType.FuelGasConsumption);
        public DoubleMeasuringEditable Pumping => Measurings.GetOrDefault(PropertyType.Pumping);


        public DoubleMeasuringEditable CoolingUnitsInUse => Measurings.GetOrDefault(PropertyType.CoolingUnitsInUse);
        public DoubleMeasuringEditable CoolingUnitsInReserve => Measurings.GetOrDefault(PropertyType.CoolingUnitsInReserve);
        public DoubleMeasuringEditable CoolingUnitsUnderRepair => Measurings.GetOrDefault(PropertyType.CoolingUnitsUnderRepair);


        public DoubleMeasuringEditable DustCatchersInUse => Measurings.GetOrDefault(PropertyType.DustCatchersInUse);
        public DoubleMeasuringEditable DustCatchersInReserve => Measurings.GetOrDefault(PropertyType.DustCatchersInReserve);
        public DoubleMeasuringEditable DustCatchersUnderRepair => Measurings.GetOrDefault(PropertyType.DustCatchersUnderRepair);

    }
}