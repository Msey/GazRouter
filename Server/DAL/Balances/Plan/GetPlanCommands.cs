using System;
using System.Collections.Generic;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Plan;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Plan
{
    public abstract class GetPlanItemsQueryBase<TItem, TCorrection, TPointKey> : QueryReader<int, Tuple<List<TItem>, List<TCorrection>, List<TCorrection>, List<TCorrection>>>
        where TItem : PlanItemDto<TPointKey>, new()
        where TCorrection : PlanItemDto<TPointKey>, ICorrection, new()
    {
        protected GetPlanItemsQueryBase(ExecutionContext context)
            : base(context)
        {
        }

        protected override void BindParameters(OracleCommand command, int parameter)
        {
            command.AddInputParameter("contractId", parameter);
        }

        protected override Tuple<List<TItem>, List<TCorrection>, List<TCorrection>, List<TCorrection>> GetResult(OracleDataReader reader, int parameter)
        {
            var items = new List<TItem>();
            var commercialCorrections = new List<TCorrection>();
            var operativeCorrections = new List<TCorrection>();
            var irregularCorrections = new List<TCorrection>();

            while (reader.Read())
            {
                PlanItemDto<TPointKey> item;
                var valueType = reader.GetValue<BalanceValueType>("value_type");
                if (valueType == BalanceValueType.Base)
                {
                    var planItem = new TItem();
                    item = planItem;
                    items.Add(planItem);
                }
                else
                {
                    var correction = new TCorrection
                        {
                            StartDate = reader.GetValue<DateTime>("start_date"),
                            EndDate = reader.GetValue<DateTime>("end_date"),
                            FileDescription = reader.GetValue<string>("description"),
                            DocId = reader.GetValue<int?>("doc_id")
                        };
                    item = correction;
                    switch (valueType)
                    {
                        case BalanceValueType.Commercial:
                            commercialCorrections.Add(correction);
                            break;
                        case BalanceValueType.Operative:
                            operativeCorrections.Add(correction);
                            break;
                        case BalanceValueType.Irregularity:
                            irregularCorrections.Add(correction);
                            break;
                    }
                   
                }
                item.GasOwnerId = reader.GetValue<int>("gas_owner_id");
                FillPointId(reader, item);
                item.Value = (long)Math.Round(reader.GetValue<double>("value") * 1000);
                item.ValueType = valueType;
            }

            return
                new Tuple<List<TItem>, List<TCorrection>, List<TCorrection>,List<TCorrection>>(items, commercialCorrections,
                                           operativeCorrections, irregularCorrections);
        }

        protected override string GetCommandText(int parameter)
        {
            return Query;
        }

        protected abstract void FillPointId(OracleDataReader reader, PlanItemDto<TPointKey> planItem);
        protected abstract string Query { get; }
    }

    public class GetConsumptionQuery : GetPlanItemsQueryBase<ConsumptionItemDto, ConsumptionCorrectionDto, Guid>
    {
        public GetConsumptionQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Query
        {
            get
            {
                return @"
select con.gas_owner_id, con.gas_consumer_id, con.value, con.start_date, con.end_date, con.value_type, doc.description, doc.doc_id
from v_bl_consumptions con left join v_bl_docs doc on con.doc_id = doc.doc_id
where con.contract_id = :contractId";
            }
        }

        protected override void FillPointId(OracleDataReader reader, PlanItemDto<Guid> planItem)
        {
            planItem.PointId = reader.GetValue<Guid>("gas_consumer_id");
        }
    }

    public class GetAuxConsumptionQuery : GetPlanItemsQueryBase<AuxConsumptionItemDto, AuxConsumptionCorrectionDto, Guid>
    {
        public GetAuxConsumptionQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Query
        {
            get
            {
                return @"
select con.gas_owner_id, con.meas_station_id, con.value, con.start_date, con.end_date, con.value_type, doc.description, doc.doc_id
from v_bl_aux_consumptions con left join v_bl_docs doc on con.doc_id = doc.doc_id
where con.contract_id = :contractId";
            }
        }

        protected override void FillPointId(OracleDataReader reader, PlanItemDto<Guid> planItem)
        {
            planItem.PointId = reader.GetValue<Guid>("meas_station_id");
        }
    }

    public class GetOperConsumptionQuery : GetPlanItemsQueryBase<OperConsumptionItemDto, OperConsumptionCorrectionDto, int>
    {
        public GetOperConsumptionQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Query
        {
            get
            {
                return @"
select con.gas_owner_id, con.oper_consumer_id, con.value, con.start_date, con.end_date, con.value_type, doc.description, doc.doc_id
from v_bl_oper_consumptions con left join v_bl_docs doc on con.doc_id = doc.doc_id
where con.contract_id = :contractId";
            }
        }

        protected override void FillPointId(OracleDataReader reader, PlanItemDto<int> planItem)
        {
            planItem.PointId = reader.GetValue<int>("oper_consumer_id");
        }
    }

    public abstract class GetIntakeTransitQuery : GetPlanItemsQueryBase<IntakeTransitItemDto, IntakeTransitCorrectionDto, Guid>
    {
        protected GetIntakeTransitQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override void FillPointId(OracleDataReader reader, PlanItemDto<Guid> planItem)
        {
            planItem.PointId = reader.GetValue<Guid>("meas_station_id");
        }
    }

    public class GetIntakeQuery : GetIntakeTransitQuery
    {
        public GetIntakeQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Query
        {
            get
            {
                return @"
select bit.gas_owner_id, bit.meas_station_id, bit.value, bit.start_date, bit.end_date, bit.value_type, doc.description, doc.doc_id
from v_bl_intake_transits bit left join v_bl_docs doc on bit.doc_id = doc.doc_id
join v_meas_stations ms on bit.meas_station_id = ms.meas_station_id
where bit.contract_id = :contractId
and ms.balance_sign_id = 10";
            }
        }
    }

    public class GetTransitQuery : GetIntakeTransitQuery
    {
        public GetTransitQuery(ExecutionContext context)
            : base(context)
        {
        }

        protected override string Query
        {
            get
            {
                return @"
select bit.gas_owner_id, bit.meas_station_id, bit.value, bit.start_date, bit.end_date, bit.value_type, doc.description, doc.doc_id
from v_bl_intake_transits bit left join v_bl_docs doc on bit.doc_id = doc.doc_id
join v_meas_stations ms on bit.meas_station_id = ms.meas_station_id
where bit.contract_id = :contractId
and ms.balance_sign_id = 11";
            }
        }
    }

}