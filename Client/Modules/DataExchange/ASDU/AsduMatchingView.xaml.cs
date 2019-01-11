using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using DataExchange.ASDU;
//using DataExchange.ASDU.Telerik.Windows.Examples.TreeListView;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace DataExchange.ASDU
{
    public partial class AsduMatchingView : UserControl
    {
        public AsduMatchingView()
        {
            InitializeComponent();
        }

        private void MatchingTreeMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var menu = (RadContextMenu)sender;
            var row = menu.GetClickedElement<GridViewRow>();
            if (row != null)
            {
                row.IsSelected = true;
            }
        }
    }
}
