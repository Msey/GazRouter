using System.Linq;
using Telerik.Windows.Controls.GridView;

namespace GazRouter.Repair.Plan
{
    public partial class PlanTableView
    {
        public PlanTableView()
        {
            InitializeComponent();
        }

        private void Grid_OnFiltered(object sender, GridViewFilteredEventArgs e)
        {
            //var repairs = Grid.Items.Cast<Repair>().ToList();
            //((RepairMainViewModel) DataContext).RefreshGantt(repairs);
        }
    }
}
