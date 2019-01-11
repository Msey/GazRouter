using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.AggregatorTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.DayBalance.EditAggregators
{
    public class EditAggregatorsViewModel : DialogViewModel
    {

        private int _systemId;
        private DateTime _day;
        private SeriesDTO _serie;
        private AggregatorType _aggrType;

        public EditAggregatorsViewModel(int systemId, DateTime day, AggregatorType aggrType, Action actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _systemId = systemId;
            _day = day;
            _aggrType = aggrType;
            
            SaveCommand = new DelegateCommand(Save);

            Load();
        }
        

        public List<AggrItem> AggrList { get; set; } 

        private async void Load()
        {
            Lock();
            var aggrList = await new ObjectModelServiceProxy().GetAggregatorListAsync(
                new GetAggregatorListParameterSet
                {
                    AggregatorType = _aggrType,
                    SystemId = _systemId
                });

            _serie = await new SeriesDataServiceProxy().AddSerieAsync(
                new AddSeriesParameterSet
                {
                    Year = _day.Year,
                    Month = _day.Month,
                    Day = _day.Day,
                    PeriodTypeId = PeriodType.Day
                });

            if (_serie == null) return;

            var values = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    EntityIdList = aggrList.Select(a => a.Id).ToList(),
                    SeriesId = _serie.Id,
                    PropertyList = { PropertyType.Flow }
                });

            AggrList = new List<AggrItem>();

            foreach (var aggr in aggrList)
            {
                var val = 0.0;
                if (values.ContainsKey(aggr.Id) && values[aggr.Id].ContainsKey(PropertyType.Flow))
                    val = values[aggr.Id][PropertyType.Flow].OfType<PropertyValueDoubleDTO>().FirstOrDefault()?.Value ?? 0.0;
                
                AggrList.Add(new AggrItem(aggr, val));
            }
            
            OnPropertyChanged(() => AggrList);

            Unlock();
        }

        public DelegateCommand SaveCommand { get; set; }
        
        private async void Save()
        {
            Lock();
            
            if (AggrList == null || _serie == null) return;

            var valueList = AggrList.Select(a =>
                new SetPropertyValueParameterSet
                {
                    EntityId = a.AggrDto.Id,
                    PropertyTypeId = PropertyType.Flow,
                    SeriesId = _serie.Id,
                    Value = a.Value
                }).ToList();
            
            await new SeriesDataServiceProxy().SetPropertyValueAsync(valueList);

            DialogResult = true;
        }

        public string Header => _aggrType == AggregatorType.GasSupply ? "Ввод запаса газа" : "Ввод балансовых потерь";
    }

    public class AggrItem
    {
        public AggrItem(AggregatorDTO dto, double val)
        {
            AggrDto = dto;
            Value = val;
        }

        public AggregatorDTO AggrDto { get; set; }
        public double Value { get; set; }
    }

    
}
