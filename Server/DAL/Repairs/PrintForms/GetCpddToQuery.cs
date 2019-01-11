using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.PrintForms;
using GazRouter.DTO.Repairs.Workflow;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Repairs.PrintForms
{
    public class GetCpddToQuery : QueryReader<int, List<TargetingUserDTO>>
    {
        public GetCpddToQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(int parameters)
        {
            return @"
select 
    a.agreed_lists_cpdd_id, a.entity_type_id, a.cpdd_department, a.cpdd_fio, a.fax, a.sortorder
from 
    v_AGREED_LISTS_CPDD a
where 
    a.entity_type_id=:entitytype 
order by sortorder";
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("entitytype", parameters);
        }

        protected override List<TargetingUserDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<TargetingUserDTO>();
            while (reader.Read())
            {
                var user = new TargetingUserDTO()
                {
                    FIO = reader.GetValue<string>("cpdd_fio"),
                    SortOrder = reader.GetValue<int>("sortorder"),
                    Position = reader.GetValue<string>("cpdd_department"),
                    Fax = reader.GetValue<string>("fax")
                };

                result.Add(user);
            }
            return result;
        }
    }
}
