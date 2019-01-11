using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.ChemicalTests
{
    public class ChemicalTestsViewModel : FormViewModelBase
    {

        public override ReportSettings GetReportSettings()
        {
            return new ReportSettings
            {
                SiteSelector = true,
                SerieSelector = true
            };
        }

        
        public List<ItemBase> Items { get; set; } 
        
        
        public override async void Refresh()
        {
            if (Site == null) return;

            Lock();

            var measStations = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
                new GetMeasStationListParameterSet {SiteId = Site.Id});

            var distrStations = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                new GetDistrStationListParameterSet
                {
                    SiteId = Site.Id
                });

            var compStations = await new ObjectModelServiceProxy().GetCompStationTreeAsync(Site.Id);

            var measPointList = measStations.MeasPoints;
            measPointList.AddRange(distrStations.MeasPoints);
            measPointList.AddRange(compStations.MeasPoints);

            var values = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    StartDate = Timestamp,
                    EndDate = Timestamp,
                    PeriodType = PeriodType.Twohours,
                    LoadMessages = true,
                    CreateEmpty = true,
                    EntityIdList = measPointList.Select(p => p.Id).ToList()
                });

            Items = new List<ItemBase>();

            foreach (var station in measStations.MeasStations)
            {
                var stationItem = new ItemBase(station);
                foreach (var line in measStations.MeasLines.Where(l => l.ParentId == station.Id))
                {
                    var lineItem = new ItemBase(line);
                    foreach (var point in measStations.MeasPoints.Where(p => p.ParentId == line.Id))
                    {
                        var pointItem = new MeasPointItem(point, values, Timestamp);
                        lineItem.Childs.Add(pointItem);
                    }
                    if (lineItem.Childs.Any())
                        stationItem.Childs.Add(lineItem);
                }
                if (stationItem.Childs.Any())
                    Items.Add(stationItem);
            }

            foreach (var station in compStations.CompStations)
            {
                var stationItem = new ItemBase(station);
                foreach (var shop in compStations.CompShops.Where(c => c.ParentId == station.Id))
                {
                    var shopItem = new ItemBase(shop);
                    foreach (var point in compStations.MeasPoints.Where(p => p.ParentId == shop.Id))
                    {
                        var pointItem = new MeasPointItem(point, values, Timestamp);
                        shopItem.Childs.Add(pointItem);
                    }
                    if (shopItem.Childs.Any())
                        stationItem.Childs.Add(shopItem);
                }
                if (stationItem.Childs.Any())
                    Items.Add(stationItem);
            }

            foreach (var station in distrStations.DistrStations)
            {
                var stationItem = new ItemBase(station);
                foreach (var point in distrStations.MeasPoints.Where(p => p.ParentId == station.Id))
                {
                    var pointItem = new MeasPointItem(point, values, Timestamp);
                    stationItem.Childs.Add(pointItem);
                }
                if (stationItem.Childs.Any())
                    Items.Add(stationItem);
            }

            
            OnPropertyChanged(() => Items);

            Unlock();

        }
    }

    public class ItemBase
    {
        public ItemBase(CommonEntityDTO entity)
        {
            Entity = entity;
            Childs = new List<ItemBase>();
        }

        public CommonEntityDTO Entity { get; }

        public List<ItemBase> Childs { get; set; } 
    }

    public class MeasPointItem : ItemBase
    {
        private readonly Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> _values;
        private readonly DateTime _timestamp;

        public MeasPointItem(CommonEntityDTO entity,
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> values,
            DateTime timestamp)
            : base(entity)
        {
            _values = values;
            _timestamp = timestamp;
        }

        private DoubleMeasuring GetValue(PropertyType pType)
        {
            var val = new DoubleMeasuring(Entity.Id, pType, PeriodType.Twohours);
            val.Extract(_values, _timestamp);
            return val;

        }

        public DoubleMeasuring DewPoint => GetValue(PropertyType.DewPoint);
        public DoubleMeasuring DewPointHydrocarbon => GetValue(PropertyType.DewPointHydrocarbon);

        public DoubleMeasuring Density => GetValue(PropertyType.Density);
        public DoubleMeasuring DensityStandard => GetValue(PropertyType.DensityStandard);

        public DoubleMeasuring CombustionHeatLow => GetValue(PropertyType.CombustionHeatLow);
        public DoubleMeasuring CombustionHeatHigh => GetValue(PropertyType.CombustionHeatHigh);
        public DoubleMeasuring Wobbe => GetValue(PropertyType.Wobbe);


        public DoubleMeasuring ContentNitrogen => GetValue(PropertyType.ContentNitrogen);
        public DoubleMeasuring ContentCarbonDioxid => GetValue(PropertyType.ContentCarbonDioxid);
        public DoubleMeasuring ContentMethane => GetValue(PropertyType.ContentMethane);
        public DoubleMeasuring ContentEthane => GetValue(PropertyType.ContentEthane);
        public DoubleMeasuring ContentPropane => GetValue(PropertyType.ContentPropane);
        public DoubleMeasuring ContentButane => GetValue(PropertyType.ContentButane);
        public DoubleMeasuring ContentIsobutane => GetValue(PropertyType.ContentIsobutane);
        public DoubleMeasuring ContentPentane => GetValue(PropertyType.ContentPentane);
        public DoubleMeasuring ContentIsopentane => GetValue(PropertyType.ContentIsopentane);
        public DoubleMeasuring ContentNeopentane => GetValue(PropertyType.ContentNeopentane);
        public DoubleMeasuring ContentHexane => GetValue(PropertyType.ContentHexane);
        public DoubleMeasuring ContentHydrogen => GetValue(PropertyType.ContentHydrogen);
        public DoubleMeasuring ContentHelium => GetValue(PropertyType.ContentHelium);


        public DoubleMeasuring ConcentrationSourSulfur => GetValue(PropertyType.ConcentrationSourSulfur);
        public DoubleMeasuring ConcentrationHydrogenSulfide => GetValue(PropertyType.ConcentrationHydrogenSulfide);
        public DoubleMeasuring ConcentrationOxygen => GetValue(PropertyType.ConcentrationOxygen);
        public DoubleMeasuring Dryness => GetValue(PropertyType.Dryness);
        public DoubleMeasuring ContentImpurities => GetValue(PropertyType.ContentImpurities);


    }
}
