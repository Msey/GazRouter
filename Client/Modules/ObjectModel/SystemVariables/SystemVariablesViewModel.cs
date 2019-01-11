using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.SystemVariables;
using GazRouter.DTO.Infrastructure.Faults;
using GazRouter.DTO.SystemVariables;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.ObjectModel.SystemVariables
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class SystemVariablesViewModel : LockableViewModel, INavigationAware
    {
        private SystemVariable _selectedSystemVariable;

        private List<SystemVariable> _systemVariables;

        public SystemVariablesViewModel()
        {
            RefreshCommand = new DelegateCommand(OnRefreshCommandExecuted);
        }

        public List<SystemVariable> SystemVariables
        {
            get { return _systemVariables; }
            set { SetProperty(ref _systemVariables, value); }
        }

        public SystemVariable SelectedSystemVariable
        {
            get { return _selectedSystemVariable; }
            set { SetProperty(ref _selectedSystemVariable, value); }
        }

        public DelegateCommand RefreshCommand { get; }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void Refresh()
        {
            if (RefreshCommand.CanExecute())
            {
                RefreshCommand.Execute();
            }
        }

        public void Save(SystemVariable systemVariable)
        {
            Behavior.TryLock();
            try
            {
                new SysVarServiceProxy().EditIusVariableValueAsync(new IusVariableParameterSet
                {
                    Name = systemVariable.Name,
                    Value = systemVariable.Value
                });
            }
            catch (FaultException<FaultDetail> ex) when (ex.Detail.FaultType == FaultType.IntegrityConstraint)
            {
                MessageBoxProvider.Alert(ex.Detail.Message, "Ошибка");
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void LoadSystemVariables()
        {
            Behavior.TryLock();
            try
            {
                var result = await new SysVarServiceProxy().GetIusVariableListAsync();
                SystemVariables = result.Select(v => new SystemVariable(v, this)).ToList();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void OnRefreshCommandExecuted()
        {
            LoadSystemVariables();
        }
    }
}