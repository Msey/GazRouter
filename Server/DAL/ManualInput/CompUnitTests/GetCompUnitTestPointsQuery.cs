using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests
{
    public class GetCompUnitTestPointsQuery : QueryReader<List<SuperchargerChartPointDTO>>
    {
        public GetCompUnitTestPointsQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return
                @"SELECT   t1.comp_unit_test_id, 
                           t1.type_line, 
                           t1.x, 
                           t1.y
                  FROM     v_comp_units_tests_points t1";
        }

        protected override List<SuperchargerChartPointDTO> GetResult(OracleDataReader reader)
        {
            var points = new List<SuperchargerChartPointDTO>();
            while (reader.Read())
            {
                var point =
                    new SuperchargerChartPointDTO
                    {
                        ParentId = reader.GetValue<int>("COMP_UNIT_TEST_ID"),
                        LineType = reader.GetValue<int>("TYPE_LINE"),
                        X = reader.GetValue<double>("X"),
                        Y = reader.GetValue<double>("Y")
                    };
                points.Add(point);
            }
            return points;
        }
    }
}