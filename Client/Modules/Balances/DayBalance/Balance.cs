using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Balances.Common.TreeGroupType;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.GasCostItemGroups;
using GazRouter.DTO.GasCosts;
using Telerik.Windows.Controls;
using ViewModelBase = GazRouter.Common.ViewModel.ViewModelBase;

namespace GazRouter.Balances.DayBalance
{
    public class Balance : ViewModelBase
    {
        private readonly BalanceData _data;
        private readonly int? _groupId;
        private TreeGroupType _measStationGroupType;
        private TreeGroupType _distrStationGroupType;
        private TreeGroupType _operConsumerGroupType;

        public Balance(BalanceData data, int? groupId)
        {
            _data = data;
            _groupId = groupId;
        }

        

        public TableViewModel Intake { get; set; }
        public TableViewModel Transit { get; set; }
        public TableViewModel Consumers { get; set; }
        public TableViewModel AuxCosts { get; set; }
        public TableViewModel OperConsumers { get; set; }

        public ItemBase PipeMinus { get; set; }
        public ItemBase PipePlus { get; set; }
        public ItemBase GasSupply { get; set; }
        public ItemBase BalanceLoss { get; set; }

        public TableViewModel Summary { get; set; }

        public void Build()
        {
            if (Intake == null || Transit == null || BalanceGroupTypes.MeasStationGroupType != _measStationGroupType)
                BuildIntakeTransitTree();

            if (OperConsumers == null || BalanceGroupTypes.OperConsumerGroupType != _operConsumerGroupType) BuildOperConsumerTree();

            if (Consumers == null || BalanceGroupTypes.DistrStationGroupType != _distrStationGroupType)
                BuildConsumerTree();

            if (AuxCosts == null) BuildAuxCostsTree();

            if (GasSupply == null) BuildGasSupply();

            if (BalanceLoss == null) BuildBalanceLoss();

            BuildSummary();

            _measStationGroupType = BalanceGroupTypes.MeasStationGroupType;
            _distrStationGroupType = BalanceGroupTypes.DistrStationGroupType;
            _operConsumerGroupType = BalanceGroupTypes.OperConsumerGroupType;
        }

        private string GroupName
            => _data.DataDto.BalanceGroups.SingleOrDefault(g => g.Id == _groupId)?.Name ?? _data.SystemDto.Name;

        private void BuildIntakeTransitTree()
        {
            // ПОСТУПЛЕНИЕ
            Intake = new TableViewModel("Поступление", _data.Day);
            var intakeRoot = new SummaryItem(GroupName, _data.Day) { IsBold = true };
            intakeRoot.Childs.AddRange(BuildMeasStationTree(BalanceGroupTypes.MeasStationGroupType, Sign.In));
            Intake.Childs.Add(intakeRoot);

            // ТРАНЗИТ
            Transit = new TableViewModel("Транзит", _data.Day);
            var transitRoot = new SummaryItem(GroupName, _data.Day) { IsBold = true };
            transitRoot.Childs.AddRange(BuildMeasStationTree(BalanceGroupTypes.MeasStationGroupType, Sign.Out));
            Transit.Childs.Add(transitRoot);
        }

        private List<ItemBase> BuildMeasStationTree(TreeGroupType groupType, Sign sign)
        {
            var tree = new List<ItemBase>();
            var balItem = sign == Sign.In ? BalanceItem.Intake : BalanceItem.Transit;
            
            var stationItems = new List<MeasStationItem>();
            foreach (var station in _data.GetMeasStationList(sign, _groupId))
            {
                var stationItem = new MeasStationItem(station, _data.Day) { BalItem = balItem };
                _data.GetFactValue(stationItem);
                _data.GetPlanValue(stationItem);

                var lines = _data.GetMeasLineList(station);
                if (lines.Count > 1)
                {
                    foreach (var line in lines)
                    {
                        var lineItem = new ItemBase(line, _data.Day) { BalItem = balItem };
                        _data.GetFactValue(lineItem);
                        stationItem.AddChild(lineItem);
                    }
                }
                stationItems.Add(stationItem);
            }

            switch (groupType)
            {
                case TreeGroupType.None:
                    tree.AddRange(stationItems);
                    break;

                case TreeGroupType.BySite:
                    foreach (var site in _data.GetSiteList(balItem))
                    {
                        var siteItem = new SummaryItem(site, _data.Day) { BalItem = balItem };
                        siteItem.Childs.AddRange(stationItems.Where(s => s.Entity.ParentId == site.Id));
                        if (siteItem.Childs.Count > 0)
                            tree.Add(siteItem);
                    }
                    break;

                case TreeGroupType.ByEnterprise:
                    foreach (var e in _data.GetEnterpriseList(balItem))
                    {
                        var entItem = new SummaryItem(e, _data.Day) { BalItem = balItem };
                        entItem.Childs.AddRange(stationItems.Where(s => s.Entity.NeighbourEnterpriseId == e.Id));
                        if (entItem.Childs.Count > 0)
                            tree.Add(entItem);
                    }

                    // Для ГИСов, у которых не задано смежное предприятие
                    var noEnterpriseItem = new SummaryItem("<Предприятие не задано>", _data.Day);
                    noEnterpriseItem.Childs.AddRange(stationItems.Where(s => !s.Entity.NeighbourEnterpriseId.HasValue));
                    if (noEnterpriseItem.Childs.Count > 0)
                        tree.Add(noEnterpriseItem);

                    break;
            }

            return tree;
        }

        private List<OperConsumerItem> _operConsumers; 
        private void BuildOperConsumerTree()
        {
            var balItem = BalanceItem.OperConsumers;
            _operConsumers = new List<OperConsumerItem>();
            OperConsumers = new TableViewModel("ПЭН", _data.Day);
            var root = new SummaryItem(GroupName, _data.Day) { IsBold = true };
            OperConsumers.Childs.Add(root);

            var consumers = _data.GetOperConsumerList(_groupId);

            foreach (var consumer in consumers)
            {
                var consumerItem = new OperConsumerItem(consumer, _data.Day);
                _data.GetFactValue(consumerItem);
                _data.GetPlanValue(consumerItem);
                _operConsumers.Add(consumerItem);
            }
            

            switch (BalanceGroupTypes.OperConsumerGroupType)
            {
                case TreeGroupType.None:
                    root.Childs.AddRange(_operConsumers);
                    break;

                case TreeGroupType.BySite:
                    foreach (var site in _data.GetSiteList(balItem))
                    {
                        var siteItem = new SummaryItem(site, _data.Day) { BalItem = balItem };
                        siteItem.Childs.AddRange(_operConsumers.Where(s => s.Entity.ParentId == site.Id));
                        if (siteItem.Childs.Count > 0)
                            root.Childs.Add(siteItem);
                    }
                    break;

                case TreeGroupType.ByRegion:
                    {
                        var regions =
                            ClientCache.DictionaryRepository.Regions.Where(
                                r => _operConsumers.Select(ds => ds.Entity.RegionId).Distinct().Contains(r.Id));
                        foreach (var region in regions)
                        {
                            var regionItem = new SummaryItem(region.Name, _data.Day);
                            regionItem.Childs.AddRange(_operConsumers.Where(s => s.Entity.RegionId == region.Id));
                            root.Childs.Add(regionItem);
                        }
                        break;
                    }


                case TreeGroupType.ByRegionAndSite:
                    {
                        var regions =
                            ClientCache.DictionaryRepository.Regions.Where(
                                r => _operConsumers.Select(ds => ds.Entity.RegionId).Distinct().Contains(r.Id));
                        foreach (var region in regions)
                        {
                            var regionItem = new SummaryItem(region.Name, _data.Day);

                            foreach (var site in _data.GetSiteList(balItem))
                            {
                                var siteItem = new SummaryItem(site, _data.Day) { BalItem = balItem };
                                siteItem.Childs.AddRange(
                                    _operConsumers.Where(s => s.Entity.ParentId == site.Id && s.Entity.RegionId == region.Id));
                                if (siteItem.Childs.Count > 0)
                                    regionItem.Childs.Add(siteItem);
                            }
                            root.Childs.Add(regionItem);
                        }
                        break;
                    }
            }
        } 

        private void BuildConsumerTree()
        {
            Consumers = new TableViewModel("Потребители", _data.Day);
            var root = new SummaryItem(GroupName, _data.Day) { IsBold = true };
            Consumers.Childs.Add(root);

            var stationItemList = new List<DistrStationItem>();
            foreach (var station in _data.GetDistrStationList(_groupId))
            {
                var stationItem = new DistrStationItem(station, _data.Day);
                _data.GetFactValue(stationItem);
                _data.GetPlanValue(stationItem);
                
                if (_operConsumers != null)
                    stationItem.Recalc(_operConsumers.Where(c => c.Entity.DistrStationId == station.Id).ToList());

                var outlets = _data.GetDistrStationOutletList(station.Id);
                if (outlets.Count > 1)
                {
                    foreach (var outlet in outlets)
                    {
                        var outletItem = new ItemBase(outlet, _data.Day) { BalItem = BalanceItem.Consumers };
                        _data.GetFactValue(outletItem);
                        stationItem.AddChild(outletItem);
                    }
                }
                stationItemList.Add(stationItem);
            }


            switch (BalanceGroupTypes.DistrStationGroupType)
            {
                case TreeGroupType.None:
                    root.Childs.AddRange(stationItemList);
                    break;

                case TreeGroupType.BySite:
                    foreach (var site in _data.GetSiteList(BalanceItem.Consumers))
                    {
                        var siteItem = new SummaryItem(site, _data.Day) { BalItem = BalanceItem.Consumers };
                        siteItem.Childs.AddRange(stationItemList.Where(s => s.Entity.ParentId == site.Id));
                        if (siteItem.Childs.Count > 0)
                            root.Childs.Add(siteItem);
                    }
                    break;

                case TreeGroupType.ByRegion:
                    {
                        var regions =
                            ClientCache.DictionaryRepository.Regions.Where(
                                r => stationItemList.Select(ds => ds.Entity.RegionId).Distinct().Contains(r.Id));
                        foreach (var region in regions)
                        {
                            var regionItem = new SummaryItem(region.Name, _data.Day);
                            regionItem.Childs.AddRange(stationItemList.Where(s => s.Entity.RegionId == region.Id));
                            root.Childs.Add(regionItem);
                        }
                        break;
                    }


                case TreeGroupType.ByRegionAndSite:
                    {
                        var regions =
                            ClientCache.DictionaryRepository.Regions.Where(
                                r => stationItemList.Select(ds => ds.Entity.RegionId).Distinct().Contains(r.Id));
                        foreach (var region in regions)
                        {
                            var regionItem = new SummaryItem(region.Name, _data.Day);

                            foreach (var site in _data.GetSiteList(BalanceItem.Consumers))
                            {
                                var siteItem = new SummaryItem(site, _data.Day) { BalItem = BalanceItem.Consumers };
                                siteItem.Childs.AddRange(
                                    stationItemList.Where(s => s.Entity.ParentId == site.Id && s.Entity.RegionId == region.Id));
                                if (siteItem.Childs.Count > 0)
                                    regionItem.Childs.Add(siteItem);
                            }
                            root.Childs.Add(regionItem);
                        }
                        break;
                    }
            }
        }

        private void BuildAuxCostsTree()
        {
            AuxCosts = new TableViewModel("СТН", _data.Day);
            var root = new SummaryItem(GroupName, _data.Day) { IsBold = true };
            AuxCosts.Childs.Add(root);

            var costs = _data.GetGasCosts(_groupId);

            foreach (var site in _data.GetSiteList(BalanceItem.AuxCosts))
            {
                var siteCosts = costs.Where(s => s.SiteId == site.Id).ToList();
                var siteItem = new SummaryItem(site, _data.Day) { BalItem = BalanceItem.Consumers }; 
                foreach (var itemGroup in ClientCache.DictionaryRepository.GasCostItemGroups)
                {
                    var auxGroup = new ItemBase(itemGroup.Name, _data.Day);
                    GetGasCostValue(auxGroup, siteCosts.Where(c => c.ItemGroup == itemGroup.ItemGroup).ToList());
                    siteItem.Childs.Add(auxGroup);
                }
                //_data.GetPlanValue(siteItem);
                root.Childs.Add(siteItem);
            }
        }

        private void GetGasCostValue(ItemBase item, List<GasCostDTO> costs)
        {
            item.Current = costs.Where(gc => gc.Date == item.Day).Sum(gc => gc.Volume);
            item.Prev = costs.Where(gc => gc.Date == item.Day.AddDays(-1).Date).Sum(gc => gc.Volume);
            item.MonthTotal = costs.Where(gc => gc.Date < item.Day && gc.Date.Month == item.Day.Month).Sum(gc => gc.Volume);
        }

        private void BuildGasSupply()
        {
            var aggr = _data.GetGasSupply(_groupId);
            GasSupply = new ItemBase("Запас газа", _data.Day) {Entity = aggr};
            if (aggr != null)
            {
                _data.GetFactValue(GasSupply);
                GasSupply.MonthTotal = null;
            }


            PipeMinus = new ItemBase("Труба (-)", _data.Day);
            if (GasSupply.Delta < 0) PipeMinus.Current = -1 * GasSupply.Delta;

            PipePlus = new ItemBase("Труба (+)", _data.Day);
            if (GasSupply.Delta > 0) PipePlus.Current = GasSupply.Delta;
        }


        private void BuildBalanceLoss()
        {
            var aggr = _data.GetBalanceLoss(_groupId);
            BalanceLoss = new ItemBase("Балансовые потери", _data.Day) { Entity = aggr };
            if (aggr != null)
                _data.GetFactValue(BalanceLoss);
        }


        private void BuildSummary()
        {
            Summary = new TableViewModel("Сводка", _data.Day) {AutoExpand = true};
            var root = new ItemBase(GroupName, _data.Day) { IsBold = true };
            Summary.Childs.Add(root);

            // Если группа пустая, то Summary не формировать.
            if (Intake.Childs.Count == 0 && Transit.Childs.Count == 0 && Consumers.Childs.Count == 0 &&
                AuxCosts.Childs.Count == 0 && OperConsumers.Childs.Count == 0) return;
            


            var resources = new SummaryItem("РЕСУРСЫ", _data.Day);
            root.Childs.Add(resources);

            var distr = new TableViewModel("РАСПРЕДЕЛЕНИЕ", _data.Day);
            root.Childs.Add(distr);


            resources.Childs.Add(PipeMinus);

            var intake = new ItemBase("Поступление", _data.Day);
            Intake.CopyValues(intake);
            resources.Childs.Add(intake);

            
            distr.Childs.Add(PipePlus);

            var transit = new ItemBase("Транзит", _data.Day);
            Transit.CopyValues(transit);
            distr.Childs.Add(transit);

            var consumers = new ItemBase("Потребители", _data.Day);
            Consumers.CopyValues(consumers);
            distr.Childs.Add(consumers);


            var auxAndLoss = new SummaryItem("СТН и балансовые потери", _data.Day);
            distr.Childs.Add(auxAndLoss);

            var aux = new SummaryItem("СТН", _data.Day);
            auxAndLoss.Childs.Add(aux);

            var costs = _data.GetGasCosts(_groupId);
            foreach (var itemGroup in ClientCache.DictionaryRepository.GasCostItemGroups)
            {
                var auxGroup = new ItemBase(itemGroup.Name, _data.Day);
                GetGasCostValue(auxGroup, costs.Where(c => c.ItemGroup == itemGroup.ItemGroup).ToList());
                aux.Childs.Add(auxGroup);
            }
            

            //var auxCosts = new ItemBase("СТН", _data.Day);
            //AuxCosts.CopyValues(auxCosts);
            //auxAndLoss.Childs.Add(auxCosts);

            auxAndLoss.Childs.Add(BalanceLoss);


            var operConsumers = new ItemBase("ПЭН", _data.Day);
            OperConsumers.CopyValues(operConsumers);
            distr.Childs.Add(operConsumers);


            root.Childs.Add(GasSupply);

            root.Current = resources.Current - distr.Current;
            root.Prev = resources.Prev - distr.Prev;
            //Summary.MonthTotal = resources.MonthTotal - distr.MonthTotal;
        }
    }



}