using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GazRouter.Application.Wrappers;
using GazRouter.Common;
using GazRouter.Common.GoodStyles;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.DataProviders.DispatcherTask;
namespace GazRouter.Client.Menu.TasksDrop
{
    public class TasksDropViewModel : DropBase
    {
#region variables
        public LinkMenuItem LpuLink { get; set; }
        public List<EventWrap> EventList { get; set; }
        public bool HasEvents => EventList?.Any() ?? false;
        public string SectionName => EventList == null ? 
                                        "" : (EventList.Any(e => e.NonAck) ? 
                                            "НОВЫЕ ЗАДАЧИ" : "ПОСЛЕДНИЕ ЗАДАЧИ");
#endregion
#region methods
        public void AddLink(LinkType linkType, bool isExternal = false)
        {
            RegisterLink(linkType);
            if (!Authorization2.Inst.IsAuthorized(linkType)) return;
            // 
            var link = LinkRegister.GetLinkInfo(linkType);
            LpuLink = new LinkMenuItem(link.Name, link.Uri, link.Image);
        }
        public void RefreshEventList(List<TaskRecordPdsDTO> evntList)
        {
            //var evntList = await new EventLogServiceProxy().GetEventListAsync(
            //    new GetEventListParameterSet
            //    {
            //        QueryType = EventListType.List,
            //        SiteId = UserProfile.Current.Site.Id,
            //        StartDate = DateTime.Now.AddDays(-1).ToLocal(),
            //        EndDate = DateTime.Now.ToLocal()
            //    });

            //if (evntList.Any(e => !e.IsQuote))
            //{
            //    var nonAckList = evntList.Where(e => !e.IsQuote).ToList();
                EventList = evntList.GetRange(0, Math.Min(3, evntList.Count)).Select(e => new EventWrap(e)).ToList();
            //}
            //else
            //{
            //    EventList = evntList.GetRange(0, Math.Min(3, evntList.Count)).Select(e => new EventWrap(e)).ToList();
            //}
            OnPropertyChanged(() => EventList);
            OnPropertyChanged(() => SectionName);
            OnPropertyChanged(() => HasEvents);
        }
#endregion
    }
    public class EventWrap : DtoWrapperBase<TaskRecordPdsDTO>
    {
        public EventWrap(TaskRecordPdsDTO dto) : base(dto)
        {
            AckCommand = new DelegateCommand(Ack);
        }
        public Brush EventTextColor => Dto.CompletionDate.HasValue && 
                                       Dto.CompletionDate.Value.Date <= DateTime.Today.Date ? 
                                       Brushes.Red : Brushes.Black;
        public bool NonAck => !Dto.AckDate.HasValue;
        public DelegateCommand AckCommand { get; set; }
        public async void Ack()
        {
            await new DispatcherTaskServiceProxy().SetACKAsync(Dto.Id);

            DispatcherTasksWatcher.UpdateNonEventAck();
        }
    }
}
#region trash
//            AddLink(linkType, link);

//        private void AddLink(LinkType linkType, PermissionDTO2 link)
//        {
//            switch (linkType)
//            {
//                case LinkType.DispTasksAll:
//                    LpuLink = new LinkMenuItem(link.Name, link.Uri, link.Image);
//                    return;
//            }
//            throw new Exception("Недопустимый тип!");
//        }


//LpuVisible = true;

//private bool _lpuVisible;
//public bool LpuVisible
//{
//    get
//    {
//        return _lpuVisible;
//    }
//    set
//    {
//        _lpuVisible = value;
//        OnPropertyChanged(() => LpuVisible);
//    }
//}
//                    _itemList.Add(LpuLink);

//        private bool _pdsVisible = false;
//        public bool PdsVisible
//        {
//            get { return _pdsVisible; }
//            set
//            {
//                _pdsVisible = value;
//                OnPropertyChanged(() => PdsVisible);
//            }
//        }
//        public LinkMenuItem PdsLink { get; set; }

//public TasksDropViewModel()
//{
//    //            _itemList = new List<MenuItemBase>();
//}

//        private readonly List<MenuItemBase> _itemList;
//        public IEnumerable<MenuItemBase> Items => _itemList.AsReadOnly();

//[Obsolete]
//public TasksDropViewModel(LinkInfo lpulink, LinkInfo pdslink)
//{
//    if (AuthLink(pdslink.name, pdslink.viewModelType) == PermissionType.Hidden) { }
//    else
//    {
//        PdsLink = new LinkMenuItem(pdslink.name, pdslink.viewType.FullName);
//        //PdsLink.AddCloseAction(NotifyClose);
//        PdsVisible = true;
//
//    }
//    if (AuthLink(lpulink.name, lpulink.viewModelType) == PermissionType.Hidden) { }
//    else
//    {
//        LpuLink = new LinkMenuItem(lpulink.name, lpulink.viewType.FullName);
//        //LpuLink.AddCloseAction(NotifyClose);
//
//        LpuVisible = true;
//    }
//
//    //RefreshEventList();
//}
//[Obsolete]
//public static Func<string, Type[], PermissionType> AuthLink;
//[Obsolete]
//public struct LinkInfo
//{
//    public string name;
//    public Type viewType;
//    public Type[] viewModelType;
//}


#endregion