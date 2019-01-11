using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataLoadMonitoring
{
    public class GetLastSeries : QueryReader<PeriodType, LastSeriesDTO>
    {
        public GetLastSeries(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(PeriodType parameters)
        {

            return @"   Select     
                            max(s.key_date )                            LASTKEYDATE  
                            ,( max(s.key_date )  -  interval '2' HOUR)  PREVIOUSKEYDATE  
                        From      rd.v_value_series s
                        Where   s.period_type_id = :periodTypeId"

                ;
        }

        protected override void BindParameters(OracleCommand command, PeriodType parameters)
        {
            command.AddInputParameter("periodTypeId", parameters);
        }

        protected override LastSeriesDTO GetResult(OracleDataReader reader, PeriodType parameters)
        {
            var result = new LastSeriesDTO();
            if (!reader.Read()) return result;
            result.LastSerieKeyDate = reader.GetValue<DateTime>("LASTKEYDATE");
            result.PreviousSerieKeyDate = reader.GetValue<DateTime>("PREVIOUSKEYDATE");
            return result;
        }



       
    }
}
