using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace GazRouter.Modes.GasCosts
{
    public class CostsCellStyleSelector : StyleSelector
    {
        private readonly GasCostsMainViewModel _mainViewModel;
        public Style Coststyle { get; }
        public Style NormErrorCostStyle { get; }
        public Style PlanErrorCostStyle { get; }
        public Style SummaryStyle { get; }
        public Style SummaryNormErrorStyle { get; }
        public Style SummaryPlanErrorStyle { get; }
        public Style HiddenStyle { get; }

        public CostsCellStyleSelector(GasCostsMainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            var cellStyle = System.Windows.Application.Current.Resources["GridViewCellStyle"] as Style;

            Coststyle = new Style(typeof(GridViewCell)) { BasedOn = cellStyle };
            Coststyle.Setters.Add(new Setter(Control.FontStyleProperty, FontStyles.Italic));
         

            SummaryStyle = new Style(typeof (GridViewCell)) {BasedOn = cellStyle};
           SummaryStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Bold));

            NormErrorCostStyle = new Style(typeof (GridViewCell)) {BasedOn = cellStyle};
            NormErrorCostStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromArgb(0xff, 0xdc, 0x14, 0x3c))));
            NormErrorCostStyle.Setters.Add(new Setter(Control.FontStyleProperty, FontStyles.Italic));


            PlanErrorCostStyle = new Style(typeof(GridViewCell)) { BasedOn = cellStyle };
            PlanErrorCostStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x8c, 0x00))));
            PlanErrorCostStyle.Setters.Add(new Setter(Control.FontStyleProperty, FontStyles.Italic));


            SummaryNormErrorStyle = new Style(typeof(GridViewCell)) { BasedOn = cellStyle };
            SummaryNormErrorStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromArgb(0xff, 0xdc, 0x14, 0x3c))));

            SummaryPlanErrorStyle = new Style(typeof(GridViewCell)) { BasedOn = cellStyle };
            SummaryPlanErrorStyle.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x8c, 0x00))));

            HiddenStyle = new Style(typeof (GridViewCell)) {BasedOn = cellStyle};
            HiddenStyle.Setters.Add(new Setter(UIElement.VisibilityProperty, Visibility.Collapsed));
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var summaryItem = item as EntitySummaryRow;
            var cell = (GridViewCell) container;
            CostType costType;
            if (!Enum.TryParse(cell.Column.UniqueName, out costType))
                return null;
            if (summaryItem == null)
                return null;

      /*      if (!summaryItem.CanHasValue(costType))
            {
                return HiddenStyle;
            }*/

            bool normError = false;
            bool planError = false;

            var costCell = summaryItem.GetCell(costType);
            switch (_mainViewModel.SelectedTarget.Target)
            {
                case Target.Plan:
                    normError = costCell[Target.Plan] > costCell[Target.Norm];
                    break;
                case
                    Target.Fact:
                    normError = costCell[Target.Fact] > costCell[Target.Norm];
                    planError = costCell[Target.Fact] > costCell[Target.Plan];
                    break;
            }

            if (normError)
                return costCell.IsEditable ? NormErrorCostStyle : SummaryNormErrorStyle;
            if (planError)
                return costCell.IsEditable ? PlanErrorCostStyle : SummaryPlanErrorStyle;

            return costCell.IsEditable ? Coststyle : SummaryStyle;
        }
    }
}