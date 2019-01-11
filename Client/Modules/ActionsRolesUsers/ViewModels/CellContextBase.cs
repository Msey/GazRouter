using System;
using GazRouter.Common.ViewModel;
using JetBrains.Annotations;

namespace GazRouter.ActionsRolesUsers.ViewModels
{
    public abstract class CellContextBase : PropertyChangedBase
    {
        protected ActionContext _parentContext;
        protected bool _isChanged;
        protected int _roleId;
     
        protected CellContextBase([NotNull] ActionContext parentContext, int roleId)
        {
            if (parentContext == null) throw new ArgumentNullException("parentContext");
            _parentContext = parentContext;
     
            _roleId = roleId; 
        }

        public abstract bool? IsChecked { get; set; }

        public int RoleId
        {
            get { return _roleId; }
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
                _parentContext.IsChanged = _isChanged;
            }
        }

        public ActionContext ParentContext
        {
            get { return _parentContext; }
        }
    }
}