using System;
using System.Collections.Generic;
using System.Text;
using GazRouter.DAL.Core;
using GazRouter.DTO.Calculations.Log;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Calculations.Log
{
    public class GetCalculationLogsQuery : QueryReader<GetLogListParameterSet, List<LogCalculationDTO>>
    {
        public GetCalculationLogsQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(GetLogListParameterSet parameter)
        {
            var sql = @"    SELECT      c.id, 
                                        c.calculation_id, 
                                        c.series_id, 
                                        c.err_num, 
                                        c.err_mess, 
                                        c.err_date, 
                                        c.key_date, 
                                        c.src_code,
                                        c.calculation_description, 
                                        c.ora_login, 
                                        c.src_code, 
                                        c.period_type_id, 
                                        c.calculation_sys_name
                            FROM        v_errlog_calculations c 
                            WHERE       c.err_date BETWEEN :startDate AND :endDate";

            var sb = new StringBuilder(sql);

            if (parameter.CalculationId.HasValue)
                sb.Append(" AND c.calculation_id = :calcId");

            sb.Append(" ORDER BY err_date DESC");

            return sb.ToString();
        }

        protected override List<LogCalculationDTO> GetResult(OracleDataReader reader, GetLogListParameterSet parameterSet)
        {
            var result = new List<LogCalculationDTO>();
            while (reader.Read())
            {
                result.Add(new LogCalculationDTO
                {
                    Id = reader.GetValue<Guid>("id"),
                    CalculationId = reader.GetValue<int>("calculation_id"),
                    CalculationSysName = reader.GetValue<string>("calculation_sys_name"),
                    CalculationDescription = reader.GetValue<string>("calculation_description"),
                    ErrorNumber = reader.GetValue<string>("err_num"),
                    ErrorDate = reader.GetValue<DateTime>("err_date"),
                    KeyDate = reader.GetValue<DateTime?>("key_date"),
                    PeriodTypeId = reader.GetValue<PeriodType?>("period_type_id"),
                    ErrorMessage = reader.GetValue<string>("err_mess"),
                    SrcCode = reader.GetValue<string>("src_code")
                });
            }
            return result;
        }

        protected override void BindParameters(OracleCommand command, GetLogListParameterSet parameterSet)
        {
            if (parameterSet.CalculationId.HasValue)
            {
                command.AddInputParameter("calcId", parameterSet.CalculationId);
            }
            command.AddInputParameter("startDate", parameterSet.StartDate);
            command.AddInputParameter("endDate", parameterSet.EndDate);
        }
    }
}