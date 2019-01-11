using System.Linq;
using Telerik.Windows.Controls.GridView;

namespace GazRouter.Repair.Agreement
{
    public partial class UserAgreementsListView
    {
        public UserAgreementsListView()
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
