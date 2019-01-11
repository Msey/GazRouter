﻿using System.Collections.Generic;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.ExcelReport
{
    public class GetExcelReportSharedListQuery : QueryReader<int, List<int>>
    {
        public GetExcelReportSharedListQuery(ExecutionContext context) : base(context){ }
        protected override string GetCommandText(int parameters)
        {
            return @"SELECT 
                     DISTINCT    t.dashboard_id 
                     FROM        V_DASHBOARD_REPORTS t 
                     LEFT JOIN   v_dashboards_grants g 
                            ON   g.dashboard_id = t.dashboard_id
                     WHERE       t.user_id =  :id
                           AND   g.user_id != :id";
        }
        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("id", parameters);
        }
        protected override List<int> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<int>();
            while (reader.Read())
            {
                result.Add(reader.GetValue<int>("dashboard_id"));
            }
            return result;
        }
    }
}