using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Repairs.Agreement
{
    public class GetUserAgreementCount : QueryReader<int, int>
    {
        public GetUserAgreementCount(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = false;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_user_id", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return
@"SELECT 
   count(a.agreed_repair_record_id) as agr_count
FROM
    V_AGREED_REPAIR_RECORDS   a
    LEFT JOIN V_AGREED_USERS  u ON u.agreed_user_id = a.agreed_user_id
WHERE
    (u.user_id = :p_user_id
    or
    u.user_id in 
    (
        select v2.user_id from
                v_agreed_users v1
                left join v_agreed_users v2 on v1.agreed_user_id_ref = v2.agreed_user_id
        where v1.user_id = :user_id and v2.user_id is not null)
    ) 
    and a.agreed_date is null";
        }

        protected override int GetResult(OracleDataReader reader, int parameters)
        {
            int result = 0;
            
            while (reader.Read())
            {
                result = reader.GetValue<int>("agr_count");
            }
            return result;
        }
    }
}
