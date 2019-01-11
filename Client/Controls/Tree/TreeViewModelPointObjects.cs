using System;
using System.Linq;
using GazRouter.Application;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Microsoft.Practices.Prism;

namespace GazRouter.Controls.Tree
{
    public class TreeViewModelPointObjects : TreeViewModelTree
    {
        public void FillTree(TreeData data, bool isDeleteCommand = false, Guid? newEntityId = null) 
		{
			Guid? selectId = null;
			if (SelectedNode != null)
			{
                if (newEntityId.HasValue)
                {
                    selectId = newEntityId;
                }
                else
                {
                    if (isDeleteCommand)
                    {
                        SelectedNode = SelectedNode.Parent;
                    }
                    var entityNode = SelectedNode is FolderNode ? SelectedNode.Parent : SelectedNode;
                    selectId = entityNode.Entity.Id;
                }
			}
			Nodes.Clear();

			var enterpriseNodes = data.Enterprises?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder, IsBold = dto.Id == Settings.EnterpriseId });//1
            var siteNodes = data.Sites?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//2
            var compStationNodes = data.CompStations?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//3
            var compShopNodes = data.CompShops?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//4
            var compUnitNodes = data.CompUnits?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//5
            var distrStationNodes = data.DistrStations?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//6
            var distrStationOutletNodes = data.DistrStationOutlets?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//7
            var measStationNodes = data.MeasStations?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//8
            var measLineNodes = data.MeasLines?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//9
            var reducingStationNodes = data.ReducingStations?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//10
            var measPointNodes = data.MeasPoints?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//11
            var coolingStations = data.CoolingStations?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//12
			var coolingUnits = data.CoolingUnits?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//13
            var powerPlants = data.PowerPlants?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//14
            var powerUnits = data.PowerUnits?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//15
            var boilers = data.Boilers?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//16
            var boilerPlants = data.BoilerPlants?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });//17
            var operConsumers = data.OperConsumers?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) {SortOrder = dto.SortOrder});
            var consumers = data.Consumers?.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) {SortOrder = dto.SortOrder});
            //
            if (siteNodes != null && enterpriseNodes != null) BindNodes(siteNodes.Values, enterpriseNodes);
            if (compStationNodes != null && siteNodes != null) BindNodesWithFolder(compStationNodes.Values, siteNodes, "КС", EntityType.CompStation);
            if (distrStationNodes != null && siteNodes != null) BindNodesWithFolder(distrStationNodes.Values, siteNodes, "ГРС", EntityType.DistrStation);
            if (measStationNodes != null && siteNodes != null) BindNodesWithFolder(measStationNodes.Values, siteNodes, "ГИС", EntityType.MeasStation);
            if (reducingStationNodes != null && siteNodes != null) BindNodesWithFolder(reducingStationNodes.Values, siteNodes, "ПРГ", EntityType.ReducingStation);
            if (operConsumers != null && siteNodes != null) BindNodesWithFolder(operConsumers.Values, siteNodes, "ПЭН", EntityType.OperConsumer);

            if (compShopNodes != null && compStationNodes != null) BindNodesWithFolder(compShopNodes.Values, compStationNodes, "КЦ", EntityType.CompShop);
            if (boilerPlants != null && compStationNodes != null) BindNodesWithFolder(boilerPlants.Values, compStationNodes, "Котельные", EntityType.BoilerPlant);
            if (powerPlants != null && compStationNodes != null) BindNodesWithFolder(powerPlants.Values, compStationNodes, "ЭСН", EntityType.PowerPlant);
            if (coolingStations != null && compStationNodes != null) BindNodesWithFolder(coolingStations.Values, compStationNodes, "СОГ", EntityType.CoolingStation);
            if (compUnitNodes != null && compShopNodes != null) BindNodesWithFolder(compUnitNodes.Values, compShopNodes, "ГПА", EntityType.CompUnit);
            if (measPointNodes != null && compShopNodes != null) BindNodes(measPointNodes.Values, compShopNodes);
            if (powerUnits != null && powerPlants != null) BindNodes(powerUnits.Values, powerPlants);
            if (boilers != null && boilerPlants != null) BindNodes(boilers.Values, boilerPlants);
            if (coolingUnits != null && coolingStations != null) BindNodes(coolingUnits.Values, coolingStations);

            if (consumers != null && distrStationNodes != null) BindNodesWithFolder(consumers.Values, distrStationNodes, "Подключения", EntityType.Consumer);

            if (boilers != null && distrStationNodes != null) BindNodesWithFolder(boilers.Values, distrStationNodes, "Котлы", EntityType.Boiler);
            if (distrStationOutletNodes != null && distrStationNodes != null) BindNodesWithFolder(distrStationOutletNodes.Values, distrStationNodes, "Выходы", EntityType.DistrStationOutlet);
            if (measPointNodes != null && distrStationNodes != null) BindNodes(measPointNodes.Values, distrStationNodes);
            if (boilers != null && measStationNodes != null) BindNodesWithFolder(boilers.Values, measStationNodes, "Котлы", EntityType.Boiler);
            if (measLineNodes != null && measStationNodes != null) BindNodes(measLineNodes.Values, measStationNodes);
            if (measPointNodes != null && measLineNodes != null) BindNodes(measPointNodes.Values, measLineNodes);
            if (UserProfile.Current.Site.IsEnterprise)
                Nodes.AddRange(enterpriseNodes.Values.OrderBy(p => p.Entity.SortOrder));
             else if (siteNodes.Count > 0 && siteNodes.ContainsKey(UserProfile.Current.Site.Id))
                Nodes.AddRange(siteNodes[UserProfile.Current.Site.Id].Children);
            if (selectId.HasValue)
				SelectNode(selectId.Value);
		}
    }
}
