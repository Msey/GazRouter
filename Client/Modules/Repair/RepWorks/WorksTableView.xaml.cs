using System.Linq;
using Telerik.Windows.Controls.GridView;

namespace GazRouter.Repair.RepWorks
{
    public partial class WorksTableView
    {
        public WorksTableView()
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
