using GazRouter.DAL.Core;
using GazRouter.DTO.Repairs.Agreed;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Repairs.Agreement
{
    public class GetAgreementListQuery : QueryReader<int, List<AgreedRepairRecordDTO>>
    {
        public GetAgreementListQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string GetCommandText(int parameters)
        {
            var sql =
@"SELECT 
    a.agreed_repair_record_id,
    a.repair_id,
    a.creation_date,
    a.agreed_user_id,
    a.agreed_date,
    a.description,
    a.agreed_result,
    u.fio as a_user_fio,
    u.position as a_user_position,
    a.real_agreed_user_id,
    r.fio as r_user_fio,
    r.position as r_user_position

FROM 
    V_AGREED_REPAIR_RECORDS  a 
    LEFT JOIN V_AGREED_USERS      u ON u.agreed_user_id = a.agreed_user_id
    LEFT JOIN V_AGREED_USERS r ON r.agreed_user_id = a.real_agreed_user_id
WHERE a.repair_id = :repairid";


            return sql;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("repairid", parameters);
        }

        protected override List<AgreedRepairRecordDTO> GetResult(OracleDataReader reader, int parameters)
        {
            var result = new List<AgreedRepairRecordDTO>();
            while (reader.Read())
            {
                result.Add(
                    new AgreedRepairRecordDTO
                    {
                        Id = reader.GetValue<int>("agreed_repair_record_id"),
                        RepairID = reader.GetValue<int>("repair_id"),
                        Comment = reader.GetValue<string>("description"),
                        CreationDate = reader.GetValue<DateTime>("creation_date"),
                        AgreedUserId = reader.GetValue<int>("agreed_user_id"),
                        RealAgreedUserId = reader.GetValue<int?>("real_agreed_user_id"),
                        AgreedDate = reader.GetValue<DateTime?>("agreed_date"),
                        AgreedResult = reader.GetValue<bool?>("agreed_result"),
                        AgreedUserName = reader.GetValue<string>("a_user_fio"),
                        AgreedUserPosition = reader.GetValue<string>("a_user_position"),
                        RealAgreedUserName = reader.GetValue<string>("r_user_fio"),
                        RealAgreedUserPosition = reader.GetValue<string>("r_user_position")
                    });
            }
            return result;
        }
    }
}
