using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Events;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.EventRecipient;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Modes.EventLog
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class EventLogMainViewModel : MainViewModelBase
    {
        private List<EventLogTabBase> _tabs;

        public List<EventLogTabBase> Tabs
        {
            get { return _tabs; }
            set
            {
                _tabs = value;
                OnPropertyChanged(() => Tabs);
            }
        }

        private EventLogTabBase _selectedTab;

        public EventLogTabBase SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    foreach (var eventLogTabBase in Tabs)
                    {
                        eventLogTabBase.IsActive = eventLogTabBase == value;
                    }
                }
            }
        }


    

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if(UserProfile.Current.Site.IsEnterprise)
                Tabs = new List<EventLogTabBase>
                {
                    new MainEventViewModel(EventListType.List),
                    new MainEventViewModel(EventListType.Trash),
                    new MainEventViewModel(EventListType.Archive),
                    new AnalyticalViewModel()
                };
            else
                Tabs = new List<EventLogTabBase>
                {
                    new MainEventViewModel(EventListType.List),
                    new MainEventViewModel(EventListType.Trash),
                    new MainEventViewModel(EventListType.Archive)
                };

            SelectedTab = Tabs.First();

        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SelectedTab.IsActive = false;
        }


        //todo: Нужно разобраться с обновлением счетчика. Точно такая же процедура определена в DispatcherTaskWatcher
        private static NonAckEventCountDTO _nonAckEventCount = new NonAckEventCountDTO();
        public static async void UpdateNotAckEventCount()
        {
            if (UserProfile.Current == null || UserProfile.Current.Site == null)
                return;

            var count = await new EventLogServiceProxy().GetNotAckEventCountAsync(UserProfile.Current.Site.Id).ConfigureAwait(false);

            if (_nonAckEventCount.LastEventDate != count.LastEventDate || _nonAckEventCount.Count != count.Count)
            {
                ServiceLocator.Current.GetInstance<IEventAggregator>()
                    .GetEvent<NotAckEventCountChangedEvent>()
                    .Publish(count);

                _nonAckEventCount = count;
            }

        }

        public static void NotifyEventListUpdated()
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<EventListUpdatedEvent>().Publish(null);
        }
    }
}