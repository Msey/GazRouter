using System.Collections.Generic;
using GazRouter.DTO.Dashboards;

namespace GazRouter.Modes.Infopanels.Tree
{
    /// <summary>         
    /// global permission 
    /// local permission  
    /// </summary>
    public class PanelPermission : PermissionBase
    {
#region constructor
        public  PanelPermission(bool isEditPermission) : base(isEditPermission){}
#endregion
#region variables
        private Dictionary<int, int> _panelIdMaxPermission;
#endregion
#region methods
        public void SetPermissions(Dictionary<int, int> d)
        {
            _panelIdMaxPermission = d;
        }
        public bool IsPanelEditable(int id)
        {
            if (!GlobalEditPermission) return false;
            if (IsReportAdminRole()) return true;
            //
            if (!_panelIdMaxPermission.ContainsKey(id)) return false;
            return _panelIdMaxPermission[id] == 2;
        }
        public bool IsFolderEditable()
        {
            return GlobalEditPermission;
        }
#region commands
        public bool Filter => !(IsReportAdminRole() || IsPds);        
        public bool CanRefreshCommand()
        {
            return true;
        }
        public bool CanAddCommand(InfopanelItemType itemType, ItemBase selectedItem)
        {
            if (!GlobalEditPermission) return false;
            //
            if (selectedItem == null) return true;
            return selectedItem is FolderItem;
        }
        public bool CanEditCommand(ItemBase selectedItem)
        {
            if (selectedItem == null) return false;
            //
            if (selectedItem is FolderItem)  return IsFolderEditable();
            if (selectedItem is ItemContent) return IsPanelEditable(selectedItem.Id);
            return false;
        }
        public bool CanDeleteCommand(ItemBase selectedItem)
        {
            return CanEditCommand(selectedItem);
        }
        public bool CanCopyCommand(ItemBase selectedItem)
        {      
            if (selectedItem is ItemContent) return IsPanelEditable(selectedItem.Id);
            return false;
        }
        public bool CanShareCommand(ItemBase selectedItem)
        {
            return selectedItem != null && IsReportAdminRole();
        }
#endregion
#endregion
    }
}