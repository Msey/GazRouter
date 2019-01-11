using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.MeasLine;

namespace GazRouter.Controls.Trends
{
	public static class TrendsHelper
	{
		private static TrendsWindow _trendWindow;

        public static void ShowTrends(CommonEntityDTO entity, PropertyType propertyType, PeriodType periodType = PeriodType.Twohours)
		{
            if (_trendWindow == null)
			{
				_trendWindow = new TrendsWindow
				{
				    DataContext = new TrendsViewModel()
				};
			}
			
            _trendWindow.DataContext.AddTrend(entity, propertyType, periodType);
			if (!_trendWindow.IsOpen)
                _trendWindow.Show();
		}

        public static async void ShowTrends(Guid entityId, PropertyType propertyType, PeriodType periodType = PeriodType.Twohours)
        {
            var entityDto = await new ObjectModelServiceProxy().GetEntityByIdAsync(entityId);
            ShowTrends(entityDto, propertyType, periodType);
        }

	    public static void ShowTrends(List<Property> propList)
	    {
            if (_trendWindow == null)
            {
                _trendWindow = new TrendsWindow
                {
                    DataContext = new TrendsViewModel()
                };
            }

            _trendWindow.DataContext.TrendList.Clear();
            
            propList.ForEach(p => _trendWindow.DataContext.AddTrend(p.Entity, p.PropertyType, p.PeriodType));
            
            if (!_trendWindow.IsOpen)
                _trendWindow.Show();
        }



        #region TrendSets

        // Возвращает список трендов в зависимости от типа объекта
        public static Dictionary<string, Action> GetTrendDict(CommonEntityDTO entity)
        {
            var trendDict = new Dictionary<string, Action>();
            switch (entity.EntityType)
            {
                case EntityType.CompShop:
                    return GetCompShopTrendDict(entity);

                case EntityType.MeasStation:
                    return GetMeasStationTrendDict(entity);

                case EntityType.DistrStation:
                    return GetDistrStationTrendDict(entity);

            }

            return trendDict;
        }


        // Набор трендов для КЦ
        private static Dictionary<string, Action> GetCompShopTrendDict(CommonEntityDTO entity)
	    {
	        return new Dictionary<string, Action>
	        {
	            {
	                "Рвх - Рвых",
                    () => 
                    ShowTrends(new List<Property> 
	                {
                        new Property(entity, PropertyType.PressureInlet, PeriodType.Twohours),
	                    new Property(entity, PropertyType.PressureOutlet, PeriodType.Twohours)
	                })
	            },

	            {
	                "Твх - Твых - Таво",
                    () =>
                    ShowTrends(new List<Property>
                    {
	                    new Property(entity, PropertyType.TemperatureInlet, PeriodType.Twohours),
	                    new Property(entity, PropertyType.TemperatureOutlet, PeriodType.Twohours),
                        new Property(entity, PropertyType.TemperatureCooling, PeriodType.Twohours)
                    })
	            }
	        };
	    }


        // Набор трендов для ГИС
        private static Dictionary<string, Action> GetMeasStationTrendDict(CommonEntityDTO entity)
        {
            return new Dictionary<string, Action>
            {
                {
                    "Q часовой",
                    () =>
                    ShowTrends(new List<Property>
                    {
                        new Property(entity, PropertyType.Flow, PeriodType.Twohours),
                    })
                },

                {
                    "Q часовой (в т.ч. по замерным линиям)",
                    async () =>
                    {
                        var lines = await new ObjectModelServiceProxy().GetMeasLineListAsync(
                            new GetMeasLineListParameterSet
                            {
                                MeasStationId = entity.Id
                            });
                        var propList = new List<Property> {new Property(entity, PropertyType.Flow, PeriodType.Twohours)};
                        propList.AddRange(lines.Select(o => new Property(o, PropertyType.Flow, PeriodType.Twohours)));
                        ShowTrends(propList);
                    }

                },

                {
                    "Q суточный",
                    () =>
                    ShowTrends(new List<Property>
                    {
                        new Property(entity, PropertyType.Flow, PeriodType.Day),
                    })
                },

                {
                    "Q суточный (в т.ч. по замерным линиям)",
                    async () =>
                    {
                        var lines = await new ObjectModelServiceProxy().GetMeasLineListAsync(
                            new GetMeasLineListParameterSet
                            {
                                MeasStationId = entity.Id
                            });
                        var propList = new List<Property> {new Property(entity, PropertyType.Flow, PeriodType.Day)};
                        propList.AddRange(lines.Select(o => new Property(o, PropertyType.Flow, PeriodType.Day)));
                        ShowTrends(propList);
                    }
                }
            };
        }

        // Набор трендов для ГРС
        private static Dictionary<string, Action> GetDistrStationTrendDict(CommonEntityDTO entity)
        {
            return new Dictionary<string, Action>
            {
                {
                    "Р", 
                    () =>
                    ShowTrends(new List<Property>
                    {
                        new Property(entity, PropertyType.PressureInlet, PeriodType.Twohours),
                        new Property(entity, PropertyType.PressureInlet, PeriodType.Day)

                    })
                },

                {
                    "Т",
                    () =>
                    ShowTrends(new List<Property>
                    {
                        new Property(entity, PropertyType.TemperatureInlet, PeriodType.Twohours),
                        new Property(entity, PropertyType.TemperatureInlet, PeriodType.Day)

                    })
                },

                {
                    "Q часовой",
                    () =>
                    ShowTrends(new List<Property>
                    {
                        new Property(entity, PropertyType.Flow, PeriodType.Twohours),
                    })
                },

                {
                    "Q часовой (в т.ч. по выходам)",
                    async () =>
                    {
                        var outlets = await new ObjectModelServiceProxy().GetDistrStationOutletListAsync(
                            new GetDistrStationOutletListParameterSet
                            {
                                DistrStationId = entity.Id
                            });
                        var propList = new List<Property> {new Property(entity, PropertyType.Flow, PeriodType.Twohours)};
                        propList.AddRange(outlets.Select(o => new Property(o, PropertyType.Flow, PeriodType.Twohours)));
                        ShowTrends(propList);
                    }
                },

                {
                    "Q суточный",
                    () =>
                    ShowTrends(new List<Property>
                    {
                        new Property(entity, PropertyType.Flow, PeriodType.Day),
                    })
                },

                {
                    "Q сутки (в т.ч. по выходам)",
                    async () =>
                    {
                        var outlets = await new ObjectModelServiceProxy().GetDistrStationOutletListAsync(
                            new GetDistrStationOutletListParameterSet
                            {
                                DistrStationId = entity.Id
                            });
                        var propList = new List<Property> {new Property(entity, PropertyType.Flow, PeriodType.Day)};
                        propList.AddRange(outlets.Select(o => new Property(o, PropertyType.Flow, PeriodType.Day)));
                        ShowTrends(propList);
                    }
                }
            };
        }

        #endregion


    }


    public class Property
    {
        public Property(CommonEntityDTO entity, PropertyType propType, PeriodType period)
        {
            Entity = entity;
            PropertyType = propType;
            PeriodType = period;
        }
        public CommonEntityDTO Entity { get; set; }

        public PropertyType PropertyType { get; set; }

        public PeriodType PeriodType { get; set; }
    }

    
}
