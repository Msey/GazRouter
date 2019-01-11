using System.Windows;
using GazRouter.Application.Helpers;

namespace GazRouter.Controls.Dialogs.PeriodPickerEx
{
    public partial class PeriodPickerEx
    {
        private PeriodPickerDropViewModel _dropVm;
        public PeriodPickerEx()
        {
            InitializeComponent();

            _dropVm = new PeriodPickerDropViewModel();
            DropView.DataContext = _dropVm;
            _dropVm.PropertyChanged += (obj, args) =>
            {
                if (args.PropertyName == "DisplayString")
                    Txt.Text = _dropVm.DisplayString;
            };
        }

        public Period SelectedPeriod
        {
            get { return (Period)GetValue(SelectedPeriodProperty); }
            set { SetValue(SelectedPeriodProperty, value); }
        }
        
        public static readonly DependencyProperty SelectedPeriodProperty =
            DependencyProperty.Register("SelectedPeriodProperty", typeof(Period), typeof(PeriodPickerEx),
                                new PropertyMetadata(OnSelectedPeriodChanged));

        private static void OnSelectedPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PeriodPickerEx)d;
            control.Txt.Text = (e.NewValue as Period)?.DisplayString ?? "";
        }

        private void BtnDrop_OnClick(object sender, RoutedEventArgs e)
        {
            DropPart.IsOpen = !DropPart.IsOpen;
            Txt.Text = SelectedPeriod.DisplayString;
        }

        private void BtnSelect_OnClick(object sender, RoutedEventArgs e)
        {
            DropPart.IsOpen = !DropPart.IsOpen;
            SelectedPeriod = _dropVm.Period;
        }
    }
    
}
