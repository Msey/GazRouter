using System;
using GazRouter.Common.ViewModel;
using GazRouter.DTO;
using GazRouter.DTO.EventLog;

namespace GazRouter.Modes.EventLog
{
    public abstract class EventTabViewModelBase : LockableViewModel
    {
      /*  protected readonly List<TDto> _items = new List<TDto>();
        public abstract IEnumerable<TDto> Items { get; }*/

        private EventDTO _eventDTO;
        public EventDTO EventDTO
        {
            get { return _eventDTO; }
            set
            {
                _eventDTO = value;
                        LoadData();
                
            }
        }


        public Func<bool> ReloadParent;
        protected abstract void LoadData();
        protected abstract void Refresh(bool refreshParent = false);
        public abstract string Header { get; }


        private EventListType _type = EventListType.List;
        public EventListType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(() => Type);
            }
        }

       /* protected void ClearList()
        {
            _items.Clear();
            OnPropertyChanged(() => Items);
        }*/
    }
}
