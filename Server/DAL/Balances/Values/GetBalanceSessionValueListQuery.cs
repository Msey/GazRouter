using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.DAL.Balances.Values
{

    public class GetBalanceSessionValueListQuery : QueryReader<SessionDataParameterSet, List<BalanceExchangeValueDTO>>
    {
        public GetBalanceSessionValueListQuery(ExecutionContext context)
            : base(context)
        { }

        protected override string GetCommandText(SessionDataParameterSet parameters)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(
                   $@"   SELECT     v.value_id,
                                    v.contract_id,
                                    v.gas_owner_id,
                                    v.entity_id,
                                    v.entity_type_id,
                                    c.contract_date,
                                    pa.entity_name,
                                    pt.property_type_id,
                                    pt.unit_name,
                                    pt.description,
                                    pt.value_type,
                                    c.distr_station_id,
                                    v.item_id,
                                    v.value_base,
                                    v.value_correct,
                                    sp.parameter_gid
                        FROM  rd.v_bl_values v
                        inner join rd.v_nm_all pa on pa.entity_id=v.entity_id
                        inner join rd.v_bl_contracts c on c.contract_id = v.contract_id
                        inner join integro.V_SUMMARY_PARAMETER_CONTENT spc on v.entity_id = spc.entity_id
                        inner join integro.V_SUMMARY_PARAMETERS sp on spc.summary_parameter_id=sp.summary_parameter_id
                        left join  rd.v_gas_consumers c ON c.gas_consumer_id = v.entity_id
                        left join rd.v_property_types pt on pt.sys_name = 'flow'
                        WHERE  1 = 1");
            if (parameters.ContractIds != null && parameters.ContractIds.Any())
                queryBuilder.Append(GetBindParameters(parameters.ContractIds));
            if (parameters.SummaryId.HasValue)
                queryBuilder.Append($" AND sp.SUMMARY_ID =:summaryid");
            return  queryBuilder.ToString();
        }

        protected static string GetBindParameters(List<int> ids)
        {
            if (ids.Count == 0) return string.Empty;
            return $" AND v.contract_id IN ({string.Join(",", ids.Select(et => et))})";
            //command.AddInputParameter("contractid", parameters);
        }
        protected override void BindParameters(OracleCommand command, SessionDataParameterSet parameters)
        {
            command.AddInputParameter("summaryid", parameters.SummaryId);
        }

        protected override List<BalanceExchangeValueDTO> GetResult(OracleDataReader reader, SessionDataParameterSet parameters)
        {
            var result = new List<BalanceExchangeValueDTO>();

            while (reader.Read())
            {
                result.Add(
                    new BalanceExchangeValueDTO
                    {
                        Id = reader.GetValue<int>("value_id"),
                        ContractId = reader.GetValue<int>("contract_id"),
                        GasOwnerId = reader.GetValue<int>("gas_owner_id"),
                        EntityId = reader.GetValue<Guid>("entity_id"),
                        EntityType = reader.GetValue<EntityType?>("entity_type_id"),
                        EntityName = reader.GetValue<string>("entity_name"),
                        PropertyTypeId = reader.GetValue<int>("property_type_id"),
                        PropertyUnitName = reader.GetValue<string>("unit_name"),
                        PropertyDescription = reader.GetValue<string>("description"),
                        PropertyValueType = reader.GetValue<string>("value_type"),
                        DistrStationId = reader.GetValue<Guid?>("distr_station_id"),
                        BalanceItem = reader.GetValue<BalanceItem>("item_id"),
                        BaseValue = reader.GetValue<double>("value_base"),
                        Correction = reader.GetValue<double?>("value_correct"),
                        ParameterGidString = reader.GetValue<string>("parameter_gid"),
                        ContractDate = reader.GetValue<DateTime>("contract_date"),
                    });
            }

            //if (result == null || !result.Any())
            //{
            //    string contractIds = parameters.ContractIds != null ? string.Join(",", parameters.ContractIds) : "NULL";
            //    string summaryId = parameters.SummaryId.HasValue ? parameters.SummaryId.Value.ToString() : "NULL";
            //    result.Add(new BalanceExchangeValueDTO() {
            //        SqlQuery = $"{this.GetCommandText(parameters)}  v.contract_id IN ({contractIds}) and summaryid='{parameters.SummaryId}'"
            //    });
            //}
            return result;
        }


    }
}
