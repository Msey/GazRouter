using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.DTO.Dictionaries.PropertyTypes;


namespace GazRouter.ManualInput.Daily
{
    public class ConsumersViewModel : TabBaseViewModel
    {
        public override void Refresh(DailyData data, int coef = 1)
        {
            var date = data.Serie.KeyDate.Date;
            var prevDay = data.Serie.KeyDate.Date.AddDays(-1);

            // Сформировать дерево
            Items = new List<ItemBase>();

            var siteItem = new GroupItem
            {
                Entity = data.Site,
                FontWeight = FontWeights.Bold
            };

            foreach (var station in data.DistrStationTree.DistrStations)
            {
                var stationItem = new GroupItem
                {
                    Entity = station
                };

                foreach (var consumer in data.DistrStationTree.Consumers.Where(c => c.ParentId == station.Id))
                {
                    if (data.DisabledEntities.Any(e => e.Id == consumer.Id)) continue;
                    if (data.DistrStationTree.DistrStationOutlets.Any(o => o.ConsumerId == consumer.Id)) continue;

                    var consumerItem = new InputItem(consumer, data.Serie.Id, coef,
                        data.ValueExtractor.GetDayValue(consumer.Id, PropertyType.Flow, date)*coef ?? 0,
                        data.ValueExtractor.GetDayValue(consumer.Id, PropertyType.Flow, prevDay)*coef ?? 0,
                        date.Day > 1 ? data.ValueExtractor.GetMonthSum(consumer.Id, PropertyType.Flow, prevDay)*coef ?? 0 : 0);
                    stationItem.AddChild(consumerItem);
                }
                
                if (stationItem.Childs.Count > 0)
                    siteItem.AddChild(stationItem);
            }

            if (siteItem.Childs.Count > 0)
                Items.Add(siteItem);

            OnPropertyChanged(() => Items);
            OnPropertyChanged(() => HasItems);
        }
    }
}