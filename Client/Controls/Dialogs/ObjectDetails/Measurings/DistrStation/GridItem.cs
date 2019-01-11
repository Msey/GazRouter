using System.Collections.Generic;
using GazRouter.Controls.Measurings;

namespace GazRouter.Controls.Dialogs.ObjectDetails.Measurings.DistrStation
{
    public class GridItem
    {
        public GridItem()
        {
            Childs = new List<GridItem>();
        }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public DoubleMeasuring P { get; set; }
        public DoubleMeasuring T { get; set; }
        public DoubleMeasuring Q { get; set; }

        public List<GridItem> Childs { get; set; }
    }
}