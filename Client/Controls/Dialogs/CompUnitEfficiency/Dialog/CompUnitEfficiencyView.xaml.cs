namespace GazRouter.Controls.Dialogs.CompUnitEfficiency.Dialog
{
    public partial class CompUnitEfficiencyView
    {
        public CompUnitEfficiencyView()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                var dc = args.NewValue as CompUnitEfficiencyViewModel;
                if (dc != null)
                {
                    dc.MainChartControl = ChartControl;
                    dc.CompRatioChartControl = CompRatioChartControl;
                    dc.KpdChartControl = KpdChartControl;
                    dc.CompUnitPowerChartControl = CompUnitPowerChartControl;
                    dc.PowerFuelGasConsimptionChartControl = PowerFuelGasConsimptionChartControl;
                }
            };
        }
    }
}