using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Complexes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Complexes
{
    public class GetComplexByIdQuery : QueryReader<int, ComplexDTO>
    {
        public GetComplexByIdQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(int parameters)
        {
            return @"   SELECT	    complex_id,
                                    complex_type,
                                    start_date,
                                    end_date,
                                    is_local,
                                    system_id
                        FROM 	    V_PPW_COMPLEXES
                        WHERE       complex_id = :cmplxid
                        ORDER BY    start_date ASC";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {

            command.AddInputParameter("cmplxid", parameters);
        }

        protected override ComplexDTO GetResult(OracleDataReader reader, int parameters)
        {
            if (reader.Read())
            {
                return
                    new ComplexDTO
                    {
                        Id = reader.GetValue<int>("complex_id"),
                        ComplexName = reader.GetValue<string>("complex_type"),
                        StartDate = reader.GetValue<DateTime>("start_date"),
                        EndDate = reader.GetValue<DateTime>("end_date"),
                        IsLocal = reader.GetValue<bool>("is_local"),
                        SystemId = reader.GetValue<int>("system_id")
                    };
            }
            return null;
        }
    }
}
