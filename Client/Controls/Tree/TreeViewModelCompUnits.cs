using System;
using System.Linq;
using GazRouter.Application;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Microsoft.Practices.Prism;

namespace GazRouter.Controls.Tree
{
    public class TreeViewModelCompUnits : TreeViewModelTree
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

            var siteNodes = data.Sites.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });
            var compStationNodes = data.CompStations.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });
            var compShopNodes = data.CompShops.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });
            var compUnitNodes = data.CompUnits.ToDictionary(dto => dto.Id, dto => new EntityNode(dto) { SortOrder = dto.SortOrder });

            Nodes.AddRange(siteNodes.Values.OrderBy(p => p.Entity.SortOrder));

            BindNodesWithFolder(compStationNodes.Values, siteNodes, "КС", EntityType.CompStation);

            BindNodesWithFolder(compShopNodes.Values, compStationNodes, "КЦ", EntityType.CompShop);
            BindNodesWithFolder(compUnitNodes.Values, compShopNodes, "ГПА", EntityType.CompUnit);
            if (selectId.HasValue)
                SelectNode(selectId.Value);
        }

        
    }
}