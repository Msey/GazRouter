using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Application.Wrappers;
using GazRouter.Common;
using GazRouter.Common.Events;
using GazRouter.Common.GoodStyles;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.EventRecipient;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
namespace GazRouter.Client.Menu.EventsDrop
{
    public class EventsDropViewModel : DropBase
    {
        public void AddLink(LinkType linkType)
        {
            RegisterLink(linkType);
            if (!Authorization2.Inst.IsAuthorized(linkType)) return;
            //
            var link = LinkRegister.GetLinkInfo(linkType);
            LogLink = new LinkMenuItem(link.Name, link.Uri, link.Image);
            RefreshEventList();
        }
        [Obsolete]
        public static Func<string, Type[], PermissionType> AuthLink;

        public LinkMenuItem LogLink { get; set; }
        public List<EventWrap> EventList { get; set; }
        public bool HasEvents => EventList != null && EventList.Any();
        public string SectionName => EventList.Any(e => e.NonAck) ? "НОВЫЕ СОБЫТИЯ" : "ПОСЛЕДНИЕ СОБЫТИЯ";
        public async void RefreshEventList()
        {
            var evntList = await new EventLogServiceProxy().GetEventListAsync(
                new GetEventListParameterSet
                {
                    QueryType = EventListType.List,
                    SiteId = UserProfile.Current.Site.Id,
                    StartDate = DateTime.Now.AddDays(-1).ToLocal(),
                    EndDate = DateTime.Now.ToLocal()
                });

            if (evntList.Any(e => !e.IsQuote))
            {
                var nonAckList = evntList.Where(e => !e.IsQuote).ToList();
                EventList = nonAckList.GetRange(0, Math.Min(3, nonAckList.Count)).Select(e => new EventWrap(e)).ToList();
            }
            else
            {
                EventList = evntList.GetRange(0, Math.Min(3, evntList.Count)).Select(e => new EventWrap(e)).ToList();
            }
            OnPropertyChanged(() => EventList);
            OnPropertyChanged(() => SectionName);
            OnPropertyChanged(() => HasEvents);
        }
    }
    public class EventWrap : DtoWrapperBase<EventDTO>
    {
        public EventWrap(EventDTO dto) : base(dto)
        {
            AckCommand = new DelegateCommand(Ack);
        }
        public Brush EventTextColor => Dto.IsEmergency ? Brushes.Red : Brushes.Black;
        public bool NonAck => !Dto.IsQuote;
        public DelegateCommand AckCommand { get; set; }
        public async void Ack()
        {
            await new EventLogServiceProxy().AckEventAsync(
                new AckEventParameterSet
                {
                    EventId = Dto.Id,
                    SiteId = UserProfile.Current.Site.Id
                });
            var count = await new EventLogServiceProxy().GetNotAckEventCountAsync(UserProfile.Current.Site.Id).ConfigureAwait(false);
            ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<NotAckEventCountChangedEvent>()
                .Publish(count);
        }
    }
}
#region trash
//[Obsolete]
//public EventsDropViewModel(string name, Type viewType, Type[] viewModelType)
//{
//    if (AuthLink(name, viewModelType) == PermissionType.Hidden) return;
//    //
//    LogLink = new LinkMenuItem(name, viewType.FullName, "/Common;component/Images/32x32/event_log.png");
//    RefreshEventList();
//}
//public EventsDropViewModel() { }
#endregion