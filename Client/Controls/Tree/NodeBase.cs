using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.Controls.Tree
{
    public abstract class NodeBase : PropertyChangedBase
    {
        private ImageSource _imageSource;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isBold;

        private string _textTooltip;

        protected NodeBase()
        {
            Children = new ObservableCollection<NodeBase>();
        }

        public abstract string Name { get; }

        public NodeBase Parent { get; private set; }
        public ObservableCollection<NodeBase> Children { get; private set; }

        public string GetPath()
        {
            string path = this.Name;
            NodeBase nextParent = this.Parent;

            while (nextParent != null)
            {
                path = nextParent.Name + @"@" + path;
                nextParent = nextParent.Parent;
            }

            return path;
        }

        public virtual ImageSource ImageSource
        {
            get
            {
                return _imageSource ??
                       (_imageSource =
                           (ImageSource)
                               new ImageSourceConverter().ConvertFromString(
                                   @"/Common;component/Images/16x16/object2.png"));
            }
        }

        public string TextTooltip
        {
            get { return _textTooltip; }
            set { SetProperty(ref _textTooltip, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetProperty(ref _isExpanded, value); }
        }

        public bool IsBold
        {
            get { return _isBold; }
            set { SetProperty(ref _isBold, value); }
        }

        public virtual bool HasComment => false;

        public abstract CommonEntityDTO Entity { get; }

        public IEnumerable<NodeBase> Ancestors
        {
            get
            {
                NodeBase node = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    yield return node;
                }
            }
        }

        public void BindNode(NodeBase childNode)
        {
            childNode.Parent = this;
            Children.Add(childNode);
        }

        public virtual string ToolTipType { get { return string.Empty; } }

        public virtual bool IsVirtual => false;
    }
}