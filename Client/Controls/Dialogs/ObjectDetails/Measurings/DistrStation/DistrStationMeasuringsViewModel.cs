using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Controls.Dialogs.ObjectDetails.Measurings.DistrStation
{
    public class DistrStationMeasuringsViewModel : ValidationViewModel
    {
        private readonly Guid _stationId;
        private DateTime _timestamp;

        public DistrStationMeasuringsViewModel(Guid id)
        {
            _stationId = id;
            _timestamp = SeriesHelper.GetLastCompletedSession();
            LoadMainData();
        }

        public List<GridItem> Items { get; set; }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (SetProperty(ref _timestamp, value))
                {
                    LoadMainData();
                }
            }
        }

        private async void LoadMainData()
        {
            try
            {
                Behavior.TryLock();

                // Получение паспортной информации о ГРС 
                var tree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                    new GetDistrStationListParameterSet
                    {
                        StationId = _stationId
                    });

                var station = tree.DistrStations.First();
                var outlets = tree.DistrStationOutlets;

                // Получение измеренных значений по ГРС и ее выходам
                var entities = new List<Guid> {station.Id};
                entities.AddRange(outlets.Select(o => o.Id));

                var values = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = entities,
                        CreateEmpty = true,
                        StartDate = Timestamp,
                        EndDate = Timestamp,
                        LoadMessages = true,
                        PeriodType = PeriodType.Twohours
                    });

                Items = new List<GridItem>();

                var stationItem = new GridItem
                {
                    Name = station.Name,
                    TypeName = "ГРС",
                    P = new DoubleMeasuring(station.Id, PropertyType.PressureInlet, PeriodType.Twohours, true),
                    T = new DoubleMeasuring(station.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, true),
                    Q = new DoubleMeasuring(station.Id, PropertyType.Flow, PeriodType.Twohours, true)
                };
                Items.Add(stationItem);
                stationItem.P.Extract(values, Timestamp);
                stationItem.T.Extract(values, Timestamp);
                stationItem.Q.Extract(values, Timestamp);

                foreach (var o in outlets)
                {
                    var outItem = new GridItem
                    {
                        Name = o.Name,
                        TypeName = "Выход",
                        P = new DoubleMeasuring(o.Id, PropertyType.PressureOutlet, PeriodType.Twohours, true),
                        T = new DoubleMeasuring(o.Id, PropertyType.TemperatureOutlet, PeriodType.Twohours, true),
                        Q = new DoubleMeasuring(o.Id, PropertyType.Flow, PeriodType.Twohours, true)
                    };
                    stationItem.Childs.Add(outItem);
                    outItem.P.Extract(values, Timestamp);
                    outItem.T.Extract(values, Timestamp);
                    outItem.Q.Extract(values, Timestamp);
                }

                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
    }
}