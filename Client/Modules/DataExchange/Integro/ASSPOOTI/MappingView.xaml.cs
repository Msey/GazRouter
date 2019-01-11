using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace DataExchange.Integro.ASSPOOTI
{
    public partial class MappingView : UserControl
    {
        public MappingView()
        {
            InitializeComponent();
        }

        private void cboxSystems_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as RadComboBox).SelectedIndex = 0;
        }

    }
}
