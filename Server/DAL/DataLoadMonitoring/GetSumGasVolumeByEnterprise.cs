using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.SeriesData.Series;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetSumGasVolumeByEnterprise : QueryReader<GasSupplySumParameterSet, List<GasSupplySumValueDTO>>
    {
        public GetSumGasVolumeByEnterprise(ExecutionContext context)
        : base(context)
            { }

        protected override string GetCommandText(GasSupplySumParameterSet parameter)
        {
            return string.Format(@"
Select        ss.series_id,
              ss.key_date,
              ss.period_type_id,
              ss.description, 
              v.vol, 
              v.volchange 
From rd.v_value_series ss,
     ( Select  t.series_id  ,sum( t.gaz_volume) vol , sum( t.gaz_volume_change) volchange
      From rd.v_gas_supply  t
      Where  t.series_id in
                                            (  Select    s.series_id   
                                               From      rd.v_value_series s  
                                               Where     s.key_date >=  (  :KeyDate - :CountDays )  
                                                         and s.key_date <= :KeyDate
                                                         and  s.period_type_id = :PeriodTypeId)    
              {0}                                      
      Group by t.series_id) v
Where   v.series_id  = ss.series_id", GetQueryClause(parameter));

                ;
        }

        private static string GetQueryClause(GasSupplySumParameterSet parameter)
        {
            var sb = new StringBuilder(string.Empty);
            if (parameter.PipelineId.HasValue) sb.Append(" and   t.pipeline_id  in (:PipelineId)  ");
            if (parameter.KmBegin.HasValue) sb.Append(" AND t.kilometer_start >= :kmb");
            if (parameter.KmBegin.HasValue) sb.Append(" AND t.kilometer_end <= :kme");

            return sb.ToString();
        }

        protected override void BindParameters(OracleCommand command, GasSupplySumParameterSet parameters)
        {
            command.AddInputParameter("KeyDate", parameters.KeyDate);
            command.AddInputParameter("PeriodTypeId", parameters.PeriodType);
            command.AddInputParameter("CountDays", parameters.CountDays);
            if (parameters.PipelineId.HasValue)
                command.AddInputParameter("PipelineId", parameters.PipelineId.Value);

            if (parameters.KmBegin.HasValue)
                command.AddInputParameter("kmb", parameters.KmBegin);

            if (parameters.KmEnd.HasValue)
                command.AddInputParameter("kme", parameters.KmEnd);

        }

        protected override List<GasSupplySumValueDTO> GetResult(OracleDataReader reader, GasSupplySumParameterSet parameters)
        {
            var result = new List<GasSupplySumValueDTO>();
            while (reader.Read())
            {
                var serie = new SeriesDTO
                {
                    KeyDate = reader.GetValue<DateTime>("key_date"),
                    PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
                    Id = reader.GetValue<int>("series_id")
                };

                var gazVol = reader.GetValue<Double?>("vol");
                var gazVolChange = reader.GetValue<Double?>("volchange");
                var supplyValue = new GasSupplySumValueDTO
                {
                    Serie = serie,
                    GazVolume = gazVol,
                    GazVolumeChange = gazVolChange,
                };
                result.Add(supplyValue);
            }
            return result;

        }
    }
}

