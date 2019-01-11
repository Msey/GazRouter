using System;
using System.Collections.Generic;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;


namespace GazRouter.Modes.ProcessMonitoring.ObjectStory.CompShop
{
    public class CompShopStoryViewModel : ViewModelBase
    {
        private readonly Guid _shopId;
        private readonly DateTime _timestamp;

        public CompShopStoryViewModel(Guid shopId, DateTime timestamp)
        {
            _shopId = shopId;
            _timestamp = timestamp;
            Refresh();
        }

        public async void Refresh()
        {
            try
            {
                Behavior.TryLock();

                var data = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = {_shopId},
                        PeriodType = PeriodType.Twohours,
                        StartDate = _timestamp.AddDays(-1),
                        EndDate = _timestamp,
                        LoadMessages = true
                    });


                Items = new List<ShopItem>();

                for (var t = _timestamp; t > _timestamp.AddDays(-1); t = t.AddHours(-2))
                {
                    Items.Add(new ShopItem(_shopId, t, data));
                }

                OnPropertyChanged(() => Items);

            }
            finally 
            {
                Behavior.TryUnlock();
            }
            
        }

        public List<ShopItem> Items { get; set; }
        
    }

    public class ShopItem : StoryItemBase
    {
        

        public ShopItem(Guid shopId, DateTime timestamp, Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> propValues)
        {
            Timestamp = timestamp;

            Pattern = new StringMeasuring(shopId, PropertyType.CompressorShopPattern, PeriodType.Twohours);
            PressureInlet = new DoubleMeasuring(shopId, PropertyType.PressureInlet, PeriodType.Twohours, true);
            PressureOutlet = new DoubleMeasuring(shopId, PropertyType.PressureOutlet, PeriodType.Twohours, true);
            CompressionRatio = new DoubleMeasuring(shopId, PropertyType.CompressionRatio, PeriodType.Twohours);
            TemperatureInlet = new DoubleMeasuring(shopId, PropertyType.TemperatureInlet, PeriodType.Twohours, true);
            TemperatureOutlet = new DoubleMeasuring(shopId, PropertyType.TemperatureOutlet, PeriodType.Twohours, true);
            TemperatureCooling = new DoubleMeasuring(shopId, PropertyType.TemperatureCooling, PeriodType.Twohours, true);
            FuelGasConsumption = new DoubleMeasuring(shopId, PropertyType.FuelGasConsumption, PeriodType.Twohours, true);

            Pattern.Extract(propValues, timestamp);
            PressureInlet.Extract(propValues, timestamp);
            PressureOutlet.Extract(propValues, timestamp);
            CompressionRatio.Extract(propValues, timestamp);
            TemperatureInlet.Extract(propValues, timestamp);
            TemperatureOutlet.Extract(propValues, timestamp);
            TemperatureCooling.Extract(propValues, timestamp);
            FuelGasConsumption.Extract(propValues, timestamp);
        }


        

        public StringMeasuring Pattern { get; set; }
        public DoubleMeasuring PressureInlet { get; set; }
        public DoubleMeasuring PressureOutlet { get; set; }
        public DoubleMeasuring CompressionRatio { get; set; }
        public DoubleMeasuring TemperatureInlet { get; set; }
        public DoubleMeasuring TemperatureOutlet { get; set; }
        public DoubleMeasuring TemperatureCooling { get; set; }
        public DoubleMeasuring FuelGasConsumption { get; set; }


        
    }
}