using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.ValueTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    /// <summary>
    /// возвращает  параметры за определенную метку времени и период
    /// </summary>
    public class GetSerieValueByKeyDate : QueryReader<SerieValueParameterSet, List<BaseSerieValue>>
    {
        public GetSerieValueByKeyDate(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(SerieValueParameterSet parameter)
        {
            return @"Select    v.series_id
             ,v.entity_id       
             ,v.property_type_id
             ,v.annotation
             ,v.value_type
             ,v.value_str
             ,v.value_dat
             ,v.value_num
             ,v.value           
From   rd.v_property_values_ext v
Where series_id in (   Select s.series_id   
                                      From    rd.v_value_series s  
                                      Where  s.period_type_id = :PeriodTypeId   
and   s.key_date = :KeyDate  ) "

                ;
        }

        protected override void BindParameters(OracleCommand command, SerieValueParameterSet parameter)
        {
            command.AddInputParameter("KeyDate", parameter.KeyDate);
            command.AddInputParameter("PeriodTypeId", parameter.PeriodTypeId);
        }

        protected override List<BaseSerieValue> GetResult(OracleDataReader reader, SerieValueParameterSet parameter)
        {
            var result = new List<BaseSerieValue>();
            while (reader.Read())
            {
                var type = EntityPropertyValueHelper.GetValueType(reader.GetValue<string>("value_type"));
                var seriesId = reader.GetValue<int>("series_id");
                var entityId = reader.GetValue<Guid>("entity_id");
                var annotation = reader.GetValue<string>("annotation");
                var propertyTypeId = reader.GetValue<PropertyType>("property_type_id");


                //в случае, если отсутствует тип 
                if (!type.HasValue)
                {
                    var someValue = reader.GetValue<string>("value");
                    var entityPropertyValue = new SerieValueStringDTO
                    {
                        SerieId = seriesId,
                        Annotation = annotation,
                        EntityId = entityId,
                        Property = propertyTypeId,
                        Value = someValue
                    };
                    result.Add(entityPropertyValue);
                }
                else
                {
                    switch (type.Value)
                    {
                        case ValueTypesEnum.STRING:
                            var valStr = reader.GetValue<string>("value_str");
                            var entityPropertyValueStr = new SerieValueStringDTO
                            {
                                    SerieId = seriesId,
                                    Property = propertyTypeId,
                                    Annotation = annotation,
                                    EntityId = entityId,
                                    Value = valStr
                            };
                            result.Add(entityPropertyValueStr);
                            break;
                        case ValueTypesEnum.DOUBLE:
                        case ValueTypesEnum.INTEGER:
                            var valNumber = reader.GetValue<double?>("value_num");
                            var entityPropertyValueNum = new SerieValueDoubleDTO
                            {
                                    SerieId = seriesId,
                                    Property = propertyTypeId,
                                    Annotation = annotation,
                                    EntityId = entityId,
                                    Value = valNumber
                            };
                            result.Add(entityPropertyValueNum);
                            break;

                        case ValueTypesEnum.DATE:
                            var valDate = reader.GetValue<DateTime?>("value_dat");
                            var entityPropertyValueDate = new SerieValueDateDTO
                            {
                                    SerieId = seriesId,
                                    Property = propertyTypeId,
                                    Annotation = annotation,
                                    EntityId = entityId,
                                    Value = valDate
                            };
                            result.Add(entityPropertyValueDate);
                            break;
                        default:
                            throw new Exception("Тип не поддерживается");
                    }
                }


            }
            return result;
        }



    }
   
}
