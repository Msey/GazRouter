using System.Windows;
using GazRouter.Common;
using Telerik.Windows;

namespace GazRouter.Client.Menu
{
    public partial class MainMenuView
    {
        public MainMenuView()
        {
            InitializeComponent();
        }

        private void RadMenuItem_OnClick(object sender, RadRoutedEventArgs e)
        {
            IsolatedStorageManager.Clear();
            MessageBox.Show("Хранилище очищено");
        }

    }
}