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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace DataExchange.ASDU
{
    public partial class AsduDataView : UserControl
    {
        private readonly TreeLinkDrawHelper _linkDrawHelper;

        public AsduDataView()
        {
            InitializeComponent();
            _linkDrawHelper = new TreeLinkDrawHelper(IusTree, AsduTree, DrawingRoot, () => cbLinkDisplayMode.IsChecked.GetValueOrDefault());

        }

        private void Tree_CopyingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            // TODO
        }

        private void TreeContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            RadContextMenu menu = (RadContextMenu)sender;
            var row = menu.GetClickedElement<GridViewRow>();
            if (row != null)
            {
                row.IsSelected = true;
            }
        }
    }
}
