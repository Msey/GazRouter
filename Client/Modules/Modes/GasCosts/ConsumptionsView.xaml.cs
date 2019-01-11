using System;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Linq;

namespace GazRouter.Modes.GasCosts
{
    public partial class ConsumptionsView
    {
        public ConsumptionsView()
        {
            InitializeComponent();
            MainGrid.ColumnReordered += MainGrid_ColumnReordered;
        }

        private void MainGrid_ColumnReordered(object sender, GridViewColumnEventArgs e)
        {
            (DataContext as ConsumptionViewModelBase)?.OnColumnReordered(MainGrid.Columns);
        }

        private void MainGrid_MouseButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)//для левой кнопки обработчик не отрабатывает
        {
            var vm = DataContext as ConsumptionViewModelBase;
            if (vm == null) return;

            var tlv = (sender as RadTreeListView);
            var v = VisualTreeHelper.FindElementsInHostCoordinates(tlv.GetGlobalMousePosition(e), tlv);

            var item = (v.FirstOrDefault(x => x is DataCellsPresenter) as DataCellsPresenter)?.Item as EntitySummaryRow;
            var col = (v.FirstOrDefault(x => x is GridViewCellBase) as GridViewCellBase)?.Column;

            if (item == null || col == null) return;
            tlv.CurrentCellInfo = new GridViewCellInfo(item, col);
            //vm.OnSelectedCellChanged(item, (col.DisplayIndex == 0 || col.DisplayIndex == 1) ? "0" : col.UniqueName);
        }

        private void MainGrid_CurrentCellChanged(object sender, GridViewCurrentCellChangedEventArgs e)
        {
            if (e.NewCell == null) return;
            var vm = DataContext as ConsumptionViewModelBase;
            if (vm == null) return;

            var tlv = (sender as RadTreeListView);

            var item = e.NewCell.ParentRow.Item as EntitySummaryRow;
            var col = e.NewCell.Column;

            if (item == null || col == null) return;
            vm.OnSelectedCellChanged(item, (col.DisplayIndex == 0 || col.DisplayIndex == 1) ? "0" : col.UniqueName);
        }
    }
}