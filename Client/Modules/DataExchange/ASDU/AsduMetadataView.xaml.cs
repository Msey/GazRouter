using System.Collections.Generic;
using System.Net;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Telerik.Windows.Controls;

namespace DataExchange.ASDU
{
    public partial class AsduMetadataView : UserControl
    {
        private readonly TreeLinkDrawHelper _linkDrawHelper;

        public AsduMetadataView()
        {
            InitializeComponent();
            _linkDrawHelper = new TreeLinkDrawHelper(IusTree, AsduTree, DrawingRoot, () => cbLinkDisplayMode.IsChecked.GetValueOrDefault());
        }

        private void Tree_CopyingCellClipboardContent(object sender, GridViewCellClipboardEventArgs e)
        {
            var tree = sender as RadTreeListView;
            if (tree == null)
            {
                return;
            }
            if (tree.CurrentCell.Column.DisplayIndex != e.Cell.Column.DisplayIndex)
            {
                e.Value = null;
            }
        }

    }
}
