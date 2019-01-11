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
    public class GetContractPressureListQuery : QueryReader<GetContractPressureListQueryParameterSet,List<ContractPressureDTO>>
    {
        public GetContractPressureListQuery(ExecutionContext context) : base(context)
        {
    }
    protected override string GetCommandText(GetContractPressureListQueryParameterSet parameters)
    {
        var sb = new StringBuilder(
@"select ds.site_id,
       ds.distr_station_id,
       ds.distr_station_name,
       dso.distr_station_outlet_id,
       dso.distr_station_outlet_name,
       cpr.contract_pressure,
       cpr.input_date
from v_distr_station_outlets dso
INNER JOIN v_distr_stations ds on ds.distr_station_id = dso.distr_station_id
LEFT JOIN V_CONTRACT_PRESSURES cpr on cpr.distr_station_outlet_id = dso.distr_station_outlet_id");

            if (parameters.TargetMonth.HasValue)
                sb.Append(" AND cpr.input_date <= :target_month");

            sb.Append(@"
where (cpr.input_date is null or cpr.input_date = (
select max(cpr2.input_date) from V_CONTRACT_PRESSURES cpr2 where cpr2.distr_station_outlet_id = cpr.distr_station_outlet_id))");

        if (parameters.SiteId.HasValue)
            sb.Append(" AND ds.site_id = :site_id");

        sb.Append(" order by ds.sort_order,dso.sort_order, dso.distr_station_outlet_name");

        return sb.ToString();
    }
    protected override void BindParameters(OracleCommand command, GetContractPressureListQueryParameterSet parameters)
    {
            if (parameters.TargetMonth.HasValue)
                command.AddInputParameter("target_month", parameters.TargetMonth.Value);

            if (parameters.SiteId.HasValue)
            command.AddInputParameter("site_id", parameters.SiteId.Value);
    }
    protected override List<ContractPressureDTO> GetResult(OracleDataReader reader, GetContractPressureListQueryParameterSet parameters)
    {
        var result = new List<ContractPressureDTO>();
        while (reader.Read())
        {
                result.Add(new ContractPressureDTO()
                {
                    SiteId = reader.GetValue<Guid>("site_id"),
                    DistrStationId = reader.GetValue<Guid>("distr_station_id"),
                    DistrStationName = reader.GetValue<string>("distr_station_name"),
                    DistrStationOutletId = reader.GetValue<Guid>("distr_station_outlet_id"),
                    DistrStationOutletName = reader.GetValue<string>("distr_station_outlet_name"),
                    Pressure = reader.GetValue<double?>("contract_pressure"),
                    ChangeDate = reader.GetValue<DateTime?>("input_date"),
                    //UserName = reader.GetValue<string>("user_name"),
                });
        }
        return result;
    }
}
}
