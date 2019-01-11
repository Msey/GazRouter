using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Dictionaries.SuperchargerTypes
{
    public class GetSuperchargerTypePointsQuery : QueryReader<List<SuperchargerChartPointDTO>>
    {
        public GetSuperchargerTypePointsQuery(ExecutionContext context)
            : base(context)
        {}

        protected override string GetCommandText()
        {
            return
                @"SELECT   t1.supercharger_type_id,
                           t1.type_line,
                           t1.x,
                           t1.y
                  FROM     v_supercharger_type_points t1";
        }

        protected override List<SuperchargerChartPointDTO> GetResult(OracleDataReader reader)
        {
            var points = new List<SuperchargerChartPointDTO>();
            while (reader.Read())
            {
                var point =
                    new SuperchargerChartPointDTO
                    {
                        ParentId = reader.GetValue<int>("supercharger_type_id"),
                        LineType = reader.GetValue<int>("type_line"),
                        X = reader.GetValue<double>("x"),
                        Y = reader.GetValue<double>("y")
                    };
                points.Add(point);
            }
            return points;
        }
    }
}