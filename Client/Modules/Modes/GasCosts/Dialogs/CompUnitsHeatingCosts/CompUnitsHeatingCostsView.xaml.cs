using Telerik.Windows.Controls;
namespace GazRouter.Modes.GasCosts.Dialogs.CompUnitsHeatingCosts
{
    public partial class CompUnitsHeatingCostsView
    {
        public CompUnitsHeatingCostsView()
        {
            InitializeComponent();
        }
        private void GridViewDataControl_OnCellValidated(object sender, GridViewCellValidatedEventArgs e)
        {
            var v = ((CompUnitsHeatingCostsViewModel)this.DataContext);                        
            // var v = ((CompUnitsHeatingCostsViewModel) this.DataContext).Input1.InputValue;
            e.ValidationResult.ErrorMessage = v.Errors[v.Errors.Count - 1];
            
        }
        private void GridViewDataControl_OnCellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            var v = (CompUnitsHeatingCostsViewModel)DataContext;
            if (!v.HasErrors) return;
            //
            
            e.IsValid = false;
            e.ErrorMessage = v.Errors[v.Errors.Count - 1];
        }
    }
}
