using System;
using GazRouter.Common.ViewModel;

namespace GazRouter.Modes.EventLog
{
    public class EntityVm : ViewModelBase
    {
        private bool _isChecked;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid SiteId { get; set; }

        public Action OnCheckedChanged { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (SetProperty(ref _isChecked, value))
                {
                    OnCheckedChanged?.Invoke();
                }
            }
        }
    }
}