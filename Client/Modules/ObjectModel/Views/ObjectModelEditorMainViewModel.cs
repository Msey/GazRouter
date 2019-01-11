using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.ObjectModel.Model.Aggregators;
using GazRouter.ObjectModel.Model.Dialogs.Errors;
using GazRouter.ObjectModel.Model.Dialogs.FindTreeNode;
using GazRouter.ObjectModel.Model.Pipelines;
using GazRouter.ObjectModel.Model.Schema;
using GazRouter.ObjectModel.Model.Tree;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using MenuItemCommand = GazRouter.ObjectModel.Model.MenuItemCommand;

namespace GazRouter.ObjectModel.Views
{
    [RegionMemberLifetime(KeepAlive = false)]
    [UsedImplicitly]
    public class ObjectModelEditorMainViewModel : ViewModelBase, IPageTitle, IConfirmNavigationRequest
    {
        private readonly ErrorsViewModel _errorsViewModel;

        private GasTransportSystemDTO _gasTransportSystemDTO;
        private bool _isChanged;

        private EntityPickerDialogViewModel _entityPicker;

        public ObjectModelEditorMainViewModel()
        {            
            var tempList = ClientCache.DictionaryRepository.GasTransportSystems;
            PipeLineManagerViewModel = new PipeLineManagerViewModel();
            PointObjectsManagerViewModel = new PointObjectsManagerViewModel();
            AggregatorsViewModel = new AggregatorsViewModel();

            PipeLineManagerViewModel.EditableFullTreeVM.SetSelectedGasTransport = SetSelectedGasTransport;
            PipeLineManagerViewModel.EditableFullTreeVM.GetSelectedGasTransport = GetSelectedGasTransport;
            PipeLineManagerViewModel.EditableFullTreeVM.FindCommands = GetFindCommands();
            PipeLineManagerViewModel.EditableFullTreeVM.ValidateCommands = GetValidateCommands();

            PointObjectsManagerViewModel.EditableFullTreeVM.SetSelectedGasTransport = SetSelectedGasTransport;
            PointObjectsManagerViewModel.EditableFullTreeVM.GetSelectedGasTransport = GetSelectedGasTransport;
            PointObjectsManagerViewModel.EditableFullTreeVM.FindCommands = GetFindCommands();
            PointObjectsManagerViewModel.EditableFullTreeVM.ValidateCommands = GetValidateCommands();
            PointObjectsManagerViewModel.EditableFullTreeVM.ListGasTransportSystems = tempList;

            PipeLineManagerViewModel.EditableFullTreeVM.ListGasTransportSystems = tempList;
            PointObjectsManagerViewModel.EditableFullTreeVM.SelectedGasTransport = tempList.FirstOrDefault();

            SchemaViewModel = new SchemaViewModel
            {
                FloEditControlViewModel =
                {
                    GotoTreeCommand = new DelegateCommand<Guid?>(GoToTree)
                }
            };

            _errorsViewModel = new ErrorsViewModel(id => GoToTree(id));
        }

        public InteractionRequest<Confirmation> NavigationRequest { get; } = new InteractionRequest<Confirmation>();

        public PipeLineManagerViewModel PipeLineManagerViewModel { get; }
        public PointObjectsManagerViewModel PointObjectsManagerViewModel { get; }

        public AggregatorsViewModel AggregatorsViewModel { get; }

        public SchemaViewModel SchemaViewModel { get; }

        public bool IsNewDiagramVisible
        {
            get
            {
//#if (DEVELOP_MODE)
//                return UserProfile.Current.Login == "DEV\\alex.anisimoff";
//#else
//                return false;
//#endif
                return false;
            }
        }

        public string PageTitle => "Объектная модель";
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
#if (DEBUG)
            //SchemaViewModel.IsSelected = true;
#endif
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            SchemaViewModel.ConfirmNavigationRequest(NavigationRequest, continuationCallback);
        }

        private List<MenuItemCommand> GetFindCommands()
        {
            return new List<MenuItemCommand>
            {
                new MenuItemCommand(OpenFindPicker, () => true, "Поиск..."),
                new MenuItemCommand(FindById, () => true, "Поиск по GUID...")
            };
        }

        private List<MenuItemCommand> GetValidateCommands()
        {
            return new List<MenuItemCommand>
            {
                new MenuItemCommand(Validate, () => true, "Валидация"),
                new MenuItemCommand(ShowErrors, () => true, "Список ошибок...")
            };
        }

        private async void Validate()
        {
            PipeLineManagerViewModel.EditableFullTreeVM.Behavior.TryLock();
            PointObjectsManagerViewModel.EditableFullTreeVM.Behavior.TryLock();

            await new ObjectModelServiceProxy().ValidateAsync();
            PipeLineManagerViewModel.EditableFullTreeVM.Refresh();
            PointObjectsManagerViewModel.EditableFullTreeVM.Refresh();
            ShowErrors();

            PipeLineManagerViewModel.EditableFullTreeVM.Behavior.TryUnlock();
            PointObjectsManagerViewModel.EditableFullTreeVM.Behavior.TryUnlock();
        }

        private void ShowErrors()
        {
            _errorsViewModel.DialogResult = null;
            new ErrorsView {DataContext = _errorsViewModel}.ShowDialog();
            _errorsViewModel.LoadErrors();
        }

        private void SetSelectedGasTransport(LockableViewModel vm, GasTransportSystemDTO param)
        {
            if (_isChanged)
            {
                return;
            }
            _isChanged = true;
            if (_gasTransportSystemDTO == param)
            {
                return;
            }
            _gasTransportSystemDTO = param;
            if (vm == PipeLineManagerViewModel.EditableFullTreeVM)
            {
                PointObjectsManagerViewModel.EditableFullTreeVM.SelectedGasTransport = param;
            }
            else if (vm == PointObjectsManagerViewModel.EditableFullTreeVM)
            {
                PipeLineManagerViewModel.EditableFullTreeVM.SelectedGasTransport = param;
            }
            _isChanged = false;
        }

        private GasTransportSystemDTO GetSelectedGasTransport()
        {
            return _gasTransportSystemDTO;
        }

        private void GoToTree(Guid? entityId)
        {
            if (PointObjectsManagerViewModel.EditableFullTreeVM.TreeModel.SelectNode(entityId.Value))
            {
                PointObjectsManagerViewModel.IsSelected = true;
            }
            else if (PipeLineManagerViewModel.EditableFullTreeVM.TreeModel.SelectNode(entityId.Value))
            {
                PipeLineManagerViewModel.IsSelected = true;
            }
            else
            {
                MessageBoxProvider.Alert("Объект не найден!", "Сообщение");
            }
        }

        private void OpenFindPicker()
        {
            _entityPicker = new EntityPickerDialogViewModel(() => GoToTree(_entityPicker.SelectedItem.Id), null);
            var view = new EntityPickerDialogView {DataContext = _entityPicker};
            view.ShowDialog();
        }

        private void FindById()
        {
            var findByIdViewModel = new FindTreeNodeByIdViewModel(GoToTree);
            var view = new FindTreeNodeByIdView {DataContext = findByIdViewModel};
            view.ShowDialog();
        }
    }
}