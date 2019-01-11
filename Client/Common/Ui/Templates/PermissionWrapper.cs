using GazRouter.Common.ViewModel;
namespace GazRouter.Common.Ui.Templates
{
    public class PermissionWrapper : PropertyChangedBase
    {
        private PermissionType _permissionType;
        public PermissionType PermissionType
        {
            get { return _permissionType; }
            set
            {
                _permissionType = value;
                OnPropertyChanged(() => PermissionType);
            }
        }

        private string _displayMember;
        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged(() => DisplayMember);
            }
        }
    }
}
