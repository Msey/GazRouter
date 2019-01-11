using System;
using System.Windows;
using Utils.Extensions;
using PropertyMetadata = Telerik.Windows.PropertyMetadata;

namespace GazRouter.Controls.Volume
{
    public partial class VolumeValue
    {
        public VolumeValue()
        {
            InitializeComponent();
        }



        #region VOLUME
        public double? Volume
        {
            get { return GetValue(VolumeProperty) as double?; }
            set { SetValue(VolumeProperty, value); }
        }


        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register(
                "Volume",
                typeof(double?),
                typeof(VolumeValue),
                new PropertyMetadata(OnAnyPropertyChanged));




        #endregion


        #region FORMAT STRING
        public string FormatString
        {
            get { return GetValue(FormatStringProperty) as string; }
            set { SetValue(FormatStringProperty, value); }
        }


        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register(
                "FormatString",
                typeof(string),
                typeof(VolumeValue),
                new PropertyMetadata(OnAnyPropertyChanged));

        #endregion


        private static void OnAnyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as VolumeValue;
            if (ctrl != null)
                ctrl.Txt.Text = ctrl.Volume != null ? ctrl.Volume.Value.ToString(ctrl.FormatString) : "";
            
        }

    }

}


