using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Controls.Tree;

namespace GazRouter.ObjectModel.Model.Pipelines
{
    public partial class PipelineEditableFullTreeView
    {
        private double _oldHeight = 300;

        public PipelineEditableFullTreeView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldModel = e.OldValue as EditableFullTreeViewModel;
            if (oldModel != null)
            {
                oldModel.TreeModel.PropertyChanged -= TreeModelOnPropertyChanged;
            }
            var newModel = e.NewValue as EditableFullTreeViewModel;
            if (newModel != null)
            {
                newModel.TreeModel.PropertyChanged += TreeModelOnPropertyChanged;
            }
        }

        private void TreeModelOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            if (ea.PropertyName != nameof(TreeViewModelBase.SelectedNode) )
            {
                return;
            }
            var treeViewModelPointObjects = sender as ValidatedTreeViewModelPipeline;
            if (treeViewModelPointObjects?.SelectedNode != null)
            {
                var path = treeViewModelPointObjects.SelectedNode.GetPath();
                var item = tree.GetItemByPath(path);
                if (item != null && !item.IsInViewport)
                {
                    tree.BringPathIntoView(path);
                }
            }
        }

        private void ShowPropertiesCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            var box = (CheckBox) sender;
            if (!box.IsChecked.HasValue || !box.IsChecked.Value)
            {
                _oldHeight = LayoutRoot.RowDefinitions[1].Height.Value;
                LayoutRoot.RowDefinitions[1].Height = new GridLength(0);
                LayoutRoot.RowDefinitions[1].MinHeight = 0;
            }
            else
            {
                LayoutRoot.RowDefinitions[1].Height = new GridLength(_oldHeight);
            }
        }
    }
}