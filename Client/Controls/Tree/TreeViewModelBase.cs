using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GazRouter.Common.ViewModel;

namespace GazRouter.Controls.Tree
{
    public abstract class TreeViewModelBase : AsyncViewModelBase
    {
        protected readonly static Thickness _margin = new Thickness(0, 0, 5, 0);

        private NodeBase _selectedNode;

        protected TreeViewModelBase()
        {
            Nodes = new ObservableCollection<NodeBase>();
        }


        public NodeBase SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (!SetProperty(ref _selectedNode, value)) return;

                if (_selectedNode == null) return;
                
                _selectedNode.IsSelected = true;
                _selectedNode.IsExpanded = true;
            }
        }

        private ObservableCollection<NodeBase> _nodes;
        public ObservableCollection<NodeBase> Nodes
        {
            get { return _nodes; } 
            set { SetProperty(ref _nodes, value); }
        }

        private IEnumerable<NodeBase> GetAllNodes()
        {
            var stack = new Stack<NodeBase>();

            foreach (var item in Nodes)
            {
                stack.Push(item);
            }

            while (stack.Count > 0)
            {
                var result = stack.Pop();
                yield return result;
                foreach (var node in result.Children)
                    stack.Push(node);
            }
        }

        public bool SelectNode(Guid guid)
        {
            var allNodes = GetAllNodes().OfType<EntityNode>().ToList();
            var selectedNode = allNodes.SingleOrDefault(n => n.Entity.Id == guid);

            foreach (var node in allNodes)
            {
                node.IsSelected = selectedNode == node;
            }
            SelectedNode = selectedNode;
            if (selectedNode == null) return false;

            foreach (var ancestor in selectedNode.Ancestors)
            {
                ancestor.IsExpanded = true;
            }
           
	        return true;
        }

    
    }
}