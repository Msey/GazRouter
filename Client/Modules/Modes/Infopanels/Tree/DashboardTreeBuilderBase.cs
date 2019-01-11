using System;
using System.Threading.Tasks;
using GazRouter.DTO.Dashboards.Folders;
using Microsoft.Practices.ObjectBuilder2;

namespace GazRouter.Modes.Infopanels.Tree
{
    public abstract class DashboardTreeBuilderBase
    {
#region constructor
        protected DashboardTreeBuilderBase()
        {            
            GlobalGenericFolderName = "448B336E-509C-4296-9296-1FDBD9435C94";
            GenericFolderName       = "Общая";
            RootName = "root";
            RootId = -100;
            //
            RootItem = new FolderItem(new FolderDTO
            {
                Name = RootName, Id = RootId
            });
        }
#endregion
#region property
        public readonly string GlobalGenericFolderName;
        public readonly string GenericFolderName;
        public readonly string RootName;
        public readonly int RootId;
        public ItemBase RootItem;
#endregion
#region methods
        public abstract Task Build();
        public void Traversal<T>(T data, Action<T> action) where T : ItemBase
        {
            action.Invoke(data);
            data.Childs?.ForEach(item => Traversal((T)item, action));
        }
        public void Traversal(Action<ItemBase> action) 
        {
            Traversal(RootItem, action);
        }
#endregion
    }
}