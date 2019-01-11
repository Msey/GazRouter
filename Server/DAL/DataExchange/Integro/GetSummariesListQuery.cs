using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.Integro;
using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DAL.DataExchange.Integro
{
    public class GetSummariesListQuery : QueryReader<List<SummaryDTO>>
    {
        public GetSummariesListQuery(ExecutionContext context) : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"SELECT * FROM integro.v_summaries";
        }

        protected override List<SummaryDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<SummaryDTO>();
            while (reader.Read())
            {
                result.Add(new SummaryDTO
                {
                    Id = reader.GetValue<Guid>("summary_id"),
                    Name = reader.GetValue<string>("name"),
                    Descriptor = reader.GetValue<string>("descriptor"),
                    TransformFileName = reader.GetValue<string>("transform_file_name"),
                    PeriodType = reader.GetValue<PeriodType>("period_type_id"),
                });
            }
            return result;
            //var m = new SummaryManager(context);
        }
    }
}
