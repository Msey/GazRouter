using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.DTO.Dictionaries.PropertyTypes;


namespace GazRouter.ManualInput.Daily
{
    public class DistrStationsViewModel : TabBaseViewModel
    {
        public override void Refresh(DailyData data, int coef = 1)
        {
            var date = data.Serie.KeyDate;
            var prevDay = date.Date.AddDays(-1);

            // Сформировать дерево
            Items = new List<ItemBase>();

            var siteItem = new GroupItem
            {
                Entity = data.Site,
                FontWeight = FontWeights.Bold
            };

            foreach (var station in data.DistrStationTree.DistrStations)
            {
                if (data.DisabledEntities.Any(e => e.Id == station.Id)) continue;

                var stationItem = new GroupItem { Entity = station };

                foreach (var outlet in data.DistrStationTree.DistrStationOutlets.Where(ml => ml.ParentId == station.Id))
                {
                    if (data.DisabledEntities.Any(e => e.Id == outlet.Id)) continue;

                    var outletItem = new InputItem(outlet, data.Serie.Id, coef,
                        data.ValueExtractor.GetDayValue(outlet.Id, PropertyType.Flow, date)*coef ?? 0,
                        data.ValueExtractor.GetDayValue(outlet.Id, PropertyType.Flow, prevDay)*coef ?? 0,
                        date.Day > 1 ? data.ValueExtractor.GetMonthSum(outlet.Id, PropertyType.Flow, prevDay)*coef ?? 0 : 0);
                
                    stationItem.AddChild(outletItem);
                }

                if (stationItem.Childs.Count > 0)
                {
                    siteItem.AddChild(stationItem);
                    CheckEntityList.Add(station.Id);
                }
            }

            if (siteItem.Childs.Count > 0)
                Items.Add(siteItem);

            OnPropertyChanged(() => Items);
            OnPropertyChanged(() => HasItems);
        }
    }
}