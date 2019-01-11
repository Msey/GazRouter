using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Balances.Commercial.Fact;
using GazRouter.Balances.Commercial.Plan;
using GazRouter.Balances.Common.TreeGroupType;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.ObjectBuilder2;

namespace GazRouter.Balances.Commercial.Common
{
    public class BalanceTreeBuilder
    {
        private Target _target;
        private BalanceDataBase _data;
        private double _coef;
        private List<OwnerItem> _ownerItems;
        private ItemActions _actions;


        public BalanceTreeBuilder(Target target, BalanceDataBase data, double coef, ItemActions actions)
        {
            _target = target;
            _data = data;
            _coef = coef;
            _actions = actions;

            CreateOwners();
        }


        #region OWNERS

        public List<OwnerItem> OwnerItems => _ownerItems;

        public OwnerItem GetOwnerItem(Guid entityId, int ownerId, BalanceItem balItem)
        {
            return GetOwnerItemList(entityId, balItem, ownerId).SingleOrDefault();
        }

        private void CreateOwners()
        {
            _ownerItems = new List<OwnerItem>();

            if (_data == null) return;

            var enterprise = _data.Enterprises.Single(e => e.Id == UserProfile.Current.Site.Id);

            // Поступление
            _data.GetMeasStationList(BalanceItem.Intake, null)
                .ForEach(s => _ownerItems.AddRange(CreateOwnerItemList(s, BalanceItem.Intake)));

            // Транзит
            _data.GetMeasStationList(BalanceItem.Transit, null)
                .ForEach(s => _ownerItems.AddRange(CreateOwnerItemList(s, BalanceItem.Transit)));

            // Потребители
            _data.DistrStations.Consumers.Where(c => c.UseInBalance)
                .ForEach(c => _ownerItems.AddRange(CreateOwnerItemList(c, BalanceItem.Consumers)));

            // CTH
            _ownerItems.AddRange(CreateOwnerItemList(enterprise, BalanceItem.AuxCosts));

            // БАЛАНСОВЫЕ ПОТЕРИ
            _ownerItems.AddRange(CreateOwnerItemList(enterprise, BalanceItem.BalanceLoss));


            // ПЭН
            _data.OperConsumers.ForEach(
                c => _ownerItems.AddRange(CreateOwnerItemList(c, BalanceItem.OperConsumers)));


            // Запас газа
            _data.GetMeasStationList(BalanceItem.GasSupply, null)
                .ForEach(s => _ownerItems.AddRange(CreateOwnerItemList(s, BalanceItem.GasSupply)));

            // ТТР
            _ownerItems.AddRange(CreateOwnerItemList(enterprise, BalanceItem.Transport));


        }

        private OwnerItem CreateOwnerItem(GasOwnerDTO owner, EntityDTO entity, BalanceItem balItem)
        {
            OwnerItem ownerItem = null;
            if (_target == Target.Plan) ownerItem = new PlanOwnerItem(owner, entity, balItem, _actions);
            if (_target == Target.Fact) ownerItem = new FactOwnerItem(owner, entity, balItem, _actions);

            if (ownerItem != null)
            {
                ownerItem.InitValues(_data.PlanValues, Target.Plan, _coef);
                if (_target == Target.Fact) ownerItem.InitValues(_data.FactValues, Target.Fact, _coef);
            }

            return ownerItem;
        }

        private List<OwnerItem> CreateOwnerItemList(EntityDTO entity, BalanceItem balItem)
        {
            return _data.Owners.Select(owner => CreateOwnerItem(owner, entity, balItem)).ToList();
        }

        private List<OwnerItem> GetOwnerItemList(Guid entityId, BalanceItem balItem, int? ownerId)
        {
            return
                _ownerItems.Where(i => i.Entity.Id == entityId && i.BalItem == balItem)
                    .Where(i => !ownerId.HasValue || i.Owner.Id == ownerId)
                    .Where(i => _data.GetOwnerVisibility(i.Owner.Id, entityId, balItem) || i.HasValues)
                    .ToList();
        }
        #endregion

        
        #region ROOT ITEMS

        public SummaryItem Intake { get; set; }

        public SummaryItem Transit { get; set; }

        public SummaryItem Consumers { get; set; }

        public SummaryItem AuxCosts { get; set; }

        public SummaryItem BalanceLoss { get; set; }

        public SummaryItem OperConsumers { get; set; }

        public SummaryItem GasSupply { get; set; }

        public SummaryItem Transport { get; set; }

        #endregion

        public void RefreshTrees(int? ownerId)
        {
            RefreshIntakeTree(BalanceGroupTypes.MeasStationGroupType, ownerId);
            RefreshTransitTree(BalanceGroupTypes.MeasStationGroupType, ownerId);
            RefreshConsumersTree(BalanceGroupTypes.DistrStationGroupType, ownerId);
            RefreshAuxCostsTree(ownerId);
            RefreshOperConsumersTree(BalanceGroupTypes.OperConsumerGroupType, ownerId);
            RefreshTransportTree(ownerId);
            if (_target == Target.Fact)
            {
                RefreshBalanceLossTree(ownerId);
                RefreshGasSupplyTree(BalanceGroupTypes.MeasStationGroupType, ownerId);
            }
        }

        private void RefreshIntakeTree(TreeGroupType groupType, int? ownerId)
        {
            Intake = CreateSummaryItem("ПОСТУПЛЕНИЕ ВСЕГО:", true);
            Intake.AddChildList(GetMeasStationTree(BalanceItem.Intake, groupType, ownerId));
        }

        private void RefreshTransitTree(TreeGroupType groupType, int? ownerId)
        {
            Transit = CreateSummaryItem("ПОДАЧА ВСЕГО:", true);
            Transit.AddChildList(GetMeasStationTree(BalanceItem.Transit, groupType, ownerId));
        }

        private List<ItemBase> GetMeasStationTree(BalanceItem balItem, TreeGroupType groupType, int? ownerId)
        {
            var result = new List<ItemBase>();
            var stations = _data.GetMeasStationList(balItem, null);

            switch (groupType)
            {
                case TreeGroupType.None:
                    foreach (var station in stations)
                    {
                        var measStationItem = CreateSummaryItem(station, balItem, true);
                        measStationItem.AddChildList(GetOwnerItemList(station.Id, balItem, ownerId));
                        if (measStationItem.Childs.Any() || !ownerId.HasValue)
                            result.Add(measStationItem);
                    }
                    break;

                case TreeGroupType.BySite:
                    foreach (var site in _data.GetSiteList(balItem, null))
                    {
                        var siteItem = CreateSummaryItem(site, balItem);
                        foreach (var station in stations.Where(s => s.ParentId == site.Id))
                        {
                            var measStationItem = CreateSummaryItem(station, balItem, true);
                            measStationItem.AddChildList(GetOwnerItemList(station.Id, balItem, ownerId));
                            if (measStationItem.Childs.Any() || !ownerId.HasValue)
                                siteItem.AddChild(measStationItem);
                        }
                        if (siteItem.Childs.Any())
                            result.Add(siteItem);
                    }
                    break;

                case TreeGroupType.ByEnterprise:
                    foreach (var ent in _data.GetEnterpriseList(balItem))
                    {
                        var entItem = CreateSummaryItem(ent, balItem);
                        foreach (var station in stations.Where(s => s.NeighbourEnterpriseId == ent.Id))
                        {
                            var measStationItem = CreateSummaryItem(station, balItem, true);
                            measStationItem.AddChildList(GetOwnerItemList(station.Id, balItem, ownerId));
                            if (measStationItem.Childs.Any() || !ownerId.HasValue)
                                entItem.AddChild(measStationItem);
                        }
                        if (entItem.Childs.Any())
                            result.Add(entItem);
                    }

                    // Для ГИСов, у которых не задано смежное предприятие
                    var noEnterpriseItem = CreateSummaryItem("<Предприятие не задано>", false);
                    foreach (var station in stations.Where(s => !s.NeighbourEnterpriseId.HasValue))
                    {
                        var measStationItem = CreateSummaryItem(station, balItem, true);
                        measStationItem.AddChildList(GetOwnerItemList(station.Id, balItem, ownerId));
                        if (measStationItem.Childs.Any() || !ownerId.HasValue)
                            noEnterpriseItem.AddChild(measStationItem);
                    }
                    if (noEnterpriseItem.Childs.Any())
                        result.Add(noEnterpriseItem);
                    break;
            }

            return result;
        }

        public void RefreshConsumersTree(TreeGroupType groupType, int? ownerId)
        {
            var balItem = BalanceItem.Consumers;
            Consumers = CreateSummaryItem("ПОТРЕБИТЕЛИ ВСЕГО:", true);

            var stations = _data.GetDistrStationList(null);
            switch (groupType)
            {
                case TreeGroupType.None:
                    foreach (var station in stations)
                    {
                        var stationItem = CreateSummaryItem(station, balItem);
                        if (station.IsForeign) stationItem.Alias = $"{station.Name} (cтор.)";
                        foreach (var consumer in _data.GetConsumerList(station.Id))
                        {
                            var consumerItem = CreateSummaryItem(consumer, balItem, true);
                            consumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                            if (consumerItem.Childs.Any() || !ownerId.HasValue)
                                stationItem.AddChild(consumerItem);
                        }
                        if (stationItem.Childs.Any())
                            Consumers.AddChild(stationItem);
                    }
                    break;

                case TreeGroupType.BySite:
                    foreach (var site in _data.GetSiteList(balItem, null))
                    {
                        var siteItem = CreateSummaryItem(site, balItem);
                        foreach (var station in stations.Where(s => s.ParentId == site.Id))
                        {
                            var stationItem = CreateSummaryItem(station, balItem);
                            if (station.IsForeign) stationItem.Alias = $"{station.Name} (cтор.)";
                            foreach (var consumer in _data.GetConsumerList(station.Id))
                            {
                                var consumerItem = CreateSummaryItem(consumer, balItem, true);
                                consumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                                if (consumerItem.Childs.Any() || !ownerId.HasValue)
                                    stationItem.AddChild(consumerItem);
                            }
                            if (stationItem.Childs.Any())
                                siteItem.AddChild(stationItem);
                        }
                        if (siteItem.Childs.Any())
                            Consumers.AddChild(siteItem);
                    }
                    break;

                case TreeGroupType.ByRegion:
                    foreach (var region in _data.Regions)
                    {
                        var regionConsumers = _data.GetConsumerList(null).Where(c => c.RegionId == region.Id).ToList();
                        if (regionConsumers.Count == 0) continue;
                        
                        var regionItem = CreateSummaryItem(region.Name, false);
                        foreach (var station in stations.Where(s => regionConsumers.Any(c => c.ParentId == s.Id)))
                        {
                            var stationItem = CreateSummaryItem(station, balItem);
                            if (station.IsForeign) stationItem.Alias = $"{station.Name} (cтор.)";
                            foreach (var consumer in regionConsumers.Where(c => c.ParentId == station.Id))
                            {
                                var consumerItem = CreateSummaryItem(consumer, balItem, true);
                                consumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                                if (consumerItem.Childs.Any() || !ownerId.HasValue)
                                    stationItem.AddChild(consumerItem);
                            }
                            if (stationItem.Childs.Any())
                                regionItem.AddChild(stationItem);
                        }
                        if (regionItem.Childs.Any())
                            Consumers.AddChild(regionItem);
                    }
                    break;

                case TreeGroupType.ByRegionAndSite:
                    var sites = _data.GetSiteList(balItem, null);
                    foreach (var region in _data.Regions)
                    {
                        var regionConsumers = _data.GetConsumerList(null).Where(c => c.RegionId == region.Id).ToList();
                        if (regionConsumers.Count == 0) continue;

                        var regionItem = CreateSummaryItem(region.Name, false);
                        foreach (var site in sites)
                        {
                            var siteItem = CreateSummaryItem(site, balItem);
                            foreach (var station in stations.Where(s => s.ParentId == site.Id && regionConsumers.Any(c => c.ParentId == s.Id)))
                            {
                                var stationItem = CreateSummaryItem(station, balItem);
                                if (station.IsForeign) stationItem.Alias = $"{station.Name} (cтор.)";
                                foreach (var consumer in regionConsumers.Where(c => c.ParentId == station.Id))
                                {
                                    var consumerItem = CreateSummaryItem(consumer, balItem, true);
                                    consumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                                    if (consumerItem.Childs.Any() || !ownerId.HasValue)
                                        stationItem.AddChild(consumerItem);
                                }
                                if (stationItem.Childs.Any())
                                    siteItem.AddChild(stationItem);
                            }
                            if (siteItem.Childs.Any())
                                regionItem.AddChild(siteItem);
                        }
                        
                        if (regionItem.Childs.Any())
                            Consumers.AddChild(regionItem);
                    }
                    break;

                case TreeGroupType.ByDistrNetworks:
                    foreach (var network in _data.DistrNetworks)
                    {
                        var networkItem = CreateSummaryItem(network.Name, false);
                        var consumers = _data.GetConsumerList(null).Where(c => c.DistrNetworkId == network.Id).ToList();
                        foreach (var station in _data.DistrStations.DistrStations.Where(s => consumers.Any(c => c.ParentId == s.Id)))
                        {
                            var stationItem = CreateSummaryItem(station, balItem, true);
                            if (station.IsForeign) stationItem.Alias = $"{station.Name} (cтор.)";
                            foreach (var consumer in consumers.Where(c => c.ParentId == station.Id))
                            {
                                var consumerItem = CreateSummaryItem(consumer, balItem, true);
                                consumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                                if (consumerItem.Childs.Any() || !ownerId.HasValue)
                                    stationItem.AddChild(consumerItem);
                            }
                            if (stationItem.Childs.Any())
                                networkItem.AddChild(stationItem);
                        }
                        if (networkItem.Childs.Any())
                            Consumers.AddChild(networkItem);
                    }
                    break;
            }
        }

        public void RefreshAuxCostsTree(int? ownerId)
        {
            var balItem = BalanceItem.AuxCosts;
            var enterprise = _data.Enterprises.Single(e => e.Id == UserProfile.Current.Site.Id);

            AuxCosts = CreateSummaryItem(enterprise, balItem, true);
            AuxCosts.Alias = "СТН ВСЕГО:";
            AuxCosts.IsExpanded = true;
            AuxCosts.AddChildList(GetOwnerItemList(enterprise.Id, balItem, ownerId));
        }

        public void RefreshBalanceLossTree(int? ownerId)
        {
            var balItem = BalanceItem.BalanceLoss;
            var enterprise = _data.Enterprises.Single(e => e.Id == UserProfile.Current.Site.Id);

            BalanceLoss = CreateSummaryItem(enterprise, balItem, true);
            BalanceLoss.Alias = "БАЛАНСОВЫЕ ПОТЕРИ ВСЕГО:";
            BalanceLoss.IsExpanded = true;
            BalanceLoss.AddChildList(GetOwnerItemList(enterprise.Id, balItem, ownerId));
        }

        public void RefreshOperConsumersTree(TreeGroupType groupType, int? ownerId)
        {
            var balItem = BalanceItem.OperConsumers;
            OperConsumers = CreateSummaryItem("ПЭН ВСЕГО:", true);
            var consumers = _data.GetOperConsumerList(null);

            switch (groupType)
            {
                case TreeGroupType.None:
                    foreach (var consumer in consumers)
                    {
                        var comsumerItem = CreateSummaryItem(consumer, balItem, true);
                        comsumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                        if (comsumerItem.Childs.Any() || !ownerId.HasValue)
                            OperConsumers.AddChild(comsumerItem);
                    }
                    break;

                case TreeGroupType.BySite:
                    foreach (var site in _data.GetSiteList(balItem, null))
                    {
                        var siteItem = CreateSummaryItem(site, balItem);
                        foreach (var consumer in consumers.Where(c => c.ParentId == site.Id))
                        {
                            var comsumerItem = CreateSummaryItem(consumer, balItem, true);
                            comsumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                            if (comsumerItem.Childs.Any() || !ownerId.HasValue)
                                siteItem.AddChild(comsumerItem);
                        }
                        if (siteItem.Childs.Any())
                            OperConsumers.AddChild(siteItem);
                    }

                    break;

                case TreeGroupType.ByRegion:
                    foreach (var region in _data.Regions)
                    {
                        var regionItem = CreateSummaryItem(region.Name, false);
                        foreach (var consumer in consumers.Where(c => c.RegionId == region.Id))
                        {
                            var comsumerItem = CreateSummaryItem(consumer, balItem, true);
                            comsumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                            if (comsumerItem.Childs.Any() || !ownerId.HasValue)
                                regionItem.AddChild(comsumerItem);
                        }
                        if (regionItem.Childs.Any())
                            OperConsumers.AddChild(regionItem);
                    }
                    break;

                case TreeGroupType.ByRegionAndSite:
                    var sites = _data.GetSiteList(balItem, null);
                    foreach (var region in _data.Regions)
                    {
                        var regionConsumers = consumers.Where(c => c.RegionId == region.Id).ToList();
                        if (!regionConsumers.Any()) continue;

                        var regionItem = CreateSummaryItem(region.Name, false);
                        foreach (var site in sites)
                        {
                            var siteItem = CreateSummaryItem(site, balItem);
                            foreach (var consumer in regionConsumers.Where(s => s.ParentId == site.Id))
                            {
                                var comsumerItem = CreateSummaryItem(consumer, balItem, true);
                                comsumerItem.AddChildList(GetOwnerItemList(consumer.Id, balItem, ownerId));
                                if (comsumerItem.Childs.Any() || !ownerId.HasValue)
                                    siteItem.AddChild(comsumerItem);
                            }
                            if (siteItem.Childs.Any())
                                regionItem.AddChild(siteItem);
                        }

                        if (regionItem.Childs.Any())
                            OperConsumers.AddChild(regionItem);
                    }
                    break;
            }
        }

        public void RefreshGasSupplyTree(TreeGroupType groupType, int? ownerId)
        {
            GasSupply = CreateSummaryItem("ЗАПАС ГАЗА ВСЕГО:", true);
            GasSupply.AddChildList(GetMeasStationTree(BalanceItem.GasSupply, groupType, ownerId));
        }

        public void RefreshTransportTree(int? ownerId)
        {
            var balItem = BalanceItem.Transport;
            var enterprise = _data.Enterprises.Single(e => e.Id == UserProfile.Current.Site.Id);

            Transport = CreateSummaryItem(enterprise, balItem, true);
            Transport.Alias = "ТТР ВСЕГО:";
            Transport.IsExpanded = true;
            Transport.AddChildList(GetOwnerItemList(enterprise.Id, balItem, ownerId));
        }

        private SummaryItem CreateSummaryItem(string name, bool isExpanded)
        {
            switch (_target)
            {
                case Target.Plan:
                    return new PlanSummaryItem(name, _actions) { IsExpanded = isExpanded };

                case Target.Fact:
                    return new FactSummaryItem(name, _actions) { IsExpanded = isExpanded };
            }
            return null;
        }

        private SummaryItem CreateSummaryItem(EntityDTO entity, BalanceItem balItem, bool isInOutPoint = false)
        {
            switch (_target)
            {
                case Target.Plan:
                    return new PlanSummaryItem(entity, balItem, isInOutPoint, _actions);

                case Target.Fact:
                    return new FactSummaryItem(entity, balItem, isInOutPoint, _actions);
            }
            return null;
        }

        

    }
}