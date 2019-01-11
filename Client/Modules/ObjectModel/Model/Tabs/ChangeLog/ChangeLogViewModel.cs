using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.ObjectModel.Model.Tabs.ChangeLog
{
	public class ChangeLogViewModel : ViewModelBase
    {
        private Guid? _entityId;
	    private bool _isActive;

		public ChangeLogViewModel()
		{
            Refresh();
		}

        /// <summary>
        /// Идентификатор объекта, для которого отображается история изменения
        /// </summary>
        public Guid? EntityId
        {
            get { return _entityId; }
            set
            {
                if (SetProperty(ref _entityId, value))
                    Refresh();
            }
        }

        /// <summary>
        /// Если установлен в True, то при каждом изменении EntityId обновляется список вложений
        /// Это сделано для того, чтобы грузить данные только в том случае когда вкладка активна
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                    Refresh();
            }
        }

        private async void Refresh()
		{
		    if (!_isActive) return;
		    try
		    {
                Behavior.TryLock();

		        List = _entityId.HasValue
		            ? await new ObjectModelServiceProxy().GetEntityChangeListAsync(_entityId.Value)
		            : new List<EntityChangeDTO>();

                OnPropertyChanged(() => List);
		    }
		    finally 
		    {
                Behavior.TryUnlock();
		    }
		}
        
        
		public List<EntityChangeDTO> List { get; set; }

    }
}