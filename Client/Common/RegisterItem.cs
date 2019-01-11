using System;
using System.Collections.ObjectModel;
using GazRouter.Common.Ui.Templates;
using GazRouter.Common.ViewModel;
namespace GazRouter.Common{
    public enum PermissionType
    {
        Hidden = 0,
        Read,
        Write
    }
    public class RegisterItem : PropertyChangedBase
    {
#region constructor
        [Obsolete]
        public RegisterItem(string name, int id, int? parentId)
        {
            Items = new ObservableCollection<RegisterItem>();
            Name = name;
            Id = id;            
        }
        public RegisterItem(string name, int id)
        {
            Items = new ObservableCollection<RegisterItem>();
            Name  = name;
            Id    = id;
        }
#endregion
#region property
        public static Action<RegisterItem> IsChanged;

        public int Id { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public int? ParentId => _parent.Id;
        private RegisterItem _parent;

        private PermissionType _permissions;
        public PermissionType Permission
        {
            get
            {
                return _permissions;
            }
            set
            {
                SetPermission(value);
                SetValueForAllChildrens(value);// 1 действие родителя - устанавливает значения для всех детенышей
                ChangeParentPermission();      // 2 сообщение родителю о изменении его детеныша
            }
        }

        public int RoleId { get; set; }

        private ObservableCollection<RegisterItem> _items;
        public ObservableCollection<RegisterItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged(() => Items);
            }
        }
#endregion
#region methods
        public void SetPermission(PermissionType permission)
        {
            _permissions = permission;
            OnPropertyChanged(() => Permission);
            IsChanged?.Invoke(this);
        }
        private void SetValueForAllChildrens(PermissionType permission)
        {
            if (_items.Count <= 0) return;
            foreach (var registerItem in _items) registerItem.Permission = permission;
        }
        private void ChangeParentPermission()
        {
            if (_parent == null) return;
            // 
            var maxPermissionInChildrens = PermissionType.Hidden;
            foreach (var item in _parent.Items)
                if ((int) item.Permission > (int)maxPermissionInChildrens)
                    maxPermissionInChildrens = item.Permission;
            //
            if (_parent.Permission == maxPermissionInChildrens) return;
            _parent.SetPermission(maxPermissionInChildrens);
        }

        public void SetParent(RegisterItem parent)
        {
            _parent = parent;
        }
        public static PermissionType GetPermissionType(int i)
        {
            switch (i)
            {
                case 2: return PermissionType.Write;
                case 1: return PermissionType.Read;
                case 0: return PermissionType.Hidden;
            }
            throw new Exception("Недопустимый тип!");
        }
        public static ObservableCollection<PermissionWrapper> GetPermissionWrapper()
        {
            return new ObservableCollection<PermissionWrapper>
            {
                new PermissionWrapper {PermissionType = PermissionType.Hidden,
                    DisplayMember = "запрещено"},
                new PermissionWrapper {PermissionType = PermissionType.Read,
                    DisplayMember = "просмотр"},
                new PermissionWrapper {PermissionType = PermissionType.Write,
                    DisplayMember = "редактирование"}
            };
        }
#endregion
    }
}