using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;
using System.Diagnostics;

namespace GazRouter.ManualInput.Hourly.QuickForms.CompUnits
{
    public class CompUnitsViewModel : QuickForm
    {
        public CompUnitsViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new CompUnitColumnVisibility(PropTypeList);
            Items =GenerateNewItemList(data, volumeUnits);
        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData, volumeUnits);

            if (AreNewItemsOk<CompUnitItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as CompUnitItem;
                    var newitem = newItems[i] as CompUnitItem;

                    MarkIfUpdated(item, newitem, it => it.PressureSuperchargerInlet, ct => ct.PressureSuperchargerInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.PressureSuperchargerOutlet, ct => ct.PressureSuperchargerOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureSuperchargerInlet, ct => ct.TemperatureSuperchargerInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureSuperchargerOutlet, ct => ct.TemperatureSuperchargerOutletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.RpmSupercharger, ct => ct.RpmSuperchargerUpdated = true);
                }
            }
        }
        private List<ItemBase> GenerateNewItemList(HourlyData data, VolumeUnits volumeUnits)
        {
            var itemList = new List<ItemBase>();

            foreach (var station in data.Tree.CompStations)
            {
                foreach (var shop in data.Tree.CompShops.Where(s => s.ParentId == station.Id))
                {
                    foreach (var unit in data.Tree.CompUnits.Where(u => u.ParentId == shop.Id))
                    {
                        if (data.CompUnitStates.Any(s => s.CompUnitId == unit.Id && s.State == CompUnitState.Work))
                        {
                            if (data.InputOffEntities.Any(e => e.Id == unit.Id)) continue;
                            var item = new CompUnitItem(unit, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue, PropTypeList, volumeUnits)
                            {
                                ShopName = shop.Name,
                                StationName = station.Name,
                                State = CompUnitState.Work
                            };
                            itemList.Add(item);
                        }

                    }
                }
            }

            return itemList;
        }
        public override EntityType EntityType => EntityType.CompUnit;

        public CompUnitColumnVisibility ColumnVisibility { get; set; }
    }



    public class CompUnitColumnVisibility : ColumnVisibilityBase
    {
        public CompUnitColumnVisibility(List<PropertyType> propType)
            : base(propType) {}

        public bool PressureSuperchargerInlet => Visibility.GetOrDefault(PropertyType.PressureSuperchargerInlet);
        public bool PressureSuperchargerOutlet => Visibility.GetOrDefault(PropertyType.PressureSuperchargerOutlet);

        public bool TemperatureSuperchargerInlet => Visibility.GetOrDefault(PropertyType.TemperatureSuperchargerInlet);
        public bool TemperatureSuperchargerOutlet => Visibility.GetOrDefault(PropertyType.TemperatureSuperchargerOutlet);


        public bool FuelGasConsumption => Visibility.GetOrDefault(PropertyType.FuelGasConsumption);
        public bool Pumping => Visibility.GetOrDefault(PropertyType.Pumping);


        public bool RpmSupercharger => Visibility.GetOrDefault(PropertyType.RpmSupercharger);
        public bool RpmHighHeadTurbine => Visibility.GetOrDefault(PropertyType.RpmSupercharger);
        public bool RpmLowHeadTurbine => Visibility.GetOrDefault(PropertyType.RpmSupercharger);


        public bool TemperatureHighHeadTurbineInlet => Visibility.GetOrDefault(PropertyType.TemperatureHighHeadTurbineInlet);
        public bool TemperatureHighHeadTurbineOutlet => Visibility.GetOrDefault(PropertyType.TemperatureHighHeadTurbineOutlet);

        public bool TemperatureLowHeadTurbineInlet => Visibility.GetOrDefault(PropertyType.TemperatureLowHeadTurbineInlet);
        public bool TemperatureLowHeadTurbineOutlet => Visibility.GetOrDefault(PropertyType.TemperatureLowHeadTurbineOutlet);

        public bool TemperatureFreeTurbineInlet => Visibility.GetOrDefault(PropertyType.TemperatureFreeTurbineInlet);
        public bool TemperatureFreeTurbineOutlet => Visibility.GetOrDefault(PropertyType.TemperatureFreeTurbineOutlet);

        public bool PressureAxialFlowCompressorOutlet => Visibility.GetOrDefault(PropertyType.PressureAxialFlowCompressorOutlet);
        public bool TemperatureAxialFlowCompressorInlet => Visibility.GetOrDefault(PropertyType.TemperatureAxialFlowCompressorInlet);

        public bool TemperatureBearing => Visibility.GetOrDefault(PropertyType.TemperatureBearing);
        public bool PressureFallConfusor => Visibility.GetOrDefault(PropertyType.PressureFallConfusor);

        
    }


    public class CompUnitItem : ItemBase
    {
        public CompUnitItem(CompUnitDTO unit, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes,
            VolumeUnits volumeUnits)
            : base (unit, propValues, date, updateAction, propTypes, volumeUnits) {}

        public string StationName { get; set; }
        public string ShopName { get; set; }

        public CompUnitState State { get; set; }


        public DoubleMeasuringEditable PressureSuperchargerInlet => Measurings.GetOrDefault(PropertyType.PressureSuperchargerInlet);
        public DoubleMeasuringEditable PressureSuperchargerOutlet => Measurings.GetOrDefault(PropertyType.PressureSuperchargerOutlet);

        public DoubleMeasuringEditable TemperatureSuperchargerInlet => Measurings.GetOrDefault(PropertyType.TemperatureSuperchargerInlet);
        public DoubleMeasuringEditable TemperatureSuperchargerOutlet => Measurings.GetOrDefault(PropertyType.TemperatureSuperchargerOutlet);


        public DoubleMeasuringEditable FuelGasConsumption => Measurings.GetOrDefault(PropertyType.FuelGasConsumption);
        public DoubleMeasuringEditable Pumping => Measurings.GetOrDefault(PropertyType.Pumping);


        public DoubleMeasuringEditable RpmSupercharger => Measurings.GetOrDefault(PropertyType.RpmSupercharger);
        public DoubleMeasuringEditable RpmHighHeadTurbine => Measurings.GetOrDefault(PropertyType.RpmSupercharger);
        public DoubleMeasuringEditable RpmLowHeadTurbine => Measurings.GetOrDefault(PropertyType.RpmSupercharger);


        public DoubleMeasuringEditable TemperatureHighHeadTurbineInlet => Measurings.GetOrDefault(PropertyType.TemperatureHighHeadTurbineInlet);
        public DoubleMeasuringEditable TemperatureHighHeadTurbineOutlet => Measurings.GetOrDefault(PropertyType.TemperatureHighHeadTurbineOutlet);

        public DoubleMeasuringEditable TemperatureLowHeadTurbineInlet => Measurings.GetOrDefault(PropertyType.TemperatureLowHeadTurbineInlet);
        public DoubleMeasuringEditable TemperatureLowHeadTurbineOutlet => Measurings.GetOrDefault(PropertyType.TemperatureLowHeadTurbineOutlet);

        public DoubleMeasuringEditable TemperatureFreeTurbineInlet => Measurings.GetOrDefault(PropertyType.TemperatureFreeTurbineInlet);
        public DoubleMeasuringEditable TemperatureFreeTurbineOutlet => Measurings.GetOrDefault(PropertyType.TemperatureFreeTurbineOutlet);

        public DoubleMeasuringEditable PressureAxialFlowCompressorOutlet => Measurings.GetOrDefault(PropertyType.PressureAxialFlowCompressorOutlet);
        public DoubleMeasuringEditable TemperatureAxialFlowCompressorInlet => Measurings.GetOrDefault(PropertyType.TemperatureAxialFlowCompressorInlet);

        public DoubleMeasuringEditable TemperatureBearing => Measurings.GetOrDefault(PropertyType.TemperatureBearing);
        public DoubleMeasuringEditable PressureFallConfusor => Measurings.GetOrDefault(PropertyType.PressureFallConfusor);
    }
}