using System;
using System.Collections.Generic;
using System.Windows.Input;
using GazRouter.Common.Ui.Behaviors;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ActionsRolesUsers.ViewModels
{
    public class ActiveSessionsViewModel : LockableViewModel, ITabItem
    {        
        private List<UserSessionDTO> _sessions;

        public ActiveSessionsViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
        }

        public List<UserSessionDTO> Sessions
        {
            get
            {
            
                return _sessions;
            }
            private set
            {
                _sessions = value;
                OnPropertyChanged(() => Sessions);
            }
        }

        public ICommand RefreshCommand { get; }

        internal async void Refresh()
        {
            try
            {
                Behavior.TryLock("Загрузка сессий");
                var sessions = await new SessionManagementServiceProxy().GetActiveSessionsAsync();
                Sessions = new List<UserSessionDTO>(sessions);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }

        public void Activate()
        {
            if (_sessions == null)
                Refresh();
            
        }

        public void Deactivate()
        {
          
        }
    }
}