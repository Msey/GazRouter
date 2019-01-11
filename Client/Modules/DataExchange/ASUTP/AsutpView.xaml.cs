namespace GazRouter.DataExchange.ASUTP
{
    public partial class AsutpView
    {
        public AsutpView()
        {
            InitializeComponent();
            treeListView.DataLoaded += TreeListView_DataLoaded;
            treeListView.SelectionChanged += TreeListView_SelectionChanged;
        }

        private void TreeListView_DataLoaded(object sender, System.EventArgs e)
        {
            AsutpViewModel vm = DataContext as AsutpViewModel;
            if (vm.SpecMode)
            {
                if (vm.Reload)
                    vm.FindSpecItemInTree();
            }
        }
        
        private void TreeListView_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            AsutpViewModel vm = DataContext as AsutpViewModel;
            if (vm.SpecMode)
            {
                if (vm.SpecItem != null)
                {
                    if (vm.SpecHierarchy != null)
                    {
                        foreach (ItemBase ancestor in vm.SpecHierarchy)
                            treeListView.ExpandHierarchyItem(ancestor);
                    }
                }
                vm.SpecMode = false;
            }
        }
    }
}