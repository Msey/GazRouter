using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.SeriesData.PropertyValues;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;


namespace GazRouter.Modes.ProcessMonitoring.PipelineTrend
{
    public class PipelineTrendViewModel : ViewModelBase
    {
        private Guid _pipeId;
        private DateTime _timestamp;


        public PipelineTrendViewModel(Guid pipeId)
        {
            _timestamp = SeriesHelper.GetLastCompletedSession();
            _pipeId = pipeId;
            Refresh();
        }

        public PipelineTrendViewModel(Guid pipeId, DateTime ts)
        {
            _timestamp = ts;
            _pipeId = pipeId;
            Refresh();
        }

        public async void Refresh()
        {
            try
            {
                Behavior.TryLock();

                // Получить список КЦ
                var shopList = await new ObjectModelServiceProxy().GetCompShopListAsync(
                    new GetCompShopListParameterSet
                    {
                        PipelineId = _pipeId
                    });

                // получаем значения измеренных параметров
                var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = shopList.Select(s => s.Id).ToList(),
                        StartDate = _timestamp.AddHours(-2),
                        EndDate = _timestamp,
                        PeriodType = PeriodType.Twohours,
                        LoadMessages = true
                    });



                PressureByKmList = new List<PressureByKmPoint>();

                foreach (var shop in shopList.OrderBy(s => s.KmOfConn))
                {
                    if (shop.KmOfConn.HasValue)
                    {
                        var pressureInlet = new DoubleMeasuring(shop.Id, PropertyType.PressureInlet, PeriodType.Twohours, true);
                        pressureInlet.Extract(propValues, _timestamp);
                        if (pressureInlet.UserValue.HasValue)
                            PressureByKmList.Add(new PressureByKmPoint(shop.KmOfConn.Value, pressureInlet.UserValue.Value));

                        var pressureOutlet = new DoubleMeasuring(shop.Id, PropertyType.PressureOutlet, PeriodType.Twohours, true);
                        pressureOutlet.Extract(propValues, _timestamp);
                        if (pressureOutlet.UserValue.HasValue)
                            PressureByKmList.Add(new PressureByKmPoint(shop.KmOfConn.Value, pressureOutlet.UserValue.Value));
                    }
                }

                
                // промежуточные точки
                if (PressureByKmList.Count >= 2)
                {
                    var tmpLst = new List<PressureByKmPoint>();
                    
                    for (var i = 0; i < PressureByKmList.Count - 2; i++)
                    {
                        tmpLst.Add(PressureByKmList[i]);
                        var l = PressureByKmList[i + 1].Km - PressureByKmList[i].Km;
                        var pn = PressureByKmList[i].Pressure;
                        var pk = PressureByKmList[i+1].Pressure;
                        if (l < 2) continue;
                        for (var j = 1; j < 10; j++)
                        {
                            var km = PressureByKmList[i].Km + j*l/10;
                            var p = Math.Sqrt(pn*pn - (pn*pn - pk*pk)*(j*l/10)/l);
                            tmpLst.Add(new PressureByKmPoint(km, p));
                        }
                    }

                    tmpLst.Add(PressureByKmList.Last());
                    PressureByKmList = tmpLst;
                }

                OnPropertyChanged(() => PressureByKmList);

            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }


        public List<PressureByKmPoint> PressureByKmList { get; set; }
    }


    public class PressureByKmPoint
    {
        public PressureByKmPoint(double km, double pressure)
        {
            Km = km;
            Pressure = pressure;
        }

        public double Pressure { get; set; }
        public double Km { get; set; }
        
    }
}