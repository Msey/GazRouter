using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetSiteStatisticsByTechDataQuery : QueryReader<DateTime, List<SiteDataLoadStatistics> >
    {
        public GetSiteStatisticsByTechDataQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(DateTime parameter)
        {

            return
@"Select
                t.nm,
                s.site_id,
                t.series_id,
                v.key_date,
                v.period_type_id,                
                t.values_count
From
                (             Select   nm, series_id,  COUNT(*) as  values_count
                               From
                                               (               Select  regexp_substr(lpu.entity_name, 'ЛПУ МГ [^/]+') AS nm, pv.series_id
                                                               From   rd.v_property_values pv
                                                               left Join     rd.v_nm_all lpu  on  lpu.entity_id = pv.entity_id
                                                               Where  regexp_like(lpu.entity_name, '^.*/ЛПУ МГ .*/') and  pv.series_id in 
                                                                              (              Select     series_id 
                                                                                             From       v_value_series
                                                                                             WHERE      :key_date <=  key_date  
                                                                                                        and  key_date <  ( :key_date + 2 )
                                                                              )
                                               )
                               Group  by nm,  series_id
                )   t
Join                    v_value_series v  on t.series_id = v.series_id
Left join  v_nm_sites s        on  s.site_name = t.nm";
        }

        protected override void BindParameters(OracleCommand command, DateTime parameters)
        {
            command.AddInputParameter(":key_date", parameters);
        }

        protected override List<SiteDataLoadStatistics> GetResult(OracleDataReader reader, DateTime parameters)
        {
            var result = new List<SiteDataLoadStatistics>();
            while (reader.Read())
            {
                var stat = new SiteDataLoadStatistics
                    { 
                           Site = new SiteDTO
                           {
                               Name = reader.GetValue<string>("nm"),
                               Id = reader.GetValue<Guid>("site_id"),
                               EntityType = EntityType.Site
                           },
                           DataSeries = new SeriesDTO
                           {
                               Id = reader.GetValue<int>("series_id"),
                               KeyDate = reader.GetValue<DateTime>("key_date"),
                               PeriodTypeId = reader.GetValue<PeriodType>("period_type_id")
                            },
                           ValuesCount = reader.GetValue<int>("values_count")
                    };
                result.Add(stat);
            }
            return result;
        }
    }
}



