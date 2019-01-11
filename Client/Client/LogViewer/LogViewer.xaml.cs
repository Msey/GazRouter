using System;
using System.Windows;
using Telerik.Windows.Controls;

namespace GazRouter.Client.LogViewer
{
    public partial class LogViewer
    {
        public LogViewer()
        {
            InitializeComponent();

            DataContext = new LogViewerViewModel();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }

        private void LogViewer_OnClosed(object sender, WindowClosedEventArgs e)
        {
            ((LogViewerViewModel) DataContext).Close();
        }

        private void GridViewDataControl_OnDataLoaded(object sender, EventArgs e)
        {
            var radGridView = (RadGridView) sender;
            radGridView.ScrollIndexIntoView(radGridView.Items.Count - 1);
        }
    }
}