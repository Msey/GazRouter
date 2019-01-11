using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;
using Utils.ValueExtrators;


namespace GazRouter.ManualInput.Daily
{
    public class MeasStationsViewModel : TabBaseViewModel
    {
        private readonly Sign _sign;

        public MeasStationsViewModel(Sign sign)
        {
            _sign = sign;
        }


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

            foreach (var station in data.MeasStationTree.MeasStations.Where(s => s.BalanceSignId == _sign))
            {
                if (data.DisabledEntities.Any(e => e.Id == station.Id)) continue;

                var stationItem = new GroupItem { Entity = station };

                foreach (var line in data.MeasStationTree.MeasLines.Where(ml => ml.ParentId == station.Id))
                {
                    if (data.DisabledEntities.Any(e => e.Id == line.Id)) continue;

                    var lineItem = new InputItem(line, data.Serie.Id, coef,
                        data.ValueExtractor.GetDayValue(line.Id, PropertyType.Flow, date)*coef ?? 0,
                        data.ValueExtractor.GetDayValue(line.Id, PropertyType.Flow, prevDay)*coef ?? 0,
                        date.Day > 1 ? data.ValueExtractor.GetMonthSum(line.Id, PropertyType.Flow, prevDay)*coef ?? 0 : 0);

                    stationItem.AddChild(lineItem);
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