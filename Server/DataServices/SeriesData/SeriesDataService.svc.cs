using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DAL.SeriesData.CompUnits;
using GazRouter.DAL.SeriesData.GasInPipes;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DAL.SeriesData.ValueMessages;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DataServices.ObjectModel;
using GazRouter.DAL.SeriesData.SerieChecks;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData;
using GazRouter.DTO.SeriesData.CompUnitPropertyValues;
using GazRouter.DTO.SeriesData.GasInPipes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.SerieChecks;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.Trends;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.DataServices.SeriesData
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class SeriesDataService : ServiceBase, ISeriesDataService
    {

        /* ДОРОГИЕ ДРУЗЬЯ! 
         * ПРЕЖДЕ ЧЕМ ДОБАВЛЯТЬ СЮДА КАКУЮ-НИБУДЬ "ОЧЕНЬ НУЖНУЮ"
         * И "ОЧЕНЬ ОСОБЕННУЮ" ФУНКЦИЮ, ПОЖАЛУЙСТА, УБЕДИТЕСЬ,
         * ЧТО ТО ЖЕ САМОЕ НЕЛЬЗЯ ПОЛУЧИТЬ С ПОМОЩЬЮ УЖЕ ИМЕЮЩИХСЯ.
         * СПАСИБО!
         */


        #region SERIES

        // Получить список серий, удовлетворящих заданным критериям
        public List<SeriesDTO> GetSeriesList(GetSeriesListParameterSet parameters)
        {
            return ExecuteRead<GetSeriesListQuery, List<SeriesDTO>, GetSeriesListParameterSet>(parameters);
        }

        // Найти определенную серию, удовлетворящую заданному критерию
        // Здесь можно передать либо id серии, либо метку времени и период.
        // Если не указывать метку времени, то вернет последнюю серию для заданного периода или вообще
        public SeriesDTO GetSeries(GetSeriesParameterSet parameters)
        {
            return ExecuteRead<GetSeriesQuery, SeriesDTO, GetSeriesParameterSet>(parameters);
        }

        // Добавление серии. 
        // Добавляет серию если такой серии еще нет, если есть, то вернет уже существующую
        public SeriesDTO AddSerie(AddSeriesParameterSet parameters)
        {
            var id = ExecuteRead<AddSeriesCommand, int, AddSeriesParameterSet>(parameters);
            return ExecuteRead<GetSeriesQuery, SeriesDTO, GetSeriesParameterSet>(new GetSeriesParameterSet { Id = id });
        }

        #endregion


        #region Value_Messages

        // Получить список ошибок и тревог для значения свойства (сформированных проверками)
        public Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>> GetPropertyValueMessageList(
            GetPropertyValueMessageListParameterSet parameters)
        {
            return
                ExecuteRead
                    <GetPropertyValueMessageListQuery, Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>>,
                        GetPropertyValueMessageListParameterSet>(parameters);
        }


        // Квитирование сообщения (только для тревог)
        public void AcceptMessage(Guid parameters)
        {
            ExecuteNonQuery<AcceptMessageCommand, Guid>(parameters);
        }



        // Запуск проверки значений параметров по заданному объекту за заданную метку времени
        public void PerformChecking(List<PerformCheckingParameterSet> parameters)
        {
            using (var context = OpenDbContext())
            {
                foreach (var set in parameters)
                {
                    new PerformCheckingCommand(context).Execute(set);
                }
            }
        }

        #endregion


        // Возвращает значение свойства
        public BasePropertyValueDTO GetPropertyValue(GetPropertyValueParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var value = new GetPropertyValueQuery(context).Execute(parameters);

                if (parameters.LoadMessages)
                {
                    var msgList = new GetPropertyValueMessageListQuery(context).Execute(
                        new GetPropertyValueMessageListParameterSet
                        {
                            EntityIdList = { parameters.EntityId },
                            PropertyType = parameters.PropertyTypeId,
                            PeriodType = parameters.PeriodTypeId,
                            StartDate = parameters.Timestamp,
                            EndDate = parameters.Timestamp
                        });

                    if (msgList.ContainsKey(parameters.EntityId) &&
                        msgList[parameters.EntityId].ContainsKey(parameters.PropertyTypeId))
                        value.MessageList = msgList[parameters.EntityId][parameters.PropertyTypeId];
                }
                return value;
            }
        }

        public List<BasePropertyValueDTO> GetPropertyValueList(GetPropertyValueListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var valueList = new GetPropertyValueListQuery(context).Execute(parameters);
                if (parameters.LoadMessages)
                {
                    var msgList = new GetPropertyValueMessageListQuery(context).Execute(
                        new GetPropertyValueMessageListParameterSet
                        {
                            EntityIdList = { parameters.EntityId },
                            PropertyType = parameters.PropertyTypeId,
                            PeriodType = parameters.PeriodTypeId,
                            StartDate = parameters.StartDate,
                            EndDate = parameters.EndDate
                        });
                    if (msgList.ContainsKey(parameters.EntityId) &&
                        msgList[parameters.EntityId].ContainsKey(parameters.PropertyTypeId))
                    {
                        foreach (var value in valueList)
                        {
                            value.MessageList.AddRange(
                                msgList[parameters.EntityId][parameters.PropertyTypeId].Where(
                                    m => m.Timestamp == value.Date));
                        }
                    }
                }
                return valueList;
            }
        }

        // Получить тренд
        public TrendDTO GetTrend(GetTrendParameterSet parameters)
        {
            var trend = new TrendDTO();
            using (var context = OpenDbContext())
            {
                trend.Data = new GetPropertyValueListQuery(context).Execute(
                    new GetPropertyValueListParameterSet
                    {
                        EntityId = parameters.EntityId,
                        PropertyTypeId = parameters.PropertyTypeId,
                        PeriodTypeId = parameters.PeriodTypeId,
                        StartDate = parameters.StartDate,
                        EndDate = parameters.EndDate
                    });
                trend.Id = parameters.Id;
            }
            return trend;
        }

        // Возвращает значения всех свойств одного или нескольких объектов за указанный период времени, либо для указанного SerieId
        // Если не указывать объекты, то вернет значения свойств по всем объектам
        public Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> GetEntityPropertyValueList(
            GetEntityPropertyValueListParameterSet parameters)
        {

            using (var context = OpenDbContext())
            {
                var valueList = new GetEntityPropertyValueListQuery(context).Execute(parameters);
                
                if (parameters.LoadMessages)
                {
                    var msgList = new GetPropertyValueMessageListQuery(context).Execute(
                        new GetPropertyValueMessageListParameterSet
                        {
                            EntityIdList = parameters.EntityIdList,
                            SerieId = parameters.SeriesId,
                            PeriodType = parameters.PeriodType,
                            StartDate = parameters.StartDate,
                            EndDate = parameters.EndDate
                        });

                    foreach (var entity in valueList)
                    {
                        if (msgList.ContainsKey(entity.Key))
                        {
                            var entMsgList = msgList[entity.Key];
                            foreach (var prop in entity.Value)
                            {
                                if (entMsgList.ContainsKey(prop.Key))
                                {
                                    var propMsgList = entMsgList[prop.Key];
                                    foreach (var value in prop.Value)
                                    {
                                        value.MessageList.AddRange(propMsgList.Where(m => m.Timestamp == value.Date));
                                    }
                                }
                            }
                        }
                    }
                }
                
                return valueList;
            }
        }


        // Сохранение значений свойств
        // Если значение есть, то перезаписывает его, если нет, то добавляет
        public void SetPropertyValue(List<SetPropertyValueParameterSet> parameters)
        {
            using (var context = OpenDbContext())
            {
                parameters.ForEach(p => new SetPropertyValueCommand(context).Execute(p));
            }
        }



        #region SERIE_CHECK

        public List<SerieCheckDTO> GetSerieCheckList()
        {
            return ExecuteRead<GetSerieCheckListQuery, List<SerieCheckDTO>>();
        }


        public void UpdateSerieCheck(UpdateSerieCheckParameterSet parameters)
        {
            ExecuteNonQuery<UpdateSerieCheckCommand, UpdateSerieCheckParameterSet>(parameters);   
        }

        public void UpdateEntityTypeProperty(UpdateEntityPropertyTypeParameterSet parameters)
        {
            ExecuteNonQuery<UpdateEntityPropertyTypeCommand, UpdateEntityPropertyTypeParameterSet>(parameters);
        }

        #endregion


        




        // Возвращает значения параметров ГПА за указанную метку времения, включая параметры окружающей среды,
        // а также физ-хим. показатели газа
        public Dictionary<PropertyType, PropertyValueDoubleDTO> GetCompUnitPropertyValueList(
            GetCompUnitPropertyValuesParameterSet parameters)
        {
            var result = new Dictionary<PropertyType, PropertyValueDoubleDTO>();

            using (var context = OpenDbContext())
            {
                var compUnit = new GetCompUnitByIdQuery(context).Execute(parameters.CompUnitId);
                if (compUnit == null) return null;

                var entityList = new List<Guid> { compUnit.Id, compUnit.CompStationId };

                var measPoint = new ObjectModelService().FindMeasPoint(compUnit.Id);
                if (measPoint != null)
                    entityList.Add(measPoint.Id);

                var values = new GetEntityPropertyValueListQuery(context).Execute(
                    new GetEntityPropertyValueListParameterSet
                    {
                        EntityIdList = entityList,
                        PeriodType = PeriodType.Twohours,
                        StartDate = parameters.Timestamp,
                        EndDate = parameters.Timestamp
                    });

                foreach (var val in values.SelectMany(ent => ent.Value))
                {
                    result.Add(val.Key,
                        val.Value.FirstOrDefault() as PropertyValueDoubleDTO);
                }

            }

            return result;
        }




        #region НАРАБОТКА ГПА

        public CompUnitsOperatingTimeDto GetOperatingTimeCompUnitList(DateIntervalParameterSet parameters)
        {
            List<CompStationDTO> compStationlist;
            List<CompShopDTO> compShoplist;
            List<CompUnitDTO> compUnitlist;
            List<SiteDTO> sitelist;
            Dictionary<Guid, Dictionary<CompUnitState, List<DateIntervalDTO>>> operatingTime;
            var subres = new Dictionary<SiteDTO, Dictionary<CompStationDTO, Dictionary<CompShopDTO, List<CompUnitDTO>>>>();
            using (var context = OpenDbContext())
            {
                sitelist = new GetSiteListQuery(context).Execute(new GetSiteListParameterSet());
                compStationlist = new GetCompStationListQuery(context).Execute(null);
                compShoplist = new GetCompShopListQuery(context).Execute(null);
                compUnitlist = new GetCompUnitListQuery(context).Execute(null);
                operatingTime = new GetOperatingTimeQuery(context).Execute(parameters);
            }
            foreach (var siteDTO in sitelist)
            {
                var t1 = new Dictionary<CompStationDTO, Dictionary<CompShopDTO, List<CompUnitDTO>>>();
                foreach (var compStationDTO in compStationlist.Where(t => t.ParentId == siteDTO.Id))
                {
                    var temp1 = compShoplist.Where(t => t.ParentId == compStationDTO.Id)
                                            .ToDictionary(compShopDTO => compShopDTO,
                                                          compShopDTO =>
                                                          compUnitlist.Where(p => p.ParentId == compShopDTO.Id).ToList());
                    t1.Add(compStationDTO, temp1);
                }

                subres.Add(siteDTO, t1);

            }
            return new CompUnitsOperatingTimeDto
            {
                EntityTree = subres,
                OperatingTime = operatingTime
            };
        }

        #endregion

        #region ПОЛУЧЕНИЕ ЗАПАСА ГАЗА

        public List<GasInPipeDTO> GetGasInPipeList(GetGasInPipeListParameterSet parameters)
        {
            return ExecuteRead<GetGasInPipeListQuery, List<GasInPipeDTO>, GetGasInPipeListParameterSet>(parameters);
        }
        
        #endregion


        public double GetGasInPipeChange(DateTime parameters)
        {
            var curDay = parameters.Date.AddHours(AppSettingsManager.DispatherDayStartHour - 2);
            var prevDay = curDay.AddDays(-1);
            using (var context = OpenDbContext())
            {
                var curVolume = new GetGasInPipeListQuery(context).Execute(
                    new GetGasInPipeListParameterSet
                    {
                        BeginDate = curDay,
                        EndDate = curDay
                    }).Sum(v => v.Volume);

                var prevVolume = new GetGasInPipeListQuery(context).Execute(
                    new GetGasInPipeListParameterSet
                    {
                        BeginDate = prevDay,
                        EndDate = prevDay
                    }).Sum(v => v.Volume);

                return (curVolume - prevVolume) ?? 0;
            }
        }
    }
}
