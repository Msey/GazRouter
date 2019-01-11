using System;
using GazRouter.Common.ViewModel;


namespace GazRouter.Balances.DayBalance
{
    public class TableViewModel : SummaryItem
    {
        private bool _showSortOrder;
        
        public TableViewModel(string alias, DateTime day) : base(alias, day)
        {
            
        }

        public bool ShowSortOrder
        {
            get { return _showSortOrder; }
            set { SetProperty(ref _showSortOrder, value); }
        }


        public bool AutoExpand { get; set; }
        
        public ItemBase SelectedItem { get; set; }

    }
}
        