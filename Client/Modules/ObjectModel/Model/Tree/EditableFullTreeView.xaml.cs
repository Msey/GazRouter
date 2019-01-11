using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Controls.Tree;

namespace GazRouter.ObjectModel.Model.Tree
{
    public partial class EditableFullTreeView
    {
        private double _oldHeight = 300;

        public EditableFullTreeView()
        {
            InitializeComponent();

            DataContextChanged += EditableFullTreeView_DataContextChanged;
        }

        private void EditableFullTreeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = e.OldValue as EditableFullTreeViewModel;
            if (viewModel != null)
            {
                viewModel.TreeModel.PropertyChanged -= TreeModelOnPropertyChanged;
            }
             viewModel = e.NewValue as EditableFullTreeViewModel;
            if (viewModel != null)
            {
                viewModel.TreeModel.PropertyChanged += TreeModelOnPropertyChanged;
            }
        }

        private void TreeModelOnPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            if (ea.PropertyName != "SelectedNode")
            {
                return;
            }
            var treeViewModelPointObjects = sender as TreeViewModelPointObjects;
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
                _oldHeight = TreeGrid.RowDefinitions[1].Height.Value;
                TreeGrid.RowDefinitions[1].Height = new GridLength(0);
                TreeGrid.RowDefinitions[1].MinHeight = 0;
            }
            else
            {
                TreeGrid.RowDefinitions[1].Height = new GridLength(_oldHeight);
            }
        }
    }
}