using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.PropertyValues;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.PropertyValues
{
    public class GetPropertyValueQuery : QueryReader<GetPropertyValueParameterSet, BasePropertyValueDTO>
	{
        public GetPropertyValueQuery(ExecutionContext context)
			: base(context)
		{
		}

        protected override string GetCommandText(GetPropertyValueParameterSet parameters)
		{
            var q = @"  SELECT      pv1.series_id,
                                    pv1.value_type, 
                                    pv1.value_num, 
                                    pv1.value_str, 
                                    vs1.key_date,
                                    vs1.period_type_id,
                                    pv1.source_type
                        FROM        rd.v_property_values pv1
                        INNER JOIN  v_value_series vs1 ON pv1.series_id = vs1.series_id
                        WHERE       pv1.entity_id = :entityid
                            AND     pv1.property_type_id = :propertyType";

            var sb = new StringBuilder(q);

            // Если задано явное SeriesId игнорим все остальное: Timestamp и PeriodType
            if (parameters.SeriesId.HasValue)
            {
                sb.Append(" AND pv1.series_id = :seriesid");
            }
            else
            {
                sb.Append(" AND vs1.period_type_id = :periodType");
                if (parameters.Timestamp.HasValue)
                {
                    sb.Append(" AND vs1.key_date = :timestamp");
                }
                else
                {
                    // если Timestamp не задан, то просто берем последнюю серию
                    sb.Append(@" AND vs1.key_date = 
                                (
                                    SELECT      MAX(vs.key_date)
                                    FROM        rd.v_property_values pv 
                                    INNER JOIN  v_value_series vs on pv.series_id = vs.series_id
                                    WHERE       pv.entity_id = :entityid
                                        AND     pv.property_type_id = :propertyType
                                        AND     vs.period_type_id = :periodType
                                )");
                }
            }

            return sb.ToString();

		}

        protected override void BindParameters(OracleCommand command, GetPropertyValueParameterSet parameters)
		{
            command.AddInputParameter("entityid", parameters.EntityId);
            command.AddInputParameter("propertyType",parameters.PropertyTypeId);
            
            if (parameters.SeriesId.HasValue)
                command.AddInputParameter("seriesid", parameters.SeriesId);
            else
            {
                command.AddInputParameter("periodType", parameters.PeriodTypeId);

                if (parameters.Timestamp.HasValue)
                    command.AddInputParameter("timestamp", parameters.Timestamp);   
            }
            

            
		}

        protected override BasePropertyValueDTO GetResult(OracleDataReader reader, GetPropertyValueParameterSet parameters)
        {
            if (reader.Read())
            {
                return PropertyValueHelper.GetValue(reader, true);
            }
            return null;
        }
	}
}
