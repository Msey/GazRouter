using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.DTO.Dictionaries.PropertyTypes;


namespace GazRouter.ManualInput.Daily
{
    public class ReducingStationViewModel : TabBaseViewModel
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

            foreach (var station in data.ReducingStations)
            {
                if (data.DisabledEntities.Any(e => e.Id == station.Id)) continue;

                var stationItem = new InputItem(station, data.Serie.Id, coef,
                    data.ValueExtractor.GetDayValue(station.Id, PropertyType.Flow, date)*coef ?? 0,
                    data.ValueExtractor.GetDayValue(station.Id, PropertyType.Flow, prevDay)*coef ?? 0,
                    date.Day > 1 ? data.ValueExtractor.GetMonthSum(station.Id, PropertyType.Flow, prevDay)*coef ?? 0 : 0);
            
                siteItem.AddChild(stationItem);
            }

            if (siteItem.Childs.Count > 0)
                Items.Add(siteItem);

            OnPropertyChanged(() => Items);
            OnPropertyChanged(() => HasItems);
        }
    }
}