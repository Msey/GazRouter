using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Repairs.Complexes
{
    public class GetComplexListQuery : QueryReader<GetRepairPlanParameterSet, List<ComplexDTO>>
    {
        public GetComplexListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetRepairPlanParameterSet parameters)
        {
            return @"   SELECT	    complex_id,
                                    complex_type,
                                    start_date,
                                    end_date,
                                    is_local,
                                    system_id
                        FROM 	    V_PPW_COMPLEXES
                        WHERE       start_date >= :startdate
                        AND         end_date < :enddate
                        AND         system_id = :systemid
                        ORDER BY    start_date ASC";
        }

        protected override void BindParameters(OracleCommand command, GetRepairPlanParameterSet parameters)
        {
            command.AddInputParameter("startdate", new DateTime(parameters.Year, 1, 1, 0, 0, 0, DateTimeKind.Local));
            command.AddInputParameter("enddate", new DateTime(parameters.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Local));
            command.AddInputParameter("systemid", parameters.SystemId);
        }

        protected override List<ComplexDTO> GetResult(OracleDataReader reader, GetRepairPlanParameterSet parameters)
        {
            var complexList = new List<ComplexDTO>();
            while (reader.Read())
            {
                complexList.Add(
                    new ComplexDTO
                    {
                        Id = reader.GetValue<int>("complex_id"),
                        ComplexName = reader.GetValue<string>("complex_type"),
                        StartDate = reader.GetValue<DateTime>("start_date"),
                        EndDate = reader.GetValue<DateTime>("end_date"),
                        IsLocal = reader.GetValue<bool>("is_local"),
                        SystemId = reader.GetValue<int>("system_id")
                    });
            }
            return complexList;
        }
    }
}
