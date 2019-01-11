using System.Windows;

namespace GazRouter.Controls.InputStory
{
    public partial class InputStoryView
    {
        
        public InputStoryView()
        {
            InitializeComponent();
            this.Loaded += InputStoryView_Loaded;
            this.Unloaded += InputStoryView_Unloaded;            
        }
        private void InputStoryView_Loaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as InputStoryViewModel).AutorefreshEnable();
        }

        private void InputStoryView_Unloaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as InputStoryViewModel).AutorefreshDisable();  
        }
    }
}
