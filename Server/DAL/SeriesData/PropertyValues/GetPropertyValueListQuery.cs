using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.PropertyValues;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.PropertyValues
{
    public class GetPropertyValueListQuery : QueryReader<GetPropertyValueListParameterSet, List<BasePropertyValueDTO>>
    {
        public GetPropertyValueListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, GetPropertyValueListParameterSet parameters)
        {
            command.AddInputParameter("entityid", parameters.EntityId);
            command.AddInputParameter("propertytypeid", parameters.PropertyTypeId);
            command.AddInputParameter("periodtypeid", parameters.PeriodTypeId);
            command.AddInputParameter("startdate", parameters.StartDate);
            command.AddInputParameter("enddate", parameters.EndDate);
        }

        protected override string GetCommandText(GetPropertyValueListParameterSet parameters)
        {
            return @"   SELECT      pv.series_id,
                                    pv.value, 
                                    pv.value_str, 
                                    pv.value_num, 
                                    pv.value_dat, 
                                    pv.value_type,
                                    pv.period_type_id, 
                                    pv.source_type,
                                    pv.key_date
                        FROM        rd.v_property_values_ext pv
                        WHERE       pv.entity_id = :entityid
                            AND     pv.property_type_id = :propertytypeid
                            AND     pv.period_type_id = :periodtypeid
                            AND     pv.key_date BETWEEN :startdate AND :enddate                            
                        ORDER BY    key_date";
        }

        protected override List<BasePropertyValueDTO> GetResult(OracleDataReader reader, GetPropertyValueListParameterSet parameters)
        {
           var result = new List<BasePropertyValueDTO>();
            while (reader.Read())
            {
                result.Add(PropertyValueHelper.GetValue(reader, true));
            }

            return result;
        }
    }


}