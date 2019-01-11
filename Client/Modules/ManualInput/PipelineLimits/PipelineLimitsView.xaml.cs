using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.ManualInput.PipelineLimits
{
    public partial class PipelineLimitsView : UserControl
    {
        public PipelineLimitsView()
        {
            InitializeComponent();
            Tree.SelectionChanged += Tree_SelectionChanged;
        }

        private void Tree_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            if(Tree.SelectedItem != null)
                Tree.ScrollIntoView(Tree.SelectedItem);
        }
    }
}
