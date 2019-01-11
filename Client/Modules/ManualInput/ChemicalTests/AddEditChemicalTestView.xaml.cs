namespace GazRouter.ManualInput.ChemicalTests
{
    public partial class AddEditChemicalTestView
    {
        public AddEditChemicalTestView()
        {
            InitializeComponent();
        }

        private void RadMaskedNumericInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var v = (sender as Telerik.Windows.Controls.RadMaskedNumericInput);
            if (v.Value == null && e.PlatformKeyCode == 189) v.Value = -0;
        }

    }
}
