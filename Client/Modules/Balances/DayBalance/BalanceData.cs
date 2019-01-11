using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.DTO.Balances.DayBalance;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Dictionaries.AggregatorTypes;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.Balances.DayBalance
{
    public class BalanceData
    {
        public DateTime Day { get; set; }

        public GasTransportSystemDTO SystemDto { get; set; }

        public DayBalanceDataDTO DataDto { get; set; }

        public BalanceData(DayBalanceDataDTO dataDto, GasTransportSystemDTO systemDto, DateTime day)
        {
            DataDto = dataDto;
            Day = day;
            SystemDto = systemDto;
        }
        

        public List<MeasStationDTO> GetMeasStationList(Sign sign, int? balGroupId)
        {
            // Если указана балансовая группа, то возвращаем все ГИС данной группы
            // Если группа не указа, то из выборки исключаем промежуточные ГИС (IsIntemediate == true)
            var balItem = sign == Sign.In ? BalanceItem.Intake : BalanceItem.Transit;
            return
                GetSortedList(
                    DataDto.MeasStations.Where(s => s.BalanceSignId == sign)
                        .Where(s => !balGroupId.HasValue || s.BalanceGroupId == balGroupId)
                        .Where(s => balGroupId.HasValue || !s.IsIntermediate), balItem).ToList();
        }

        public List<MeasLineDTO> GetMeasLineList(MeasStationDTO station)
        {
            var section = station.BalanceSignId == Sign.In ? BalanceItem.Intake : BalanceItem.Transit;
            return GetSortedList(DataDto.MeasLines.Where(l => l.ParentId == station.Id), section).ToList();
        }

        public List<SiteDTO> GetSiteList(BalanceItem balItem)
        {
            return GetSortedList(DataDto.Sites, balItem).ToList();
        }

        public List<EnterpriseDTO> GetEnterpriseList(BalanceItem balItem)
        {
            return GetSortedList(DataDto.Enterprises, balItem).ToList();
        }

        public List<DistrStationDTO> GetDistrStationList(int? balGroupId)
        {
            return
                GetSortedList(DataDto.DistrStations.Where(s => !balGroupId.HasValue || s.BalanceGroupId == balGroupId),
                    BalanceItem.Consumers).ToList();
        }

        public List<DistrStationOutletDTO> GetDistrStationOutletList(Guid stationId)
        {
            return GetSortedList(DataDto.DistrStationOutlets.Where(o => o.ParentId == stationId), BalanceItem.Consumers).ToList();
        }

        public List<OperConsumerDTO> GetOperConsumerList(int? balGroupId)
        {
            return
                GetSortedList(DataDto.OperConsumers.Where(c => !balGroupId.HasValue || c.BalanceGroupId == balGroupId),
                    BalanceItem.OperConsumers).ToList();
        }

        public List<CommonEntityDTO> GetMiscObject()
        {
            return DataDto.MiscTabEntities;
        }

        public List<GasCostDTO> GetGasCosts(int? balGroupId)
        {
            return DataDto.AuxCosts.Where(c => !balGroupId.HasValue || c.BalanceGroupId == balGroupId).ToList();
        } 

        public AggregatorDTO GetGasSupply(int? balGroupId)
        {
            return DataDto.Aggregators.SingleOrDefault(a => a.BalanceGroupId == balGroupId && a.AggregatorType == AggregatorType.GasSupply);
        }

        public AggregatorDTO GetBalanceLoss(int? balGroupId)
        {
            return DataDto.Aggregators.SingleOrDefault(a => a.BalanceGroupId == balGroupId && a.AggregatorType == AggregatorType.BalanceLoss);
        }


        private IEnumerable<T> GetSortedList<T>(IEnumerable<T> entityList, BalanceItem balItem)
            where T : CommonEntityDTO
        {
            var res = entityList;
            foreach (var e in res)
            {
                e.SortOrder =
                    DataDto.SortOrderList.SingleOrDefault(so => so.EntityId == e.Id && so.BalItem == balItem)?.SortOrder ??
                    e.SortOrder;
            }
            return res.OrderBy(e => e.SortOrder);
        }


        public void GetFactValue(ItemBase item)
        {
            if (item != null && DataDto.FactValues.ContainsKey(item.Entity.Id))
            {
                var entityValues = DataDto.FactValues[item.Entity.Id];
                if (entityValues.ContainsKey(PropertyType.Flow))
                {
                    var prevDay = Day.AddDays(-1);
                    var propValues = entityValues[PropertyType.Flow].OfType<PropertyValueDoubleDTO>().ToList();
                    item.Current = propValues.SingleOrDefault(v => v.Day == Day.Day && v.Month == Day.Month)?.Value;
                    item.Prev = propValues.SingleOrDefault(v => v.Day == prevDay.Day && v.Month == prevDay.Month)?.Value;
                    item.MonthTotal = propValues.Where(v => v.Month == Day.Month && v.Day < Day.Day).Sum(v => v.Value);
                }
            }
        }


        public void GetPlanValue(ItemBase item)
        {
            if (DataDto.PlanValues == null) return;

            if (item.Entity.EntityType == EntityType.DistrStation)
            {
                var stationValues = DataDto.PlanValues.Where(v => v.DistrStationId == item.Entity.Id).ToList();
                var volDay = 0.0;
                var volMonth = 0.0;
                foreach (var value in stationValues)
                {
                    var irrVal = value.IrregularityList.FirstOrDefault(v => v.StartDayNum <= Day.Day && Day.Day <= v.EndDayNum)?.Value;
                    if (irrVal.HasValue) volDay += irrVal.Value;
                    else volDay += value.BaseValue / Day.DaysInMonth();

                    volMonth += value.BaseValue;
                }
                item.DayPlan = volDay;
                item.MonthPlan = volMonth;
            }
            else
            {
                var stationValues = DataDto.PlanValues.Where(v => v.EntityId == item.Entity.Id).ToList();
                var volDay = 0.0;
                var volMonth = 0.0;
                foreach (var value in stationValues)
                {
                    var irrVal = value.IrregularityList.FirstOrDefault(v => v.StartDayNum <= Day.Day && Day.Day <= v.EndDayNum)?.Value;
                    if (irrVal.HasValue) volDay += irrVal.Value;
                    else volDay += value.BaseValue / Day.DaysInMonth();

                    volMonth += value.BaseValue;
                }
                item.DayPlan = volDay;
                item.MonthPlan = volMonth;
            }
        }
    }
}