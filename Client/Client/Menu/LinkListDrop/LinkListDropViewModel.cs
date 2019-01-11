using System;
using System.Collections.Generic;
using GazRouter.Common;
namespace GazRouter.Client.Menu.LinkListDrop
{
    public class LinkListDropViewModel : DropBase
    {
        public LinkListDropViewModel()
        {
            _itemList = new List<MenuItemBase>();
        }
#region variables
        [Obsolete]
        public static Func<string, Type[], PermissionType> AuthLink;
        private readonly List<MenuItemBase> _itemList;
        public IEnumerable<MenuItemBase> Items => _itemList.AsReadOnly();
#endregion
#region methods
#region new methods
        public void AddLink(LinkType linkType)
        {
            RegisterLink(linkType);
            if (!Authorization2.Inst.IsAuthorized(linkType)) return;
            //
            var link = LinkRegister.GetLinkInfo(linkType);
            var item = new LinkMenuItem(link.Name, link.Uri, link.Image);
            item.AddCloseAction(NotifyClose);
            _itemList.Add(item);
        }
        public void AddSapLink(LinkType linkType, bool isExternal = false)
        {
            RegisterLink(linkType);
            if (!Authorization2.Inst.IsAuthorized(linkType)) return;
            //
            var link = LinkRegister.GetLinkInfo(linkType);
            var item = new LinkMenuItem(link.Name, link.Uri, link.Image, isExternal);
            item.AddCloseAction(NotifyClose);
            _itemList.Add(item);
        }
#endregion
        public void AddSeporator()
        {
            _itemList.Add(new SeparatorMenuItem());
        }
        public void AddSection(string name)
        {
            _itemList.Add(new SectionMenuItem(name));
        }
#endregion
    }
}
//        // 1
//        [Obsolete]
//        public void AddLink(string name, Type viewType, Type[] viewModelType, string imgSrc = "", bool isExternal = false)
//        {
//            if (AuthLink(name, viewModelType) == PermissionType.Hidden) return;
//            // 
//            var item = new LinkMenuItem(name, viewType.FullName, imgSrc, isExternal);
//            item.AddCloseAction(NotifyClose);
//            _itemList.Add(item);
//        }
//        // 2 ReportViewModel + customViewModel
//        [Obsolete]
//        public void AddLink(string name, Type viewType, Type viewModelType, string imgSrc = "")
//        {
//            if (AuthLink(name, new []{ viewModelType }) == PermissionType.Hidden) return;            
//            //
//            var uri = $"{viewType.FullName}?reportId={viewModelType.FullName}";
//            var item = new LinkMenuItem(name, uri, imgSrc);
//            item.AddCloseAction(NotifyClose);
//            _itemList.Add(item);
//        }
//        // 3 - stubAction
//        [Obsolete]
//        public void AddLink(string name, Type viewModelType, Action action, string imgSrc = "")
//        {
//            if (AuthLink(name, new [] { viewModelType }) == PermissionType.Hidden) return;
//            //
//            var item = new LinkMenuItem(name, action, imgSrc);
//            item.AddCloseAction(NotifyClose);
//            _itemList.Add(item);
//        }
//        // 4 SAP BO
//        [Obsolete]
//        public void AddLink(string name, string uri, string imgSrc = "", bool isExternal = false)
//        {
//            var item = new LinkMenuItem(name, uri, imgSrc, isExternal);
//            item.AddCloseAction(NotifyClose);
//            _itemList.Add(item);
//        }