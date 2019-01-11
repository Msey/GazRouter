using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Dictionaries.ValueTypes;
using GazRouter.DTO.SeriesData.Series;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetEntityPropertyValues : QueryReader<EntityPropertyValueParameterSet, List<BaseEntityProperty>>
    {
        public GetEntityPropertyValues(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(EntityPropertyValueParameterSet parameter)
        {
            return @"Select        v.series_id
             ,v.entity_id
             ,n.entity_name
             ,et.short_name  entity_type
             ,v.property_type_id
             ,pt.name  property_type             
             ,v.value
             ,v.annotation
             ,v.value_type
             ,v.value_str
             ,v.value_dat
             ,v.value_num
             ,ph.unit_name
             ,s.site_id
             ,s.site_name
             ,v.key_date
             ,v.period_type_id             
From   rd.v_property_values_ext v
left Join   rd.v_nm_short_all n  on n.entity_id = v.entity_id
 Join   rd.v_entities e  on e.entity_id = v.entity_id
Join   rd.v_entity_types et on et.entity_type_id = e.entity_type_id
Join   rd.v_property_types pt on pt.property_type_id = v.property_type_id
Join   rd.v_phisical_types  ph on ph.phisical_type_id = pt.phisical_type_id
join   rd.v_sites  s  on s.site_id =  rd.p_entity.getsiteid(v.entity_id)   
Where       1=1
            and v.source_type is null
            and v.series_id  =:SeriesId
            and s.site_id  =:SiteId"

                ;
        }

        protected override void BindParameters(OracleCommand command, EntityPropertyValueParameterSet parameters)
        {
            command.AddInputParameter("SeriesId", parameters.DataSeries.Id);
            command.AddInputParameter("SiteId", parameters.Site.Id);
        }

        protected override List<BaseEntityProperty> GetResult(OracleDataReader reader, EntityPropertyValueParameterSet parameters)
        {
            var result = new List<BaseEntityProperty>();
            while (reader.Read())
            {
                var type = EntityPropertyValueHelper.GetValueType(reader.GetValue<string>("value_type"));
                var siteId = reader.GetValue<Guid>("site_id");
                var siteName = reader.GetValue<string>("site_name");
                var serie = new SeriesDTO
                {
                    Id = reader.GetValue<int>("series_id"),
                    KeyDate = reader.GetValue<DateTime>("key_date"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id")
                };
                var entityId = reader.GetValue<Guid>("entity_id");
                var entityShortName = reader.GetValue<string>("entity_name");
                var entityTypeName =   reader.GetValue<string>("entity_type");
                var annotation = reader.GetValue<string>("annotation");
                
                var property = reader.GetValue<PropertyType>("property_type_id");
                //в случае, если отсутствует тип 
                if (!type.HasValue)
                {
                    var someValue = reader.GetValue<string>("value");
                    var entityPropertyValue = new EntityPropertyValueStringDTO
                    {
                        Annotation = annotation,
                        DataSeries = serie,
                        EntityId = entityId,
                        EntityShortName = entityShortName,
                        EntityTypeName = entityTypeName,
                        Property = property,
                        Value = someValue,
                        SiteId = siteId,
                        SiteName = siteName
                    };
                    result.Add(entityPropertyValue);
                }
                else
                {
                    switch (type.Value)
                    {
                        case ValueTypesEnum.STRING:
                            var valStr = reader.GetValue<string>("value_str");
                            var entityPropertyValueStr = new EntityPropertyValueStringDTO
                            {
                                    Annotation = annotation,
                                    DataSeries = serie,
                                    EntityId = entityId,
                                    EntityShortName = entityShortName,
                                    EntityTypeName = entityTypeName,
                                    Property = property,
                                    Value = valStr,
                                    SiteId = siteId,
                                    SiteName = siteName
                            };
                            result.Add(entityPropertyValueStr);
                            break;
                        case ValueTypesEnum.DOUBLE:
                        case ValueTypesEnum.INTEGER:
                            var valNumber = reader.GetValue<double?>("value_num");
                            var entityPropertyValueNum = new EntityPropertyValueDoubleDTO
                            {
                                    Annotation = annotation,
                                    DataSeries = serie,
                                    EntityId = entityId,
                                    EntityShortName = entityShortName,
                                    EntityTypeName = entityTypeName,
                                    Property = property,
                                    Value = valNumber,
                                    SiteId = siteId,
                                    SiteName = siteName
                            };
                            result.Add(entityPropertyValueNum);
                            break;

                        case ValueTypesEnum.DATE:
                            var valDate = reader.GetValue<DateTime?>("value_dat");
                            var entityPropertyValueDate = new EntityPropertyValueDateDTO
                            {
                                    Annotation = annotation,
                                    DataSeries = serie,
                                    EntityId = entityId,
                                    EntityShortName = entityShortName,
                                    EntityTypeName = entityTypeName,
                                    Property = property,
                                    Value = valDate,
                                    SiteId = siteId,
                                    SiteName = siteName
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
