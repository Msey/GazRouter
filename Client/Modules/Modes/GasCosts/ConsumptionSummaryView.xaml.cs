using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Markup;

namespace GazRouter.Modes.GasCosts
{
    public partial class ConsumptionSummaryView
    {
        public ConsumptionSummaryView()
        {
            InitializeComponent();


            TreeListView.DataLoaded += TreeListViewOnDataLoaded;
            TreeListViewDuplicate.DataLoaded += TreeListViewOnDataLoaded;
        }

        private void TreeListViewOnDataLoaded(object sender, EventArgs eventArgs)
        {
            TreeListView.DataLoaded -= TreeListViewOnDataLoaded;
            TreeListView.ExpandAllHierarchyItems();
            TreeListViewDuplicate.DataLoaded -= TreeListViewOnDataLoaded;
            TreeListViewDuplicate.ExpandAllHierarchyItems();
        }
    }
}
