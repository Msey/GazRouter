using System;
using System.Collections.ObjectModel;
using GazRouter.Common.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
namespace GazRouter.Client.LogViewer
{
    public class LogViewerViewModel
    {
        public static ObservableCollection<Tuple<string, string>> LogEntries { get; } =
            new ObservableCollection<Tuple<string, string>>();

        public static void Init()
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<AddLogEntryEvent>()
                .Subscribe(OnAddLogEntryEvent);
        }
        public LogViewerViewModel()
        {
            ClearCommand = new DelegateCommand(e => LogEntries.Clear());
        }

        public DelegateCommand ClearCommand { get; set; }
        public static void OnAddLogEntryEvent(Tuple<string, string> logEntry)
        {
            LogEntries.Add(logEntry);
        }
        public void Close()
        {
        }
    }
}
