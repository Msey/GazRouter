using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.Controls.InputStory
{
    public partial class StoryErrorsPresenter : UserControl
    {
        public StoryErrorsPresenter()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(StoryErrorsPresenter_Load);
        }

        void StoryErrorsPresenter_Load(object sender, RoutedEventArgs e)
        {
            if (NewIncomingErrors != null)
            {
                if (NewIncomingErrors.Item2)
                {
                    ((Storyboard)((StoryErrorsPresenter)sender).Resources["ScaleY"]).Begin();
                    ((Storyboard)((StoryErrorsPresenter)sender).Resources["ScaleX"]).Begin();
                }
                this.textBlock.Text = $"{this.NewIncomingErrors.Item1}";
            }
        }
                
        public Tuple<int?, bool> NewIncomingErrors
        {
            get { return (Tuple<int?, bool>)GetValue(NewIncomingErrorsProperty); }
            set { SetValue(NewIncomingErrorsProperty, value); }
        }        

        public static readonly DependencyProperty NewIncomingErrorsProperty =
            DependencyProperty.Register("NewIncomingErrors", typeof(Tuple<int?, bool>), typeof(StoryErrorsPresenter), new PropertyMetadata(null));        
    }
}
