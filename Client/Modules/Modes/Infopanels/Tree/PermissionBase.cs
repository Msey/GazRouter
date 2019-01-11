using System;
using GazRouter.Application;
using GazRouter.Common;

namespace GazRouter.Modes.Infopanels.Tree
{
    /// <summary>
    /// 
    /// rules: 
    /// -- Global Permissions ---------------------------------
    /// 1. Global Denied - скрывает отображение модуля
    /// 2. Global Read   - отображает дерево и контент в режиме просмотра, кнопка Update
    /// 3. Global Write  - делегирует функции доступа на Local Permissions
    /// -- content --------------------------------------------
    /// 4. Local Denied - скрывает контент дерева
    /// 5. Local Read   - отображает контент на просмотр 
    /// 6. Local Write  - отображает контент на редактирование
    /// -- role -----------------------------------------------
    /// 7. Local Admin  - имеет полные Local привилегии
    /// 8. User         - имеет соответствующие локальные привилегии
    /// 
    /// </summary>
    public abstract class PermissionBase
    {
#region constructor
        protected PermissionBase(bool isEditPermission)
        {
            GlobalEditPermission = isEditPermission;
        }
#endregion
#region methods
        public int UserId => UserProfile.Current.Id;
        public Guid SiteId => UserProfile.Current.Site.Id;
        public bool GlobalEditPermission { get; }
        public bool IsPds => UserProfile.Current.Site.IsEnterprise;
        public bool IsReportAdminRole()
        {
            return Authorization2.Inst.IsReportAdminRole();
        }
#endregion
    }
}
#region trash
// public bool IsReportAdminRole => Authorization2.Inst.IsReportAdmin;
#endregion

