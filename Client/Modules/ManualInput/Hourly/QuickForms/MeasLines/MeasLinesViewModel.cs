using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;


namespace GazRouter.ManualInput.Hourly.QuickForms.MeasLines
{
    public class MeasLinesViewModel : QuickForm
    {
        public MeasLinesViewModel(HourlyData data, VolumeUnits volumeUnits)
            : base(data)
        {
            ColumnVisibility = new MeasLinesColumnVisibility(PropTypeList);
            
            CheckList = new List<Guid>();
            Items = GenerateNewItemList(data, volumeUnits);
        }

        public override void HighlightUpdates(HourlyData newData, VolumeUnits volumeUnits)
        {
            var newItems = GenerateNewItemList(newData, volumeUnits, false);

            if (AreNewItemsOk<MeasLineItem>(newItems))
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    var item = Items[i] as MeasLineItem;
                    var newitem = newItems[i] as MeasLineItem;

                    MarkIfUpdated(item, newitem, it => it.PressureInlet, ct => ct.PressureInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.TemperatureInlet, ct => ct.TemperatureInletUpdated = true);
                    MarkIfUpdated(item, newitem, it => it.Flow, ct => ct.FlowUpdated = true);

                }
            }
        }

        private List<ItemBase> GenerateNewItemList(HourlyData data, VolumeUnits volumeUnits, bool withCheckList=true)
        {
            var itemList = new List<ItemBase>();
            foreach (var station in data.Tree.MeasStations)
            {
                CheckList.Add(station.Id);
                foreach (var line in data.Tree.MeasLines.Where(l => l.ParentId == station.Id))
                {
                    if (data.InputOffEntities.Any(e => e.Id == line.Id)) continue;
                    itemList.Add(new MeasLineItem(line, data.PropValues, data.Serie.KeyDate, UpdatePropertyValue,
                        PropTypeList, volumeUnits)
                    { StationName = station.Name });
                   if(withCheckList) CheckList.Add(line.Id);
                }
            }
            return itemList;
        }

        public override EntityType EntityType => EntityType.MeasLine;

        public override List<Guid> CheckList { get; }

        public MeasLinesColumnVisibility ColumnVisibility { get; set; }
    }



    public class MeasLinesColumnVisibility : ColumnVisibilityBase
    {
        public MeasLinesColumnVisibility(List<PropertyType> propTypes )
            : base(propTypes) {}


        public bool PressureInlet => Visibility.GetOrDefault(PropertyType.PressureInlet);
        public bool TemperatureInlet => Visibility.GetOrDefault(PropertyType.TemperatureInlet);
        public bool Flow => Visibility.GetOrDefault(PropertyType.Flow);
    }


    public class MeasLineItem : ItemBase
    {
        public MeasLineItem(MeasLineDTO line, 
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues,
            DateTime date,
            Action<Guid, PropertyType, double> updateAction,
            List<PropertyType> propTypes,
            VolumeUnits volumeUnits)
            : base (line, propValues, date, updateAction, propTypes, volumeUnits)
        {
            
        }

        public string StationName { get; set; }

        public DoubleMeasuringEditable PressureInlet => Measurings.GetOrDefault(PropertyType.PressureInlet);
        public DoubleMeasuringEditable TemperatureInlet => Measurings.GetOrDefault(PropertyType.TemperatureInlet);
        public DoubleMeasuringEditable Flow => Measurings.GetOrDefault(PropertyType.Flow);

    }
}