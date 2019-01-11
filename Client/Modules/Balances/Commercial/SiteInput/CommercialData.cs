using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.InputStates;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.Balances.Commercial.SiteInput
{
    public class CommercialData
    {
        public ContractDTO ContractDto { get; set; }

        public List<BalanceValueDTO> FactValues { get; set; }
        public List<BalanceValueDTO> PlanValues { get; set; }
        
        public Dictionary<Guid, double?> Measurings { get; set; }

        public List<GasOwnerDTO> Owners { get; set; }

        public List<ConsumerDTO> Consumers { get; set; }

        public List<MeasStationDTO> MeasStations { get; set; }

        public TreeDataDTO DistrStations { get; set; }

        public List<OperConsumerDTO> OperConsumers { get; set; }

        public InputStateDTO InputState { get; set; }


        public List<GasOwnerDTO> GetOwnerList(int systemId)
        {
            return Owners.Where(o => o.SystemList.Any(s => s == systemId)).ToList();
        }

        public List<ConsumerDTO> GetConsumerList(Guid stationId)
        {
            return DistrStations.Consumers.Where(c => c.ParentId == stationId && c.UseInBalance).ToList();
        }

        public List<MeasStationDTO> GetMeasStationList()
        {
            return MeasStations.Where(s => !s.IsIntermediate && s.BalanceSignId != Sign.NotUse).ToList();
        }

        public List<DistrStationDTO> GetDistrStationList()
        {
            return DistrStations.DistrStations;
        }

        public double? GetFactValue(Guid entityId, int ownerId, BalanceItem balItem, int coef)
        {
            return FactValues.SingleOrDefault(v => v.EntityId == entityId && v.GasOwnerId == ownerId && v.BalanceItem == balItem)?.BaseValue * coef;
        }

        public double? GetPlanValue(Guid entityId, int ownerId, BalanceItem balItem, int coef)
        {
            var val = PlanValues.SingleOrDefault(v => v.EntityId == entityId && v.GasOwnerId == ownerId && v.BalanceItem == balItem);

            if (val == null) return null;

            if (ContractDto.PeriodTypeId == PeriodType.Day)
            {
                var irrVal =
                    val.IrregularityList?.FirstOrDefault(
                        v =>
                            v.StartDayNum <= ContractDto.ContractDate.Day && ContractDto.ContractDate.Day <= v.EndDayNum)
                        ?.Value;
                return irrVal.HasValue ? irrVal * coef : (val.Correction ?? val.BaseValue) * coef / ContractDto.ContractDate.DaysInMonth();
            }

            return (val.Correction ?? val.BaseValue) * coef;
            
        }


        public static async Task<CommercialData> GetData(InputMode mode, DateTime date, int systemId, Guid siteId)
        {
            var data = new CommercialData();

            // Загрузить список владельцев газа
            data.Owners = await new BalancesServiceProxy().GetGasOwnerListAsync(systemId);

            // Загрузить точки входа и выхода из ГТС (ГИСы)
            data.MeasStations = await new ObjectModelServiceProxy().GetMeasStationListAsync(
                new GetMeasStationListParameterSet
                {
                    SiteId = siteId
                });

            // Загрузить список ГРС
            data.DistrStations = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                new GetDistrStationListParameterSet
                {
                    SiteId = siteId,
                    UseInBalance = true
                });

            data.OperConsumers = await new ObjectModelServiceProxy().GetOperConsumersAsync(
                new GetOperConsumerListParameterSet
                {
                    SystemId = systemId
                });

            var meas = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    Day = mode == InputMode.DayValue ? date.Day : (int?)null,
                    Month = date.Month,
                    Year = date.Year,
                    PeriodType = PeriodType.Day,
                    PropertyList = {PropertyType.Flow}
                });

            data.Measurings = new Dictionary<Guid, double?>();
            foreach (var m in meas)
            {
                if (mode == InputMode.DayValue)
                    data.Measurings.Add(m.Key,
                        m.Value.GetOrDefault(PropertyType.Flow)?
                            .OfType<PropertyValueDoubleDTO>()
                            .FirstOrDefault()?
                            .Value);
                else
                    data.Measurings.Add(m.Key,
                        m.Value.GetOrDefault(PropertyType.Flow)?
                            .OfType<PropertyValueDoubleDTO>()
                            .Sum(p => p.Value));
            }

            data.ContractDto = await new BalancesServiceProxy().GetContractAsync(
                new GetContractListParameterSet
                {
                    Day = date.Day,
                    Month = date.Month,
                    Year = date.Year,
                    SystemId = systemId,
                    PeriodTypeId = mode == InputMode.DayValue ? PeriodType.Day : PeriodType.Month,
                    TargetId = Target.Fact,
                    IsFinal = false
                });
            
            // Загрузить сохраненные значения за текущий период
            if (data.ContractDto != null)
            {
                data.FactValues = await new BalancesServiceProxy().GetBalanceValuesAsync(data.ContractDto.Id);

                var stateList = await new BalancesServiceProxy().GetInputStateListAsync(
                    new GetInputStateListParameterSet
                    {
                        ContractId = data.ContractDto.Id,
                        SiteId = siteId
                    });
                data.InputState = stateList.FirstOrDefault();
            }

            if (data.InputState == null)
                data.InputState = new InputStateDTO();


            var planContract = await new BalancesServiceProxy().GetContractAsync(
                new GetContractListParameterSet
                {
                    Day = 1,
                    Month = date.Month,
                    Year = date.Year,
                    SystemId = systemId,
                    PeriodTypeId = PeriodType.Month,
                    TargetId = Target.Plan,
                    IsFinal = true
                });

            // Загрузить сохраненные значения за текущий период
            if (planContract != null)
                data.PlanValues = await new BalancesServiceProxy().GetBalanceValuesAsync(planContract.Id);

            //var x = new GasCostsServiceProxy().GetGasCostListAsync(
            //    new GetGasCostListParameterSet
            //    {
            //        StartDate = day == day.MonthStart() ? day.AddDays(-1) : day.MonthStart(),
            //        EndDate = day,
            //        Target = Target.Fact
            //    });

            return data;
        }
    }
}