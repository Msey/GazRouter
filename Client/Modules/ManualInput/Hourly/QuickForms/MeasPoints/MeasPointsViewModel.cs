using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;


namespace GazRouter.ManualInput.Hourly.QuickForms.MeasPoints
{
    public class MeasPointsViewModel : QuickForm
    {
        public MeasPointsViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new MeasPointsColumnVisibility(PropTypeList);
            Items = GenerateNewItemList(data, volumeUnits);
        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData, volumeUnits);
                               
            if (AreNewItemsOk<MeasPointItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as MeasPointItem;
                    var newitem = newItems[i] as MeasPointItem;

                    MarkIfUpdated(item, newitem, it => it.ContentHelium, ct => ct.ContentHeliumUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ConcentrationSourSulfur, ct => ct.ConcentrationSourSulfurUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ConcentrationHydrogenSulfide, ct => ct.ConcentrationHydrogenSulfideUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ConcentrationOxygen, ct => ct.ConcentrationOxygenUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.Dryness, ct => ct.DrynessUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentImpurities, ct => ct.ContentImpuritiesUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.DewPointHydrocarbon, ct => ct.DewPointHydrocarbonUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentHexane, ct => ct.ContentHexaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentNeopentane, ct => ct.ContentNeopentaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentIsopentane, ct => ct.ContentIsopentaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentPentane, ct => ct.ContentPentaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentIsobutane, ct => ct.ContentIsobutaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentButane, ct => ct.ContentButaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentPropane, ct => ct.ContentPropaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentEthane, ct => ct.ContentEthaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentMethane, ct => ct.ContentMethaneUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentCarbonDioxid, ct => ct.ContentCarbonDioxidUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentNitrogen, ct => ct.ContentNitrogenUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.Wobbe, ct => ct.WobbeUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.CombustionHeatHigh, ct => ct.CombustionHeatHighUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.CombustionHeatLow, ct => ct.CombustionHeatLowUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.DensityStandard, ct => ct.DensityStandardUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.Density, ct => ct.DensityUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.DewPoint, ct => ct.DewPointUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.ContentHydrogen, ct => ct.ContentHydrogenUpdated = true);
                    
                }
            }
        }



        private List<ItemBase> GenerateNewItemList(HourlyData data, VolumeUnits volumeUnits)
        {
            var itemList = new List<ItemBase>();
            var points = data.Tree.MeasPoints.Where(p => data.InputOffEntities.All(e => e.Id != p.Id)).ToList();

            foreach (var point in points.Where(p => p.MeasLineId.HasValue))
            {
                var item = new MeasPointItem(point, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue,
                    PropTypeList, volumeUnits);

                var line = data.Tree.MeasLines.Single(l => l.Id == point.MeasLineId);
                if (data.Tree.MeasStations.All(s => s.Id != line.ParentId)) continue;
                item.ParentType = EntityType.MeasLine;
                item.ParentName = line.ShortPath;
                itemList.Add(item);
            }
            
            foreach (var point in points.Where(p => p.CompShopId.HasValue))
            {
                var item = new MeasPointItem(point, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue,
                    PropTypeList, volumeUnits);

                var shop = data.Tree.CompShops.Single(s => s.Id == point.CompShopId);
                if (data.Tree.CompStations.All(s => s.Id != shop.ParentId)) continue;
                item.ParentType = EntityType.CompShop;
                item.ParentName = shop.ShortPath;
                itemList.Add(item);
            }
            
            foreach (var point in points.Where(p => p.DistrStationId.HasValue))
            {
                var item = new MeasPointItem(point, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue,
                    PropTypeList, volumeUnits);

                if (data.Tree.DistrStations.All(s => s.Id != point.DistrStationId)) continue;
                item.ParentType = EntityType.DistrStation;
                item.ParentName = point.DistrStationName;
                itemList.Add(item);
            }
            
            return itemList;
        }

        public override EntityType EntityType => EntityType.MeasPoint;

        public MeasPointsColumnVisibility ColumnVisibility { get; set; }
    }



    public class MeasPointsColumnVisibility : ColumnVisibilityBase
    {
        public MeasPointsColumnVisibility(List<PropertyType> propTypes )
            : base(propTypes) {}


        public bool DewPoint => Visibility.GetOrDefault(PropertyType.DewPoint);
        public bool DewPointHydrocarbon => Visibility.GetOrDefault(PropertyType.DewPointHydrocarbon);

        public bool Density => Visibility.GetOrDefault(PropertyType.Density);
        public bool DensityStandard => Visibility.GetOrDefault(PropertyType.DensityStandard);

        public bool CombustionHeatLow => Visibility.GetOrDefault(PropertyType.CombustionHeatLow);
        public bool CombustionHeatHigh => Visibility.GetOrDefault(PropertyType.CombustionHeatHigh);
        public bool Wobbe => Visibility.GetOrDefault(PropertyType.Wobbe);


        public bool ContentNitrogen => Visibility.GetOrDefault(PropertyType.ContentNitrogen);
        public bool ContentCarbonDioxid => Visibility.GetOrDefault(PropertyType.ContentCarbonDioxid);
        public bool ContentMethane => Visibility.GetOrDefault(PropertyType.ContentMethane);
        public bool ContentEthane => Visibility.GetOrDefault(PropertyType.ContentEthane);
        public bool ContentPropane => Visibility.GetOrDefault(PropertyType.ContentPropane);
        public bool ContentButane => Visibility.GetOrDefault(PropertyType.ContentButane);
        public bool ContentIsobutane => Visibility.GetOrDefault(PropertyType.ContentIsobutane);
        public bool ContentPentane => Visibility.GetOrDefault(PropertyType.ContentPentane);
        public bool ContentIsopentane => Visibility.GetOrDefault(PropertyType.ContentIsopentane);
        public bool ContentNeopentane => Visibility.GetOrDefault(PropertyType.ContentNeopentane);
        public bool ContentHexane => Visibility.GetOrDefault(PropertyType.ContentHexane);
        public bool ContentHydrogen => Visibility.GetOrDefault(PropertyType.ContentHydrogen);
        public bool ContentHelium => Visibility.GetOrDefault(PropertyType.ContentHelium);


        public bool ConcentrationSourSulfur => Visibility.GetOrDefault(PropertyType.ConcentrationSourSulfur);
        public bool ConcentrationHydrogenSulfide => Visibility.GetOrDefault(PropertyType.ConcentrationHydrogenSulfide);
        public bool ConcentrationOxygen => Visibility.GetOrDefault(PropertyType.ConcentrationOxygen);
        public bool Dryness => Visibility.GetOrDefault(PropertyType.Dryness);
        public bool ContentImpurities => Visibility.GetOrDefault(PropertyType.ContentImpurities);

    }


    public class MeasPointItem : ItemBase
    {
        public MeasPointItem(MeasPointDTO line, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes,
            VolumeUnits volumeUnits)
            : base (line, propValues, date, updateAction, propTypes, volumeUnits)
        {
            
        }

        public string ParentName { get; set; }
        public EntityType ParentType { get; set; }

        
        public DoubleMeasuringEditable DewPoint => Measurings.GetOrDefault(PropertyType.DewPoint);
        public DoubleMeasuringEditable DewPointHydrocarbon => Measurings.GetOrDefault(PropertyType.DewPointHydrocarbon);

        public DoubleMeasuringEditable Density => Measurings.GetOrDefault(PropertyType.Density);
        public DoubleMeasuringEditable DensityStandard => Measurings.GetOrDefault(PropertyType.DensityStandard);

        public DoubleMeasuringEditable CombustionHeatLow => Measurings.GetOrDefault(PropertyType.CombustionHeatLow);
        public DoubleMeasuringEditable CombustionHeatHigh => Measurings.GetOrDefault(PropertyType.CombustionHeatHigh);
        public DoubleMeasuringEditable Wobbe => Measurings.GetOrDefault(PropertyType.Wobbe);


        public DoubleMeasuringEditable ContentNitrogen => Measurings.GetOrDefault(PropertyType.ContentNitrogen);
        public DoubleMeasuringEditable ContentCarbonDioxid => Measurings.GetOrDefault(PropertyType.ContentCarbonDioxid);
        public DoubleMeasuringEditable ContentMethane => Measurings.GetOrDefault(PropertyType.ContentMethane);
        public DoubleMeasuringEditable ContentEthane => Measurings.GetOrDefault(PropertyType.ContentEthane);
        public DoubleMeasuringEditable ContentPropane => Measurings.GetOrDefault(PropertyType.ContentPropane);
        public DoubleMeasuringEditable ContentButane => Measurings.GetOrDefault(PropertyType.ContentButane);
        public DoubleMeasuringEditable ContentIsobutane => Measurings.GetOrDefault(PropertyType.ContentIsobutane);
        public DoubleMeasuringEditable ContentPentane => Measurings.GetOrDefault(PropertyType.ContentPentane);
        public DoubleMeasuringEditable ContentIsopentane => Measurings.GetOrDefault(PropertyType.ContentIsopentane);
        public DoubleMeasuringEditable ContentNeopentane => Measurings.GetOrDefault(PropertyType.ContentNeopentane);
        public DoubleMeasuringEditable ContentHexane => Measurings.GetOrDefault(PropertyType.ContentHexane);
        public DoubleMeasuringEditable ContentHydrogen => Measurings.GetOrDefault(PropertyType.ContentHydrogen);
        public DoubleMeasuringEditable ContentHelium => Measurings.GetOrDefault(PropertyType.ContentHelium);


        public DoubleMeasuringEditable ConcentrationSourSulfur => Measurings.GetOrDefault(PropertyType.ConcentrationSourSulfur);
        public DoubleMeasuringEditable ConcentrationHydrogenSulfide => Measurings.GetOrDefault(PropertyType.ConcentrationHydrogenSulfide);
        public DoubleMeasuringEditable ConcentrationOxygen => Measurings.GetOrDefault(PropertyType.ConcentrationOxygen);
        public DoubleMeasuringEditable Dryness => Measurings.GetOrDefault(PropertyType.Dryness);
        public DoubleMeasuringEditable ContentImpurities => Measurings.GetOrDefault(PropertyType.ContentImpurities);

    }
}