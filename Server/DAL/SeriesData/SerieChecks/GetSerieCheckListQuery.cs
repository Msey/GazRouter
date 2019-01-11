using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.SeriesData.SerieChecks;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.SeriesData.SerieChecks
{
    public class GetSerieCheckListQuery : QueryReader<List<SerieCheckDTO>>
    {
        public GetSerieCheckListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText()
        {
            return @"   SELECT      check_id,
                                    check_name,
                                    description,                                    
                                    status
                        FROM        v_checks
                        WHERE       1=1
                        ORDER BY    sort_order";
        }

        protected override List<SerieCheckDTO> GetResult(OracleDataReader reader)
        {
            var result = new List<SerieCheckDTO>();
            while (reader.Read())
            {
                result.Add(new SerieCheckDTO
                {
                    Id = reader.GetValue<int>("check_id"),
                    Name = reader.GetValue<string>("check_name"),
                    Description = reader.GetValue<string>("description"),
                    IsEnabled = !reader.GetValue<bool>("status")
                });
            }

            return result;
        }
    }


}