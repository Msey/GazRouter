using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using GazRouter.Application;
using Utils.Units;

namespace GazRouter.Controls
{
    public class SmartTextBlock : Control
    {
        const string TemplateXaml =
            @"<ControlTemplate   
                xmlns=""http://schemas.microsoft.com/client/2007""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                    <TextBlock x:Name=""Text"" FontFamily=""Segoe UI"" TextWrapping=""Wrap"" />
                </ControlTemplate>";

        /// <summary>
        /// The name of the TextBlock part.
        /// </summary>
        private const string TextBlockName = "Text";

        /// <summary>
        /// Gets or sets the text block reference.
        /// </summary>
        private TextBlock TextBlock { get; set; }

  

        #region public string Text
        /// <summary>
        /// Gets or sets the contents of the TextBox.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(SmartTextBlock),
                new PropertyMetadata(OnTextPropertyChanged));

        /// <summary>
        /// TextProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its Text.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SmartTextBlock)d).FillTextBlock();
        }

        #endregion public string Text


        private void FillTextBlock()
        {
            if (TextBlock == null)
                return;

            TextBlock.Width = Width;
            TextBlock.TextWrapping = TextWrapping.Wrap;
            
            var txt = Text;
            txt = txt.Replace("[T]", Temperature.GetAbbreviation(UserProfile.Current.UserSettings.TemperatureUnit));
            txt = txt.Replace("[P]", Pressure.GetAbbreviation(UserProfile.Current.UserSettings.PressureUnit));
            txt = txt.Replace("[C]", CombustionHeat.GetAbbreviation(UserProfile.Current.UserSettings.CombHeatUnit));


            TextBlock.Text = txt;
        }

        
        
        /// <summary>
        /// Initializes a new HighlightingTextBlock class.
        /// </summary>
        public SmartTextBlock()
        {
            Template = (ControlTemplate)XamlReader.Load(TemplateXaml);
            IsTabStop = false;
        }


        /// <summary>
        /// Override the apply template handler.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Grab the template part
            TextBlock = GetTemplateChild(TextBlockName) as TextBlock;

            FillTextBlock();
        }


    }
}
