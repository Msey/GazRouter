using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.Controls.Tree
{
    public abstract class TreeViewModelTree : TreeViewModelBase
    {
        protected static void BindNodes<T>(IEnumerable<T> nodes, Dictionary<Guid, T> parentNodes) where T : EntityNode
        {
            ProcessNodes(nodes, parentNodes, entityNode => entityNode);
        }

        protected static void BindNodesWithFolder(IEnumerable<EntityNode> nodes, Dictionary<Guid, EntityNode> parentNodes, string folderName, EntityType entityType)
        {
            ProcessNodes(nodes, parentNodes, entityNode => GetFolderNode(folderName, entityType, entityNode));
        }

        protected static void ProcessNodes<T>(IEnumerable<T> nodes, Dictionary<Guid, T> parentNodes, Func<NodeBase, NodeBase> getParentNode) where T:EntityNode
        {
            foreach (var node in nodes.Where(n => n.Parent == null).OrderBy(p => p.Entity.SortOrder))
            {
                T entityNode;
                if (!parentNodes.TryGetValue(((EntityDTO)node.Entity).ParentId.Value, out entityNode)) continue;
                var parentNode = getParentNode(entityNode);
                parentNode.BindNode(node);
            }
        }

        protected static FolderNode GetFolderNode(string name, EntityType entityType, NodeBase parentNode)
        {
            var sameTypeNode = parentNode.Children.OfType<FolderNode>().SingleOrDefault(n => n.EntityType == entityType);

            if (sameTypeNode != null)
                return sameTypeNode;

            var result = new FolderNode(name, entityType);
            parentNode.BindNode(result);

            return result;
        }
    }
}