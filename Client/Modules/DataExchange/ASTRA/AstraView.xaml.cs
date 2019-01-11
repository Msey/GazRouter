namespace GazRouter.DataExchange.ASTRA
{
    public partial class AstraView
    {
        public AstraView()
        {
            InitializeComponent();
            treeListView.SelectionChanged += TreeListView_SelectionChanged;
            treeListView.DataLoaded += TreeListView_DataLoaded;
        }

        private void TreeListView_DataLoaded(object sender, System.EventArgs e)
        {
            AstraViewModel vm = DataContext as AstraViewModel;
            if (vm.SpecMode)
                vm.IsSpecItemInTree();
        }

        private void TreeListView_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            AstraViewModel vm = DataContext as AstraViewModel;
            if (vm.SpecMode)
            {
                if (vm.SpecItem != null)
                {
                    if (vm.SpecHierarchy != null)
                        foreach (ItemBase ancestor in vm.SpecHierarchy)
                            treeListView.ExpandHierarchyItem(ancestor);
                }
                vm.SpecMode = false;
            }
        }
    }
}
