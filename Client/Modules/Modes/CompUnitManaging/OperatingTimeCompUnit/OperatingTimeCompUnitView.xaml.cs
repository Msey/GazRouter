
using Telerik.Windows.Controls;

namespace GazRouter.Modes.CompressorUnitManaging.OperatingTimeCompUnit
{
    public partial class OperatingTimeCompUnitView
    {
        public OperatingTimeCompUnitView()
        {
            InitializeComponent();
        }

		private void DataControl_OnSelectionChanged(object sender, SelectionChangeEventArgs e)
		{
			if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0 && e.AddedItems[0] == e.RemovedItems[0])
				return;
			if (e.AddedItems.Count > 0)
				((RadTreeListView)e.Source).ExpandHierarchyItem(e.AddedItems[0]);
		}
    }
}
