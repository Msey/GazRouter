using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.PropertyTypes;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Export
{
    public abstract class ExchangeObjectExporterBase
    {
        protected ExecutionContext _context;

        protected Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> _data;
        protected DateTime _endDate;
        protected List<PropertyTypeDTO> _propertyTypes;
        protected PeriodType? _periodTypeId;
        protected SeriesDTO _serie;
        protected DateTime _startDate;
        protected ExchangeTaskDTO _task;



        protected ExchangeObjectExporterBase(ExecutionContext context)
        {
            _context = context;
        }

        protected ExchangeObject<T> Build<T>(SeriesDTO serie) where T : new()
        {
            _serie = serie;
            if (_serie != null)
            {
                _periodTypeId = serie.PeriodTypeId;
                _startDate = _serie.KeyDate;
                Debug.Assert(_periodTypeId != null, "_periodTypeId != null");
                _endDate = _startDate.Add(((PeriodType)_periodTypeId).ToTimeSpan());
            }

            Debug.Assert(_periodTypeId != null, "_periodTypeId != null");
            var parameters = new GetEntityPropertyValueListParameterSet()
                             {
                                 PeriodType = (PeriodType) _periodTypeId,
                                 SeriesId = _serie?.Id
                             };
            _data = new GetEntityPropertyValueListQuery(_context).Execute(parameters);


            var result = new ExchangeObject<T>
                         {
                             HeaderSection = GetExchangeHeader()
                         };

            _propertyTypes = new GetPropertyTypeListQuery(_context).Execute();
            return result;
        }

        protected ExchangeHeader GetExchangeHeader()
        {
            var timeStamp = _serie?.KeyDate ?? _startDate;
            var periodType = _periodTypeId ?? PeriodType.None;
            return new ExchangeHeader
            {
                PeriodType = periodType,
                GeneratedTime = DateTime.Now,
                TimeStamp = periodType == PeriodType.Day ? timeStamp.ToUnspecified() : timeStamp,
            };
        }


        protected virtual ExchangeItem<TDto> GetExchangeItem<TDto>(TDto dto) where TDto : CommonEntityDTO, new()
        {
            return new ExchangeItem<TDto>
                   {
                       Dto = dto,
                       Properties = GetProperties(dto.Id)
                   };
        }
        protected abstract List<ExchangeItem<TDto>> GetExchangeItems<TDto>(QueryReader<Guid, TDto> query,
            EntityType currentEntityType) where TDto : CommonEntityDTO, new();


        protected virtual List<ExchangeProperty> GetProperties(Guid entityId)
        {
            var propertyList = new List<ExchangeProperty>();
            if (!_data.ContainsKey(entityId))
                return propertyList;
            foreach (var prop in _data[entityId])
            {
                var propertyTypeDTO = _propertyTypes.Single(et => et.PropertyType == prop.Key);

                dynamic value;
                if (prop.Value.Any())
                {
                    var v = prop.Value.First();

                    value = ExchangeHelper.GetValue(v);
                }
                else
                {
                    value = null;
                }


                ExchangeProperty exchangeProperty = GetExchangeProperty(entityId, value, propertyTypeDTO);
                if (exchangeProperty != null)
                {
                    propertyList.Add(exchangeProperty);
                }
            }
            return propertyList;
        }


        protected ExchangeProperty GetExchangeProperty(Guid entityId, dynamic value, PropertyTypeDTO propertyTypeDTO, string extKey = null)
        {
            return new ExchangeProperty
                   {
                       SysName = propertyTypeDTO.SysName,
                       Value = value,
                       PropertyType = propertyTypeDTO.PropertyType,
                       ExtKey = extKey
                   };
        }
    }
}