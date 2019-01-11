using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.ManualInput.CompUnitTests;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests
{
    public class GetCompUnitTestListQuery : QueryReader<GetCompUnitTestListParameterSet, List<CompUnitTestDTO>>
    {
        public GetCompUnitTestListQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText(GetCompUnitTestListParameterSet parameter)
        {
            var q =
                @"SELECT    t.comp_unit_test_id,
                            t.comp_unit_id,
                            t.comp_unit_test_date,
                            t.description,
                            t.q_min,
                            t.q_max,
                            t.density,
                            t.temperature,
                            t.pressurein
                FROM v_comp_units_tests t
                LEFT JOIN   v_entity_2_site s ON s.entity_id = t.comp_unit_id
                WHERE 1=1";


            var sb = new StringBuilder(q);

            if (parameter != null)
            {
                if (parameter.CompUnitId.HasValue) sb.Append(" AND t.comp_unit_id = : unitid");
                if (parameter.SiteId.HasValue) sb.Append(" AND s.site_id = : siteid");
            }

            sb.Append(" ORDER BY t.comp_unit_test_date DESC");
            return sb.ToString();
        }


        protected override void BindParameters(OracleCommand command, GetCompUnitTestListParameterSet parameters)
        {
            if (parameters == null) return;

            if (parameters.CompUnitId.HasValue)
                command.AddInputParameter("unitid", parameters.CompUnitId);
            if (parameters.SiteId.HasValue)
                command.AddInputParameter("siteid", parameters.SiteId);
        }

        protected override List<CompUnitTestDTO> GetResult(OracleDataReader reader, GetCompUnitTestListParameterSet parameter)
        {
            var tests = new List<CompUnitTestDTO>();
            while (reader.Read())
            {
                var test =
                    new CompUnitTestDTO
                    {
                        Id = reader.GetValue<int>("comp_unit_test_id"),
                        CompUnitId = reader.GetValue<Guid>("comp_unit_id"),
                        CompUnitTestDate = reader.GetValue<DateTime>("comp_unit_test_date"),
                        Density = reader.GetValue<double?>("density"),
                        TemperatureIn = reader.GetValue<double?>("temperature"),
                        PressureIn = reader.GetValue<double?>("pressurein"),
                        QMax = reader.GetValue<double?>("q_max"),
                        QMin = reader.GetValue<double?>("q_min"),
                        Description = reader.GetValue<string>("description")
                    };
                tests.Add(test);
            }
            return tests;
        }
    }
}