using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.DistrNetworks;
using GazRouter.DTO.Balances.Docs;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Balances.Swaps;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Balances.Commercial.Common
{
    public class BalanceDataBase
    {
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public DateTime Month { get; set; }

        public GasTransportSystemDTO SystemDto { get; set; }

        
        public List<BalanceGroupDTO> BalanceGroups { get; set; } 

        public List<BalSortOrderDTO> SortOrderList { get; set; } 

        public List<GasOwnerDTO> Owners { get; set; }

        public List<GasOwnerDisableDTO> OwnerDisables { get; set; }

        public List<SiteDTO> Sites { get; set; }

        public List<MeasStationDTO> MeasStations { get; set; }

        public TreeDataDTO DistrStations { get; set; }

        public List<OperConsumerDTO> OperConsumers { get; set; }

        public ContractDTO FactContract { get; set; }

        public BalanceValues FactValues { get; set; }

        public ContractDTO PlanContract { get; set; }

        public BalanceValues PlanValues { get; set; }

        public List<EnterpriseDTO> Enterprises => ClientCache.DictionaryRepository.Enterprises;

        public List<RegionDTO> Regions => ClientCache.DictionaryRepository.Regions; 

        public List<DistrNetworkDTO> DistrNetworks { get; set; }

        public List<DocDTO> CorrectionDocs { get; set; } 

        public List<SwapDTO> SwapList { get; set; } 

        public async static Task<BalanceDataBase> GetData(GasTransportSystemDTO systemDto, DateTime month, Target target, bool isFinal)
        {
            var data = new BalanceDataBase
            {
                SystemDto = systemDto,
                Month = month
            };

            data.BalanceGroups = await new BalancesServiceProxy().GetBalanceGroupListAsync(systemDto.Id);

            data.SortOrderList = await new BalancesServiceProxy().GetSortOrderListAsync();

            data.Sites = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    SystemId = systemDto.Id,
                    EnterpriseId = UserProfile.Current.Site.Id
                });

            // Загрузить список владельцев газа
            data.Owners = await new BalancesServiceProxy().GetGasOwnerListAsync(systemDto.Id);

            data.OwnerDisables = await new BalancesServiceProxy().GetGasOwnerDisableListAsync();

            
            // Загрузить точки входа и выхода из ГТС (ГИСы)
            data.MeasStations = await new ObjectModelServiceProxy().GetMeasStationListAsync(
                new GetMeasStationListParameterSet
                {
                    SystemId = systemDto.Id,
                    ThisEnterprise = true
                });

            // Загрузить список ГРС
            data.DistrStations = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                new GetDistrStationListParameterSet
                {
                    SystemId = systemDto.Id,
                    UseInBalance = true,
                    ThisEnterprise = true
                });

            // Загрузить список потребителей ПЭН
            data.OperConsumers = await new ObjectModelServiceProxy().GetOperConsumersAsync(
                new GetOperConsumerListParameterSet
                {
                    SystemId = systemDto.Id
                });

            data.DistrNetworks = await new BalancesServiceProxy().GetDistrNetworkListAsync();

            if (target == Target.Fact)
            {
                data.FactContract = await new BalancesServiceProxy().GetContractAsync(
                    new GetContractListParameterSet
                    {
                        Day = 1,
                        Month = month.Month,
                        Year = month.Year,
                        SystemId = systemDto.Id,
                        PeriodTypeId = PeriodType.Month,
                        TargetId = Target.Fact,
                        IsFinal = isFinal
                    });

                if (data.FactContract != null)
                {
                    data.FactValues =
                        new BalanceValues(await new BalancesServiceProxy().GetBalanceValuesAsync(data.FactContract.Id));

                    data.SwapList = await new BalancesServiceProxy().GetValueSwapsAsync(data.FactContract.Id);
                }
            }

            data.PlanContract = await new BalancesServiceProxy().GetContractAsync(
                new GetContractListParameterSet
                {
                    Day = 1,
                    Month = month.Month,
                    Year = month.Year,
                    SystemId = systemDto.Id,
                    PeriodTypeId = PeriodType.Month,
                    TargetId = Target.Plan,
                    IsFinal = target == Target.Fact || isFinal
                });

            if (data.PlanContract != null)
            {
                data.PlanValues =
                    new BalanceValues(await new BalancesServiceProxy().GetBalanceValuesAsync(data.PlanContract.Id));
                data.CorrectionDocs = await new BalancesServiceProxy().GetDocListAsync(data.PlanContract.Id);
            }

            return data;
        }


        public ContractDTO GetContract(Target target)
        {
            return target == Target.Plan ? PlanContract : FactContract;
        }

        public List<MeasStationDTO> GetMeasStationList(BalanceItem balItem, int? balGroupId)
        {
            var sign = balItem == BalanceItem.Transit ? Sign.Out : Sign.In;
            return
                GetSortedList(
                    MeasStations.Where(s => s.BalanceSignId == sign)
                        .Where(s => !balGroupId.HasValue || s.BalanceGroupId == balGroupId)
                        .Where(s => balGroupId.HasValue || !s.IsIntermediate).ToList(), balItem).ToList();
        }

        public List<SiteDTO> GetSiteList(BalanceItem balItem, int? balGroupId)
        {
            return GetSortedList(Sites.Where(s => !balGroupId.HasValue || s.BalanceGroupId == balGroupId).ToList(), balItem).ToList();
        }

        public List<EnterpriseDTO> GetEnterpriseList(BalanceItem balItem)
        {
            return GetSortedList(Enterprises, balItem).ToList();
        }

        public List<DistrStationDTO> GetDistrStationList(int? balGroupId)
        {
            return
                GetSortedList(
                    DistrStations.DistrStations.Where(s => !balGroupId.HasValue || s.BalanceGroupId == balGroupId).ToList(),
                    BalanceItem.Consumers).ToList();
        }

        public List<ConsumerDTO> GetConsumerList(Guid? stationId)
        {
            return
                DistrStations.Consumers.Where(c => c.UseInBalance)
                    .Where(c => !stationId.HasValue || c.ParentId == stationId)
                    .ToList();
        }

        public List<OperConsumerDTO> GetOperConsumerList(int? balGroupId)
        {
            return
                GetSortedList(OperConsumers.Where(c => !balGroupId.HasValue || c.BalanceGroupId == balGroupId).ToList(),
                    BalanceItem.OperConsumers).ToList();
        }

        public bool GetOwnerVisibility(int ownerId, Guid entityId, BalanceItem balItem)
        {
            return !OwnerDisables.Any(d => d.OwnerId == ownerId && d.EntityId == entityId && d.BalanceItem == balItem);
        }

        public List<GasOwnerDTO> GetVisibleOwnerList(Guid entityId, BalanceItem balItem)
        {
            return
                Owners.Where(
                    o =>
                        !OwnerDisables.Any(d => d.OwnerId == o.Id && d.EntityId == entityId && d.BalanceItem == balItem))
                    .ToList();
        }
        

        private List<T> GetSortedList<T>(List<T> entityList, BalanceItem balItem)
            where T : CommonEntityDTO
        {
            var res = entityList;
            foreach (var e in res)
            {
                e.SortOrder =
                    SortOrderList.SingleOrDefault(so => so.EntityId == e.Id && so.BalItem == balItem)?.SortOrder ??
                    e.SortOrder;
            }
            return res.OrderBy(e => e.SortOrder).ToList();
        }

        public List<AddSwapParameterSet> GetAddSwapParameterSets()
        {
            return SwapList?.Select(s =>
                new AddSwapParameterSet
                {
                    ContractId = FactContract.Id,
                    EntityId = s.EntityId,
                    BalItem = s.BalItem,
                    SrcOwnerId = s.SrcOwnerId,
                    DestOwnerId = s.DestOwnerId,
                    Volume = s.Volume
                }).ToList() ?? new List<AddSwapParameterSet>();
        } 
       
    }
}