using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.ObjectDetails.Attachments;
using GazRouter.Controls.Tree;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Infrastructure.Faults;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.ObjectModel.Model.Dialogs;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Comment;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Valve;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Booster;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Branch;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Bridge;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.InletOutletCompShop;
using GazRouter.ObjectModel.Model.Tabs.ChangeLog;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Chamber;

namespace GazRouter.ObjectModel.Model.Pipelines
{
    public class EditableFullTreeViewModel : LockableViewModel
    {
        private ValidatedTreeViewModelPipeline _treeModel;

        private bool EditPermission { get; }


    

        public EditableFullTreeViewModel()
        {
            EditPermission = Authorization2.Inst.IsEditable(LinkType.ObjectModel);

            DeleteEntityCommand = new DelegateCommand(DeleteEntity, 
                ()=> CanDeleteCommandExecute() && EditPermission);
            RefreshCommand = new DelegateCommand(Refresh);
            AddCommentCommand = new DelegateCommand(AddComment, () => TreeModel.SelectedNode is EntityNode &&
                                                      TreeModel.SelectedNode.Entity.EntityType != EntityType.Enterprise && 
                                                      EditPermission);


            AddPipelineCommand = new DelegateCommand<PipelineType?>(AddPipeLine, CanAddPipeline);
            AddValveCommand = new DelegateCommand(AddValve, () => TreeModel.SelectedNode != null);
            AddBoilerCommand = new DelegateCommand(AddBoiler, () => TreeModel.SelectedNode != null);
            AddPowerUnitCommand = new DelegateCommand(AddPowerUnit, () => TreeModel.SelectedNode != null);

            /*
            UpSortOrderCommand = new DelegateCommand(UpSetOrder,
                                                     () =>
                                                     TreeModel.SelectedNode is EntityNode && TreeModel.SelectedNode.Parent != null && TreeModel.SelectedNode.Entity.EntityType != EntityType.Valve &&
                                                     TreeModel.SelectedNode.Parent.Children.OfType<EntityNode>()
                                                              .Where(
                                                                  t =>
                                                                  t.SortOrder <
                                                                  ((EntityNode)TreeModel.SelectedNode).SortOrder)
                                                              .OrderByDescending(t => t.SortOrder)
                                                              .FirstOrDefault() != null && EditPermission);
            DownSortOrderCommand = new DelegateCommand(DownSetOrder,
                                                       () =>
                                                       TreeModel.SelectedNode is EntityNode && TreeModel.SelectedNode.Parent != null && TreeModel.SelectedNode.Entity.EntityType != EntityType.Valve &&
                                                       TreeModel.SelectedNode.Parent.Children.OfType<EntityNode>()
                                                                .Where(
                                                                    t =>
                                                                    t.SortOrder >
                                                                    ((EntityNode)TreeModel.SelectedNode).SortOrder)
                                                                .OrderBy(t => t.SortOrder)
                                                                .FirstOrDefault() != null && EditPermission);
            */
            UpSortOrderCommand = new DelegateCommand(UpSetOrder, () => CanMoveUp() && EditPermission);
            DownSortOrderCommand = new DelegateCommand(DownSetOrder, () => CanMoveDown() && EditPermission);

            Attachments = new AttachmentsViewModel();
            ChangeLog = new ChangeLogViewModel();
            AddMenuVisible = EditPermission;
        }

        private bool CanDeleteCommandExecute()
        {
            return (TreeModel.SelectedNode is EntityNode ) && !(TreeModel.SelectedNode.Entity is PipelineDTO && ((PipelineDTO)TreeModel.SelectedNode.Entity).Type == PipelineType.Main);
        }

        private bool CanMoveUp()
        {
            var selectedNode = TreeModel.SelectedNode as EntityNode;
            if (selectedNode == null || selectedNode.SortOrder == 0 || selectedNode.SortOrder == Int32.MaxValue) return false;

            var elList = (selectedNode.Parent != null ? selectedNode.Parent.Children : TreeModel.Nodes);
            int selectedIndex = elList.IndexOf(selectedNode);

            var prevNode = selectedIndex != 0 ? selectedNode.Parent.Children[selectedIndex - 1] as EntityNode : null;

            return selectedIndex > 0 && prevNode.SortOrder != 0;
        }

        private bool CanMoveDown()
        {

            var selectedNode = TreeModel.SelectedNode as EntityNode;
            if (selectedNode == null || selectedNode.SortOrder == 0 || selectedNode.SortOrder == Int32.MaxValue) return false;

            var elList = (selectedNode.Parent != null ? selectedNode.Parent.Children : TreeModel.Nodes);
            int selectedIndex = elList.IndexOf(selectedNode);

            var nextNode = selectedIndex < elList.Count - 1 ? selectedNode.Parent.Children[selectedIndex + 1] as EntityNode : null;

            return selectedIndex < elList.Count - 1 && nextNode.SortOrder != Int32.MaxValue;
        }

        public AttachmentsViewModel Attachments { get; }
        public ChangeLogViewModel ChangeLog { get; }

        public List<MenuItemViewModel> AddEnitityItems
        {
            get
            {
                var pipelineFolderNode = TreeModel.SelectedNode as PipelineFolderNode;
                if (pipelineFolderNode != null && pipelineFolderNode.PipelineType != PipelineType.Main)
                {
                    return
                        new List<MenuItemViewModel>
                                    {
                                        new AddPipelineMenuItem(AddPipelineCommand, pipelineFolderNode.PipelineType)
                                    };
                }
				var selectedFolderNode = TreeModel.SelectedNode as FolderNode;
	            if (selectedFolderNode != null)
	            {
                    switch (selectedFolderNode.EntityType)
	                {
                        case EntityType.Valve:
	                        return new List<MenuItemViewModel>
	                                   {
	                                       new AddEntityMenuItem(AddValveCommand, selectedFolderNode.EntityType)
	                                   };

                        case EntityType.Boiler:
	                        return new List<MenuItemViewModel>
	                                   {
	                                       new AddEntityMenuItem(AddBoilerCommand, selectedFolderNode.EntityType)
	                                   };

                        case EntityType.PowerUnit:
                            return new List<MenuItemViewModel>
	                                   {
	                                       new AddEntityMenuItem(AddPowerUnitCommand, selectedFolderNode.EntityType)
	                                   };
	                }
                    
	            }

	            var entityNode = TreeModel.SelectedNode as EntityNode;
                if (entityNode != null && entityNode.Entity.EntityType == EntityType.Pipeline)
                {
                    var items = new List<MenuItemViewModel>
                        {
                            new AddPipelineMenuItem(AddPipelineCommand, PipelineType.Bridge),
                            new AddPipelineMenuItem(AddPipelineCommand, PipelineType.Booster),
                            new AddPipelineMenuItem(AddPipelineCommand, PipelineType.Branch),
                            new AddPipelineMenuItem(AddPipelineCommand, PipelineType.RefiningDeviceChamber),
                            new AddPipelineMenuItem(AddPipelineCommand, PipelineType.CompressorShopInlet),
                            new AddPipelineMenuItem(AddPipelineCommand, PipelineType.CompressorShopOutlet),
                            new AddEntityMenuItem(AddValveCommand, EntityType.Valve),
                            new AddEntityMenuItem(AddBoilerCommand, EntityType.Boiler),
                            new AddEntityMenuItem(AddPowerUnitCommand, EntityType.PowerUnit)
                        };
                    return items;

                }

                return new List<MenuItemViewModel>();
            }
        }
        public DelegateCommand<PipelineType?> AddPipelineCommand { get; private set; }
        public DelegateCommand AddValveCommand { get; private set; }
        public DelegateCommand AddBoilerCommand { get; private set; }
        public DelegateCommand AddPowerUnitCommand { get; private set; }

        public DelegateCommand AddCommentCommand { get; private set; }

        public DelegateCommand UpdateEntityCommand
        {
            get { return _updateEntityCommand ?? (
                    _updateEntityCommand = new DelegateCommand(UpdateEntity, 
                    ()=> CanUpdateEnitityCommandExecuted() && EditPermission)); }
        }

        private bool CanAddPipeline(PipelineType? type)
        {
            return TreeModel.SelectedNode != null && (type != PipelineType.Main); 
        }

        private bool CanUpdateEnitityCommandExecuted()
        {
            return (TreeModel.SelectedNode is EntityNode) ;
        }

        public DelegateCommand DeleteEntityCommand { get; }

        public DelegateCommand RefreshCommand { get; }

		public DelegateCommand UpSortOrderCommand { get; }

		public DelegateCommand DownSortOrderCommand { get; }
        
        public List<MenuItemCommand> FindCommands { get; set; }
        public List<MenuItemCommand> ValidateCommands { get; set; }


        public void AddComment()
        {
            var vm = new AddEditCommentViewModel(
                Refresh,
                TreeModel.SelectedNode.Entity.Id,
                TreeModel.SelectedNode.Entity.Description);
            var v = new AddEditCommentView { DataContext = vm };
            v.ShowDialog();
        }
        
        public ValidatedTreeViewModelPipeline TreeModel
        {
            get
            {
                if (_treeModel == null)
                {
                    _treeModel = new ValidatedTreeViewModelPipeline();
                    _treeModel.PropertyChanged += TreeModelPropertyChanged;
                    Refresh();
                }
                return _treeModel;
            }
        }

        private void RefreshCommands()
        {
            EnableAddButton = TreeModel.SelectedNode != null && AddEnitityItems.Count > 0 && EditPermission;
            EnableEditButton = TreeModel.SelectedNode is EntityNode;

            AddPipelineCommand.RaiseCanExecuteChanged();
            AddValveCommand.RaiseCanExecuteChanged();
            AddBoilerCommand.RaiseCanExecuteChanged();
            AddPowerUnitCommand.RaiseCanExecuteChanged();

            AddCommentCommand.RaiseCanExecuteChanged();

            RefreshCommand.RaiseCanExecuteChanged();
            UpdateEntityCommand.RaiseCanExecuteChanged();
            DeleteEntityCommand.RaiseCanExecuteChanged();
            UpSortOrderCommand.RaiseCanExecuteChanged();
            DownSortOrderCommand.RaiseCanExecuteChanged();
        }

        private bool _enableAddButton;
	    public bool EnableAddButton
	    {
            get { return _enableAddButton; }
		    set
		    {
                _enableAddButton = value;
                OnPropertyChanged(() => EnableAddButton);
		    }
	    }

        private bool _enableEditButton;
        public bool EnableEditButton
        {
            get { return _enableEditButton; }
            set
            {
                _enableEditButton = value;
                OnPropertyChanged(() => EnableEditButton);
            }
        }

        private Guid? _newEntityId;
        private bool _addNewEntity;

        private void AddValve()
        {
            var pipelineDto = (PipelineDTO)TreeModel.SelectedNode.Entity;
            var addEditLinearValveViewModel = new AddEditValveViewModel(id =>
            {
                _newEntityId = id;
                _addNewEntity = true;
                Refresh();
            }, pipelineDto, /*GetSortOrder(entityType)*/0);
            var view = new AddEditValveView { DataContext = addEditLinearValveViewModel };
            view.ShowDialog();
        }

        private void AddBoiler()
        {
            var entity = TreeModel.SelectedNode.Entity;
            DialogHelper.AddBoiler(id =>
            {
                _newEntityId = id;
                _addNewEntity = true;
                Refresh();
            }, entity);
        }

        private void AddPowerUnit()
        {
            var entity = TreeModel.SelectedNode.Entity;
            DialogHelper.AddPowerUnit(id =>
            {
                _newEntityId = id;
                _addNewEntity = true;
                Refresh();
            }, entity);

        }

		private int? GetSortOrder(EntityType? entityType)
		{
			if (TreeModel.SelectedNode.Children.OfType<EntityNode>().Any(p => p.Entity.EntityType == entityType))
				return TreeModel.SelectedNode.Children.OfType<EntityNode>().Max(t => t.SortOrder) + 1;
			foreach (var nodeBase in TreeModel.SelectedNode.Children)
			{
				var node = nodeBase as FolderNode;
				if (node != null && node.Children.OfType<EntityNode>().Any(p => p.Entity.EntityType == entityType))
					return node.Children.OfType<EntityNode>().Max(t => t.SortOrder) + 1;
			}
			return 1;
		}

        private void AddPipeLine(PipelineType? newPipelineType)
        {
            PipelineDTO parentPipeline = null;
            var node = TreeModel.SelectedNode as FolderNode;
            if (node != null)
            {
                if (node.Parent != null)
                {
                    parentPipeline = (PipelineDTO) node.Parent.Entity;
                }
            }
            else
            {
                parentPipeline = (PipelineDTO)TreeModel.SelectedNode.Entity;
            }

            switch (newPipelineType)
            {
                    case PipelineType.Bridge:
                    var pipelineBridgeViewModel = new AddEditPipelineBridgeViewModel(id =>
                    {
                        _newEntityId = id;
                        _addNewEntity = true;
                        Refresh();
                    }, newPipelineType.Value, parentPipeline, SelectedGasTransport.Id, (int)GetSortOrder(EntityType.Pipeline));
                    var pipelineBridgeView = new AddEditPipelineBridgeView { DataContext = pipelineBridgeViewModel };
                    pipelineBridgeView.ShowDialog();
                    break;

                    case PipelineType.Branch:
                    var pipelineBranchViewModel = new AddEditPipelineBranchViewModel(id =>
                        {
                            _newEntityId = id;
                            _addNewEntity = true;
                            Refresh();
                        }, newPipelineType.Value, parentPipeline, SelectedGasTransport.Id, (int)GetSortOrder(EntityType.Pipeline));
                    var pipelineBranchView = new AddEditPipelineBranchView { DataContext = pipelineBranchViewModel };
                    pipelineBranchView.ShowDialog();
                    break;

                    case PipelineType.CompressorShopInlet:
                    var compressorShopInletViewModel = new AddEditPipelineInletOutletCompShopViewModel(id =>
                        {
                            _newEntityId = id;
                            _addNewEntity = true;
                            Refresh();
                        }, newPipelineType.Value, parentPipeline, SelectedGasTransport.Id, (int)GetSortOrder(EntityType.Pipeline));
                        var compressorShopInletView = new AddEditPipelineInletOutletCompShopView { DataContext = compressorShopInletViewModel };
                    compressorShopInletView.ShowDialog();
                    break;

                    case PipelineType.CompressorShopOutlet:
                    var compressorShopOutletViewModel = new AddEditPipelineInletOutletCompShopViewModel(id =>
			            {
			                _newEntityId = id;
			                _addNewEntity = true;
			                Refresh();
                        }, newPipelineType.Value, parentPipeline, SelectedGasTransport.Id, (int)GetSortOrder(EntityType.Pipeline));
                    var compressorShopOutletView = new AddEditPipelineInletOutletCompShopView { DataContext = compressorShopOutletViewModel };
                    compressorShopOutletView.ShowDialog();
                    break;

                    case PipelineType.Booster:
                    var pipelineBoosterViewModel = new AddEditPipelineBoosterViewModel(id =>
                    {
                        _newEntityId = id;
                        _addNewEntity = true;
                        Refresh();
                    }, parentPipeline, SelectedGasTransport.Id, (int)GetSortOrder(EntityType.Pipeline));
                    var pipelineBoosterView = new AddEditPipelineBoosterView { DataContext = pipelineBoosterViewModel };
                    pipelineBoosterView.ShowDialog();
                    break;

                    case PipelineType.RefiningDeviceChamber:
                    var pipelineChamberViewModel = new AddEditPipelineChamberViewModel(id =>
                    {
                        _newEntityId = id;
                        _addNewEntity = true;
                        Refresh();
                    }, newPipelineType.Value, parentPipeline, SelectedGasTransport.Id, (int)GetSortOrder(EntityType.Pipeline));
                    var pipelineChamberView = new AddEditPipelineChamberView { DataContext = pipelineChamberViewModel };
                    pipelineChamberView.ShowDialog();
                    break;

                default:
			var addEditPipelineViewModel = new AddEditPipelineViewModel(id =>
			{
			    _newEntityId = id;
			    _addNewEntity = true;
			    Refresh();
            }, newPipelineType.Value, parentPipeline, SelectedGasTransport.Id, (int)GetSortOrder(EntityType.Pipeline));
                    var view = new AddEditPipelineView {DataContext = addEditPipelineViewModel};
			view.ShowDialog();
                    break;
		}
		}

        private bool _isDeleteCommand;

        private void DeleteEntity()
        {
            MessageBoxProvider.Confirm("Удалить объект?",
             
                 async (e) =>
                {
                    if (e)
                    {
                        var entityNode = TreeModel.SelectedNode as EntityNode;
                        if (entityNode != null)
                        {
                            try
                            {
                                Behavior.TryLock();
                                await new ObjectModelServiceProxy().DeleteEntityAsync(
                                    new DeleteEntityParameterSet
                                    {
                                        Id = entityNode.Entity.Id,
                                        EntityType = entityNode.Entity.EntityType
                                    });
                                _isDeleteCommand = true;
                                Refresh();
                            }
                            catch (FaultException<FaultDetail> ex)
                            {
                                if (ex.Detail.FaultType == FaultType.IntegrityConstraint)
                                {
                                    MessageBoxProvider.Alert(ex.Detail.Message, "Невозможно удалить объект");
                                }
                            }
                            finally
                            {
                                Behavior.TryUnlock();
                            }
                        }
                    }
                }, "Удаление объекта",
                cancelButtonText: "Отмена",
                okButtonText:  "Удалить");
        }
        
        

        private void UpdateEntity()
        {
            var entityNode = (EntityNode) TreeModel.SelectedNode;
            switch (entityNode.Entity.EntityType)
            {
                case EntityType.Pipeline:
                {
                    switch (((PipelineDTO)entityNode.Entity).Type)
                    {
                        case PipelineType. Bridge:
                            DialogHelper.EditPipelineBridge(id => Refresh(), (PipelineDTO)entityNode.Entity, (PipelineDTO)entityNode.Parent.Entity);
                            break;

                        case PipelineType.Branch:
                            DialogHelper.EditPipelineBranch(id => Refresh(), (PipelineDTO)entityNode.Entity, (PipelineDTO)entityNode.Parent.Entity);
                            break;

                        case PipelineType.Booster:
                            DialogHelper.EditBoosterPipeline(id => Refresh(), (PipelineDTO)entityNode.Entity, (PipelineDTO)entityNode.Parent.Entity);
                            break;

                        case PipelineType.CompressorShopInlet:
                            DialogHelper.EditInletOutletPipelineCompShop(id => Refresh(), (PipelineDTO)entityNode.Entity, (PipelineDTO)entityNode.Parent.Entity);
                            break;

                        case PipelineType.CompressorShopOutlet:
                            DialogHelper.EditInletOutletPipelineCompShop(id => Refresh(), (PipelineDTO)entityNode.Entity, (PipelineDTO)entityNode.Parent.Entity);
                            break;
                        case PipelineType.RefiningDeviceChamber:
                            DialogHelper.EditPipelineChamber(id => Refresh(), (PipelineDTO)entityNode.Entity, (PipelineDTO)entityNode.Parent.Entity);
                            break;

                            default:
                            DialogHelper.EditPipeline(id => Refresh(), (PipelineDTO)entityNode.Entity);
                            break;
                    }
                    break;
                }
                case EntityType.Valve:
                {
                    DialogHelper.EditLinearValve(id => Refresh(), (ValveDTO) entityNode.Entity,
                        (PipelineDTO) entityNode.Parent.Entity);
                    break;
                }
                case EntityType.Boiler:
                {
                    DialogHelper.EditBoiler(id => Refresh(), (BoilerDTO) entityNode.Entity, entityNode.Parent.Entity);
                    break;
                }
                case EntityType.PowerUnit:
                {
                    DialogHelper.EditPowerUnit(id => Refresh(), (PowerUnitDTO) entityNode.Entity,
                        entityNode.Parent.Entity);
                    break;
                }
            }
        }

        private void TreeModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
	        if (e.PropertyName != "SelectedNode") return;
			OnPropertyChanged(() => AddMenuVisible);
			OnPropertyChanged(() => AddEnitityItems);

	        RefreshCommands();

            var entityId = TreeModel.SelectedNode is EntityNode ? TreeModel.SelectedNode.Entity.Id : (Guid?)null;
            Attachments.EntityId = entityId;
            ChangeLog.EntityId = entityId;
        }

        //		public bool AddMenuVisible => AddEnitityItems.Count > 0;

        private bool _addMenuVisible;
        public bool AddMenuVisible
        {
            get
            {
                _addMenuVisible = (AddEnitityItems.Count > 0) && EditPermission;
                return _addMenuVisible;
            }
            set
            {
                _addMenuVisible = value;
            }
        }


        private void FillTree(TreeDataDTO data)
        {
            if(_addNewEntity)
            {
                _addNewEntity = false;
            }
            else
            {
                _newEntityId = null;
            }

            if (_isDeleteCommand)
            {
                TreeModel.FillTree(data, _newEntityId, _isDeleteCommand);
                _isDeleteCommand = false;
            }
            else
            {
                TreeModel.FillTree(data, _newEntityId);
            }
        }

        public async void Refresh()
        {
            if (GetSelectedGasTransport == null)
                return;

            Behavior.TryLock();
            var filter = new EntityTreeGetParameterSet
            {
                Filter = EntityFilter.Pipelines | EntityFilter.LinearValves | EntityFilter.Boilers | EntityFilter.PowerUnits,
                SystemId = GetSelectedGasTransport().Id
            };
            if (!UserProfile.Current.Site.IsEnterprise)
                filter.SiteId = UserProfile.Current.Site.Id;
            var getTreeTask = new ObjectModelServiceProxy().GetFullTreeAsync(filter);
            var getInconsistenciesTask = new ObjectModelServiceProxy().GetInconsistenciesAsync(null);
            await TaskEx.WhenAll(getTreeTask, getInconsistenciesTask);
            TreeModel.Inconsistencies = getInconsistenciesTask.Result;
            FillTree(getTreeTask.Result);
            Behavior.TryUnlock();
        }


		#region ListGasTransportSystems

		private List<GasTransportSystemDTO> _listGasTransportSystems = new List<GasTransportSystemDTO>();

		public List<GasTransportSystemDTO> ListGasTransportSystems
		{
			get { return _listGasTransportSystems; }
			set
			{
				if (_listGasTransportSystems == value) return;
				_listGasTransportSystems = value;
				OnPropertyChanged(() => ListGasTransportSystems);
			}
		}

		#endregion

		#region SelectedGasTransport

		public GasTransportSystemDTO SelectedGasTransport
		{
			get
			{
				return GetSelectedGasTransport();
			}
			set
			{
				if (value==null) return;
				SetSelectedGasTransport(this, value);
				Refresh();
				OnPropertyChanged(() => SelectedGasTransport);
			}
		}
		public Action<LockableViewModel, GasTransportSystemDTO> SetSelectedGasTransport;
	    public Func<GasTransportSystemDTO> GetSelectedGasTransport;
        private  DelegateCommand _updateEntityCommand ;

        #endregion

		private void UpSetOrder()
		{
			UpdateOrder(true);
		}

		private async void UpdateOrder(bool up)
		{
			if (TreeModel.SelectedNode.Parent == null) return;
			var source = (EntityNode)TreeModel.SelectedNode;
			var fullparent = TreeModel.SelectedNode.Parent;
			EntityNode parent = up
									? fullparent.Children.OfType<EntityNode>()
												.Where(t => t.SortOrder < source.SortOrder)
												.OrderByDescending(t => t.SortOrder)
												.FirstOrDefault()
									: fullparent.Children.OfType<EntityNode>()
												.Where(t => t.SortOrder > source.SortOrder)
												.OrderBy(t => t.SortOrder)
												.FirstOrDefault();
			if (parent == null) return;
			var sortorder = source.SortOrder;
			source.SortOrder = parent.SortOrder;
			parent.SortOrder = sortorder;

		    await new ObjectModelServiceProxy().SetSortOrderAsync(
		        new SetSortOrderParameterSet
		        {
		            Id = source.Entity.Id,
		            SortOrder = source.SortOrder
		        });

		    await new ObjectModelServiceProxy().SetSortOrderAsync(
		        new SetSortOrderParameterSet
		        {
		            Id = parent.Entity.Id,
		            SortOrder = parent.SortOrder,
		        });
			var index = TreeModel.SelectedNode.Parent.Children.IndexOf(source);
			fullparent.Children.Remove(parent);
			fullparent.Children.Insert(index, parent);
			UpSortOrderCommand.RaiseCanExecuteChanged();
			DownSortOrderCommand.RaiseCanExecuteChanged();
		}

		private void DownSetOrder()
		{
			UpdateOrder(false);
		}
    }
}
