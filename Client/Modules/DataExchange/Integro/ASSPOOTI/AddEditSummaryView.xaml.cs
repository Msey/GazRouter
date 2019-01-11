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

namespace GazRouter.DataExchange.Integro.ASSPOOTI
{
    public partial class AddEditSummaryView : RadWindow
    {
        public AddEditSummaryView()
        {
            InitializeComponent();
        }

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/DataExchange;component/Integro/ASSPOOTI/AddEditSummaryView.xaml", System.UriKind.Relative));
        }
    }
}
