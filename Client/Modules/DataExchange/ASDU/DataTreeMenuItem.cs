using System;
using System.Collections.ObjectModel;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace DataExchange.ASDU
{
    public class DataTreeMenuItem: ViewModelBase
    {
        public DataTreeMenuItem()
        {
            this.SubItems = new ObservableCollection<DataTreeMenuItem>();
            IsEnabled = true;
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public Uri IconUrl
        {
            get;
            set;
        }
        public ObservableCollection<DataTreeMenuItem> SubItems
        {
            get;
            set;
        }

        public DelegateCommand Command { get; set; }
    }
}