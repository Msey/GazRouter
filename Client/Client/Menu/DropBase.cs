using System;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
namespace GazRouter.Client.Menu
{
    public class DropBase : ViewModelBase
    {
        private Action _closeAction;
        public void AddCloseAction(Action closeAction)
        {
            _closeAction = closeAction;
        }
        protected void NotifyClose()
        {
            _closeAction?.Invoke();
        }
#region auth
        private Branch _parent;
        public void RegisterDropMenu(LinkType linkType)
        {
            var info = LinkRegister.GetLinkInfo(linkType);
            _parent = Authorization2.Inst.AddBranch(linkType, info.Name);
        }
        public void RegisterLink(LinkType linkType)
        {
            var info = LinkRegister.GetLinkInfo(linkType);
            Authorization2.Inst.AddLeaf(_parent, linkType, info.Name);
        }
#endregion
    }
}
#region trash

//var link = new Leaf(linkType);
//_parent.Childs.Add(link);


//var parent = new Branch
//{
//    Name = name
//};
//_parent = parent;

//            var node = new PermissionNode
//            {
//                LinkType = linkType,
//                NodeType = NodeType.Leaf
//            };
#endregion
