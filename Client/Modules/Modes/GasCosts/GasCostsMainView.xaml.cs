namespace GazRouter.Modes.GasCosts
{
    public partial class GasCostsMainView
    {
        public GasCostsMainView()
        {
            InitializeComponent();
        }

        private void PipeLinesTreeFilterChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as GasCostsMainViewModel).LoadTrees();
        }
    }
}
