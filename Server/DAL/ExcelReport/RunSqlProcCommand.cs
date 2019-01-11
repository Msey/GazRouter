using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.ExcelReports;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ExcelReport
{
    public class RunSqlProcCommand : CommandScalar<string, string>
    {
        public RunSqlProcCommand(ExecutionContext context)
            : base(context, true)
        {
        }
        protected override string GetCommandText(string parameters)
        {
            return $@"select {parameters} from dual";
        }
    }
}