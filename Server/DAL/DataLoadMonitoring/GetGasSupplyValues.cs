using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetGasSupplyValues : QueryReader<int, List<GasSupplyValue>>
    {
        public GetGasSupplyValues(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameter)
        {
            return @"
Select
          t.series_id ,
          t.pipeline_id ,
          t.kilometer_start ,
          t.kilometer_end ,
          t.description,
          t.gaz_volume ,
          t.gaz_volume_change
From v_gas_supply  t
Where  t.series_id  = :SerieId"

                ;
        }
        
        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("SerieId", parameters);
        }

        protected override List<GasSupplyValue> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<GasSupplyValue>();
            while (reader.Read())
            {
                //var serie = new SeriesDTO
                //{
                //   KeyDate = reader.GetValue<DateTime>("key_date"),
                //   TargetId = reader.GetValue<Target>("target_id"),
                //   PeriodTypeId = reader.GetValue<PeriodType>("period_type_id"),
                //   Id = reader.GetValue<int>("series_id")
                //};
                var pipelineId = reader.GetValue<Guid>("pipeline_id");
                var kmStart = reader.GetValue<Double>("kilometer_start");
                var kmEnd = reader.GetValue<Double>("kilometer_end");
                var descr = reader.GetValue<string>("description");
                var gazVol = reader.GetValue<Double?>("gaz_volume");
                var gazVolChange = reader.GetValue<Double?>("gaz_volume_change");
                var supplyValue = new GasSupplyValue
                {
                    //Serie = serie,
                    Description = descr,
                    GazVolume = gazVol,
                    GazVolumeChange = gazVolChange,
                    KmEnd = kmEnd,
                    KmStart = kmStart,
                    PipelineId = pipelineId
                };
                result.Add(supplyValue);
            }
            return result;

        }

    }
}
