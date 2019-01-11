using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DTO.ManualInput.ContractPressures;
using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.ContractPressures
{
    public class GetContractPressureHistoryListQuery : QueryReader<Guid,List<ContractPressureHistoryDTO>>
    {
        public GetContractPressureHistoryListQuery(ExecutionContext context) : base(context)
        {
    }
    protected override string GetCommandText(Guid parameters)
    {
        var sb = new StringBuilder(
@"select cpr.contract_pressure,
       cpr.input_date,
       u.name as user_name,
       e.entity_name as user_site_name
from V_CONTRACT_PRESSURES cpr
INNER JOIN v_users u on u.login = cpr.input_user
INNER JOIN v_entities e on e.entity_id = u.site_id
where cpr.distr_station_outlet_id=:distr_station_outlet_id
order by cpr.input_date desc");

        return sb.ToString();
    }
    protected override void BindParameters(OracleCommand command, Guid parameters)
    {
        command.AddInputParameter("distr_station_outlet_id", parameters);
    }
    protected override List<ContractPressureHistoryDTO> GetResult(OracleDataReader reader, Guid parameters)
    {
        var result = new List<ContractPressureHistoryDTO>();
        while (reader.Read())
        {
                result.Add(new ContractPressureHistoryDTO()
                {
                    Pressure = reader.GetValue<double?>("contract_pressure"),
                    ChangeDate = reader.GetValue<DateTime>("input_date"),
                    UserName = reader.GetValue<string>("user_name"),
                    UserSiteName = reader.GetValue<string>("user_site_name"),
                });
        }
        return result;
    }
}
}
