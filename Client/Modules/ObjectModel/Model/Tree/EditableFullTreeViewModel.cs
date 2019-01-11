using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.ObjectDetails.Attachments;
using GazRouter.Controls.Dialogs.ObjectDetails.Bindings;
using GazRouter.Controls.Dialogs.ObjectDetails.Calculations;
using GazRouter.Controls.Dialogs.ObjectDetails.Urls;
using GazRouter.Controls.Tree;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Infrastructure.Faults;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.ObjectModel.Model.Dialogs;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Comment;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompUnit;
using GazRouter.ObjectModel.Model.Tabs.ChangeLog;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Telerik.Windows.Controls;
using AddEditCommentView = GazRouter.ObjectModel.Model.Dialogs.AddEdit.Comment.AddEditCommentView;
using AddEditCompUnitView = GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompUnit.AddEditCompUnitView;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.ObjectModel.Model.Tree
{
    public class EditableFullTreeViewModel : LockableViewModel
    {
        private TreeViewModelPointObjects _treeModel;
        private readonly DelegateCommand<EntityType?> _addCommand;
        public bool EditPermission { get; set; }

        public EditableFullTreeViewModel()
        {
            EditPermission = Authorization2.Inst.IsEditable(LinkType.ObjectModel);
            _updateEntityCommand = new DelegateCommand(UpdateEntity, CanUpdateEntityExecute);
            CopyEntityCommand = new DelegateCommand(CopyEntity,
                                                    () =>
                                                    (TreeModel.SelectedNode is EntityNode &&
                                                     TreeModel.SelectedNode.Entity.EntityType == EntityType.CompUnit) && EditPermission);
	        DeleteEntityCommand = new DelegateCommand(DeleteEntity,
	                                                  () =>
	                                                  (TreeModel.SelectedNode is EntityNode &&
                                                       TreeModel.SelectedNode.Entity.EntityType != EntityType.Enterprise) && EditPermission);
            RefreshCommand = new DelegateCommand(() => Refresh());

            AddCommentCommand = new DelegateCommand(AddComment, () => (TreeModel.SelectedNode is EntityNode &&
                                                      TreeModel.SelectedNode.Entity.EntityType != EntityType.Enterprise) && EditPermission);


	        MoveUpCommand = new DelegateCommand(MoveUp, () => CanMoveUp() && EditPermission);
	        MoveDownCommand = new DelegateCommand(ModeDown, () => CanMoveDown() && EditPermission);
			
            CreateFilterItems();

            _addCommand = new DelegateCommand<EntityType?>(AddEntity, a => CanAdd(a) && EditPermission);

            Attachments = new AttachmentsViewModel();
            ChangeLog = new ChangeLogViewModel();
            Urls = new UrlsViewModel();
            Bindings = new BindingsViewModel();
            Calculations = new CalculationsViewModel();
        }

        private bool CanUpdateEntityExecute()
        {
            return TreeModel.SelectedNode is EntityNode && 
                   TreeModel.SelectedNode.Entity.EntityType != EntityType.Enterprise && 
                   EditPermission;
        }

        private bool CanMoveDown()
        {
            var selectedNode = TreeModel.SelectedNode as EntityNode;
            if (selectedNode == null) return false;
            var elList = (selectedNode.Parent != null ? selectedNode.Parent.Children : TreeModel.Nodes);
            return elList.IndexOf(selectedNode) < elList.Count-1;
        }

        private bool CanMoveUp()
        {
           var selectedNode =TreeModel.SelectedNode as EntityNode;
            if (selectedNode == null) return false;

            var elList = (selectedNode.Parent != null ? selectedNode.Parent.Children : TreeModel.Nodes);
            return elList.IndexOf(selectedNode) > 0;
        }

        
	    private void CreateFilterItems()
        {
            FilterItems = new List<FilterMenuItemCommand>
                {
                    new FilterMenuItemCommand(RefreshFilter, EntityType.CompStation)
                        {Header = "КС", InUse = true},
                    new FilterMenuItemCommand(RefreshFilter, EntityType.DistrStation)
                        {Header = "ГРС", InUse = true},
                    new FilterMenuItemCommand(RefreshFilter, EntityType.MeasStation)
                        {Header = "ГИС", InUse = true},
                    new FilterMenuItemCommand(RefreshFilter, EntityType.ReducingStation)
                        {Header = "ПРГ", InUse = true}
                };
        }

        public List<FilterMenuItemCommand> FilterItems { get; private set; }

        public List<AddEntityMenuItem> AddEnitityItems
        {
            get
            {
                if (TreeModel.SelectedNode == null)
                {
                    return new List<AddEntityMenuItem>();
                }

                var selectedFolderNode = TreeModel.SelectedNode as FolderNode;
                if (selectedFolderNode != null)
                {
                    return new List<AddEntityMenuItem>
                    {
                        new AddEntityMenuItem(_addCommand, selectedFolderNode.EntityType)
                    };
                }

                var entityNode = TreeModel.SelectedNode as EntityNode;
                if (entityNode != null)
                    switch (entityNode.Entity.EntityType)
                    {
                        case EntityType.Enterprise:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.Site)
                                       };
                        case EntityType.Site:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.CompStation),
                                           new AddEntityMenuItem(_addCommand, EntityType.DistrStation),
                                           new AddEntityMenuItem(_addCommand, EntityType.MeasStation),
                                           new AddEntityMenuItem(_addCommand, EntityType.ReducingStation),
                                           new AddEntityMenuItem(_addCommand, EntityType.OperConsumer)
                                       };
                        case EntityType.CompStation:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.CompShop),
                                           new AddEntityMenuItem(_addCommand, EntityType.CoolingStation),
										   new AddEntityMenuItem(_addCommand, EntityType.BoilerPlant),
										   new AddEntityMenuItem(_addCommand, EntityType.PowerPlant),
                                       };
                        case EntityType.DistrStation:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.MeasPoint),
                                           new AddEntityMenuItem(_addCommand, EntityType.DistrStationOutlet),
                                           new AddEntityMenuItem(_addCommand, EntityType.Consumer),
                                           new AddEntityMenuItem(_addCommand, EntityType.Boiler)
                                       };
                        case EntityType.MeasStation:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.MeasLine),
										   new AddEntityMenuItem(_addCommand, EntityType.Boiler)
                                       };
                        case EntityType.CompShop:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.CompUnit),
                                           new AddEntityMenuItem(_addCommand, EntityType.MeasPoint)
                                       };
                        case EntityType.MeasLine:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.MeasPoint)
                                       };
                        case EntityType.BoilerPlant:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.Boiler)
                                       };
                        case EntityType.PowerPlant:
                            return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.PowerUnit)
                                       };
                        case EntityType.CoolingStation:
							return new List<AddEntityMenuItem>
                                       {
                                           new AddEntityMenuItem(_addCommand, EntityType.CoolingUnit)
                                       };
                    }

                return new List<AddEntityMenuItem>();
            }
        }


		public bool AddMenuVisible => AddEnitityItems.Count > 0;


        public DelegateCommand UpdateEntityCommand
        {
            get {
                return _updateEntityCommand ?? (_updateEntityCommand = new DelegateCommand(UpdateEntity,
                    () => CanUpdateEntityExecute() && 
                    EditPermission)); }
        }

        public DelegateCommand CopyEntityCommand { get; private set; }

        public DelegateCommand DeleteEntityCommand { get; private set; }

        public DelegateCommand AddCommentCommand { get; private set; }

        public DelegateCommand MoveUpCommand { get; private set; }

		public DelegateCommand MoveDownCommand { get; private set; }

        public DelegateCommand RefreshCommand { get; private set; }

		

        public List<MenuItemCommand> FindCommands { get; set; }
        public List<MenuItemCommand> ValidateCommands { get; set; }


        public AttachmentsViewModel Attachments { get; set; }
        public ChangeLogViewModel ChangeLog { get; set; }
        public UrlsViewModel Urls { get; set; }

        public BindingsViewModel Bindings { get; set; }
        public CalculationsViewModel Calculations { get; set; }


        public void AddComment()
        {
            var vm = new AddEditCommentViewModel(
                () => Refresh(), 
                TreeModel.SelectedNode.Entity.Id, 
                TreeModel.SelectedNode.Entity.Description);
            var v = new AddEditCommentView {DataContext = vm};
            v.ShowDialog();
        }

        public TreeViewModelPointObjects TreeModel
        {
            get
            {
                if (_treeModel == null)
                {
                    _treeModel = new TreeViewModelPointObjects();
                    _treeModel.PropertyChanged += TreeModelPropertyChanged;
                    Refresh();
                }
                return _treeModel;
            }
        }

        private bool CanAdd(EntityType? arg)
        {
            return arg.HasValue &&
                   (arg.Value != EntityType.MeasPoint ||
                    TreeModel.SelectedNode.Children.OfType<EntityNode>()
                        .All(n => n.Entity.EntityType != EntityType.MeasPoint));
        }

        private void RefreshCommands()
        {
            EnableAddButton = TreeModel.SelectedNode != null && 
                              AddEnitityItems.Count > 0 && 
                              EditPermission;
            EnableEditButton = TreeModel.SelectedNode is EntityNode &&
                              EditPermission;
            RefreshCommand.RaiseCanExecuteChanged();
            UpdateEntityCommand.RaiseCanExecuteChanged();
            DeleteEntityCommand.RaiseCanExecuteChanged();
            AddCommentCommand.RaiseCanExecuteChanged();
            CopyEntityCommand.RaiseCanExecuteChanged();
			MoveUpCommand.RaiseCanExecuteChanged();
			MoveDownCommand.RaiseCanExecuteChanged();
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

        private void RefreshFilter(EntityType entityType)
        {
			Fill();
        }

        private void AddEntity(EntityType? entityType)
        {
            switch (entityType.Value)
            {
                case EntityType.ReducingStation:
                    {
                        DialogHelper.AddReducingStationTree(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.Site:
                    {
                        DialogHelper.AddSite(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, SelectedGasTransport.Id, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.CompStation:
                    {
                        DialogHelper.AddCompStation(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.CompShop:
                    {
                        DialogHelper.AddCompShop(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.CompUnit:
                    {
                        DialogHelper.AddCompUnit(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, ((CompShopDTO)TreeModel.SelectedNode.Entity).EngineClass);
                        break;
                    }
                case EntityType.DistrStation:
                    {
                        DialogHelper.AddDistrStation(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, SelectedGasTransport.Id);
                        break;
                    }
                case EntityType.Consumer:
                    {
                        DialogHelper.AddConsumer(id => Refresh(id), TreeModel.SelectedNode.Entity.Id);
                        break;
                    }
                case EntityType.OperConsumer:
                    {
                        DialogHelper.AddOperConsumer(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, SelectedGasTransport.Id);
                        break;
                    }
                case EntityType.MeasStation:
                    {
                        DialogHelper.AddMeasStation(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, SelectedGasTransport.Id,
                            GetSortOrder(entityType));
                        break;
                    }
                case EntityType.MeasLine:
                    {
                        DialogHelper.AddMeasLine(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.MeasPoint:
                    {
                        DialogHelper.AddMeasPoint(id => Refresh(id), TreeModel.SelectedNode.Entity, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.DistrStationOutlet:
                    {
                        DialogHelper.AddOutlet(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
                
                case EntityType.CoolingStation:
                {
                    DialogHelper.AddCoolingStation(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
				case EntityType.Boiler:
					{
                        DialogHelper.AddBoiler(id => Refresh(id), TreeModel.SelectedNode.Entity);
						break;
					}
				case EntityType.PowerUnit:
					{
                        DialogHelper.AddPowerUnit(id => Refresh(id), TreeModel.SelectedNode.Entity);
						break;
					}
                case EntityType.BoilerPlant:
                    {
                        DialogHelper.AddBoilerPlant(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.PowerPlant:
                    {
                        DialogHelper.AddPowerPlant(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
                        break;
                    }
                case EntityType.CoolingUnit:
					{
                        DialogHelper.AddCoolingUnit(id => Refresh(id), TreeModel.SelectedNode.Entity.Id, GetSortOrder(entityType));
						break;
					}
            }
        }

        private void DeleteEntity()
        {
          MessageBoxProvider.Confirm("Удалить объект?", async confirmed =>
          {
              if (!confirmed) return;
              try
              {
                  Behavior.TryLock();
                  await new ObjectModelServiceProxy().DeleteEntityAsync(
                      new DeleteEntityParameterSet
                      {
                          Id = TreeModel.SelectedNode.Entity.Id,
                          EntityType = TreeModel.SelectedNode.Entity.EntityType
                      });
                  _isDeleteCommand = true;
                  Refresh();
              }
              catch (GazRouter.Common.ServerException ex)
              {
                  if (ex.InnerException != null && ex.InnerException.GetType() == typeof(System.ServiceModel.FaultException<GazRouter.DTO.Infrastructure.Faults.FaultDetail>) && (((System.ServiceModel.FaultException<GazRouter.DTO.Infrastructure.Faults.FaultDetail>)(ex.InnerException))).Detail.FaultType == GazRouter.DTO.Infrastructure.Faults.FaultType.IntegrityConstraint)
                  {
                      MessageBoxProvider.Alert("Невозможно удалить объект: " + ex.Message, "Невозможно удалить объект");
                  }

              }
              catch (FaultException<FaultDetail> ex)
              {
                  if (ex.Detail.FaultType == FaultType.IntegrityConstraint)
                  {
                      MessageBoxProvider.Alert("Невозможно удалить объект: " + ex.Detail.Message, "Невозможно удалить объект");
                  }
              }
              finally
              {
                  Behavior.TryUnlock();
              }
          }, "Удаление объекта", "Удалить","Отмена");

        }

        private bool _isDeleteCommand;

        private void UpdateEntity()
        {
            var entityNode = (EntityNode) TreeModel.SelectedNode;
            switch (entityNode.Entity.EntityType)
            {
                case EntityType.Site:
                    {
                        DialogHelper.EditSite(id => Refresh(), (SiteDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.CompStation:
                    {
                        DialogHelper.EditCompStation(id => Refresh(), (CompStationDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.CompShop:
                    {
                        DialogHelper.EditCompShop(id => Refresh(), (CompShopDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.CompUnit:
                    {
                        DialogHelper.EditCompUnit(id => Refresh(), (CompUnitDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.DistrStation:
                    {
                        DialogHelper.EditDistrStation(id => Refresh(), (DistrStationDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.Consumer:
                    {
                        DialogHelper.EditConsumer(id => Refresh(), (ConsumerDTO)entityNode.Entity);
                        break;
                    }
                case EntityType.MeasStation:
                    {
                        DialogHelper.EditMeasStation(id => Refresh(), (MeasStationDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.ReducingStation:
                    {
                        DialogHelper.EditReducingStationTree(id => Refresh(),
                                                               (ReducingStationDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.MeasLine:
                    {
                        DialogHelper.EditMeasLine(id => Refresh(), (MeasLineDTO) entityNode.Entity);
                        break;
                    }
                case EntityType.DistrStationOutlet:
                    {
                        DialogHelper.EditOutlet(id => Refresh(), (DistrStationOutletDTO) entityNode.Entity);
                        break;
                    }
                
                case EntityType.CoolingStation:
                    {
                        DialogHelper.EditCoolingStation(id => Refresh(), (CoolingStationDTO) entityNode.Entity);
                        break;
                    }
				case EntityType.CoolingUnit:
					{
						DialogHelper.EditCoolingUnit(id => Refresh(), (CoolingUnitDTO)entityNode.Entity);
						break;
					}
				case EntityType.Boiler:
		            {
                        DialogHelper.EditBoiler(id => Refresh(), (BoilerDTO)entityNode.Entity, entityNode.Parent.Entity);
						break;
					}
                case EntityType.PowerUnit:
                    {
                        DialogHelper.EditPowerUnit(id => Refresh(), (PowerUnitDTO)entityNode.Entity, entityNode.Parent.Entity);
                        break;
                    }
                case EntityType.BoilerPlant:
                    {
                        DialogHelper.EditBoilerPlant(id => Refresh(), (BoilerPlantDTO)entityNode.Entity);
                        break;
                    }
                case EntityType.PowerPlant:
                    {
                        DialogHelper.EditPowerPlant(id => Refresh(), (PowerPlantDTO)entityNode.Entity);
                        break;
                    }
                case EntityType.MeasPoint:
                    {
                        DialogHelper.EditMeasPoint(id => Refresh(), (MeasPointDTO)entityNode.Entity);
                        break;
                    }
                case EntityType.OperConsumer:
                    {
                        DialogHelper.EditOperConsumer(id => Refresh(), (OperConsumerDTO)entityNode.Entity);
                        break;
                    }
            }
        }

		private void MoveUp()
		{
			UpdateOrder(true);
		}

	    private void UpdateOrder(bool moveUp)
	    {
		//	if (TreeModel.SelectedNode.Parent == null) return;
            var selectedNode = (EntityNode)TreeModel.SelectedNode;
	        var elList = (selectedNode.Parent != null ? selectedNode.Parent.Children : TreeModel.Nodes);
	        var index = elList.IndexOf(selectedNode);

//			var source = (EntityNode)TreeModel.SelectedNode;
//			var fullparent = TreeModel.SelectedNode.Parent;
	        var neighborIndex = moveUp ? index - 1 : index + 1;
	        EntityNode neighbor = (EntityNode) elList[neighborIndex];
			if (neighbor == null) return;
            var sortorder = selectedNode.SortOrder;
            selectedNode.SortOrder = neighbor.SortOrder;
			neighbor.SortOrder = sortorder;
	        if (neighbor.SortOrder == selectedNode.SortOrder)
	        {
	            if (moveUp)
	                selectedNode.SortOrder--;
	            else
	                selectedNode.SortOrder++;
	        }
			new ObjectModelServiceProxy().SetSortOrderAsync(
				new SetSortOrderParameterSet
				{
                    Id = selectedNode.Entity.Id,
                    SortOrder = selectedNode.SortOrder,
				});
            new ObjectModelServiceProxy().SetSortOrderAsync(
				new SetSortOrderParameterSet
				{
					Id = neighbor.Entity.Id,
					SortOrder = neighbor.SortOrder,
				});
            elList.Remove(neighbor);
            elList.Insert(index, neighbor);
			MoveUpCommand.RaiseCanExecuteChanged();
			MoveDownCommand.RaiseCanExecuteChanged();
	    }

	    private void ModeDown()
	    {
		    UpdateOrder(false);
	    }

		private int GetSortOrder(EntityType? entityType)
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

        private void CopyEntity()
        {
            // Реализовано только для ГПА
            var compUnitDTO = (CompUnitDTO)TreeModel.SelectedNode.Entity;
            var compUnitType = ClientCache.DictionaryRepository.CompUnitTypes.First(p => p.Id == compUnitDTO.CompUnitTypeId);
            var vm = new AddEditCompUnitViewModel(id => Refresh(), compUnitDTO.ParentId.Value, compUnitType.EngineClassId)
            {
                SelectedCompUnitType =
                    ClientCache.DictionaryRepository.CompUnitTypes.SingleOrDefault(
                        ut => ut.Id == compUnitDTO.CompUnitTypeId),
                SelectedSuperchargerType =
                    ClientCache.DictionaryRepository.SuperchargerTypes.SingleOrDefault(
                        st => st.Id == compUnitDTO.SuperchargerTypeId),
                IsVirtual = compUnitDTO.IsVirtual,
                BleedingRate = compUnitDTO.BleedingRate,
                InjectionProfileVolume = compUnitDTO.InjectionProfileVolume,
                TurbineStarterConsumption = compUnitDTO.TurbineStarterConsumption,
                DryMotoringConsumption = compUnitDTO.DryMotoringConsumption,
                HasRecoveryBoiler = compUnitDTO.HasRecoveryBoiler
            };

            var v = new AddEditCompUnitView {DataContext = vm};
            v.ShowDialog();

        }

        

        private void TreeModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode")
            {
				OnPropertyChanged(() => AddMenuVisible);
                OnPropertyChanged(() => AddEnitityItems);
                RefreshCommands();

                var entityId = TreeModel.SelectedNode is EntityNode ? TreeModel.SelectedNode.Entity.Id : (Guid?)null;
                Attachments.EntityId = entityId;
                ChangeLog.EntityId = entityId;
                Urls.EntityId = entityId;
                Bindings.EntityId = entityId;
                Calculations.EntityId = entityId;
            }
        }

        private TreeData _entities;
        private void FillTree(TreeDataDTO data)
        {
            _entities = new TreeData
            {
                Enterprises = ClientCache.DictionaryRepository.Enterprises,
                Sites = data.Sites,
                CompStations = data.CompStations,
                CompShops = data.CompShops,
                CompUnits = data.CompUnits,
                DistrStations = data.DistrStations,
                MeasStations = data.MeasStations,
                ReducingStations = data.ReducingStations,
                MeasLines = data.MeasLines,
                DistrStationOutlets = data.DistrStationOutlets,
                Consumers = data.Consumers,
                MeasPoints = data.MeasPoints,
                CoolingStations = data.CoolingStations,
				CoolingUnits = data.CoolingUnits,
                PowerPlants = data.PowerPlants,
				PowerUnits = data.PowerUnits,
                BoilerPlants = data.BoilerPlants,
				Boilers = data.Boilers,
                OperConsumers = data.OperConsumers
            };
            Fill();
        }

		private void Fill()
		{

            var showCompStations = FilterItems.Any(fi => fi.InUse && fi.Type == EntityType.CompStation);
            var showDistrStations = FilterItems.Any(fi => fi.InUse && fi.Type == EntityType.DistrStation);
            var showMeasStations = FilterItems.Any(fi => fi.InUse && fi.Type == EntityType.MeasStation);
            var showReducingStations = FilterItems.Any(fi => fi.InUse && fi.Type == EntityType.ReducingStation);
            
            var entities = new TreeData
		    {
                Enterprises = _entities.Enterprises,
                Sites = _entities.Sites,
                CompStations = showCompStations ? _entities.CompStations : new List<CompStationDTO>(),
                CompShops = _entities.CompShops,
                CompUnits = _entities.CompUnits,
                DistrStations = showDistrStations ? _entities.DistrStations : new List<DistrStationDTO>(),
                MeasStations = showMeasStations ? _entities.MeasStations : new List<MeasStationDTO>(),
                ReducingStations = showReducingStations ? _entities.ReducingStations : new List<ReducingStationDTO>(),
                MeasLines = _entities.MeasLines,
                DistrStationOutlets = _entities.DistrStationOutlets,
                Consumers = _entities.Consumers,
                MeasPoints = _entities.MeasPoints,
                CoolingStations = _entities.CoolingStations,
				CoolingUnits = _entities.CoolingUnits,
                PowerPlants = _entities.PowerPlants,
				PowerUnits = _entities.PowerUnits,
                BoilerPlants = _entities.BoilerPlants,
				Boilers = _entities.Boilers,
                OperConsumers = _entities.OperConsumers
		    };

            TreeModel.FillTree(entities, _isDeleteCommand, _newEntityId);
            _isDeleteCommand = false;
           
		}

        private Guid? _newEntityId;

        public async void Refresh(Guid? newEntityId = null)
        {
	        if (GetSelectedGasTransport == null)
                return;

            _newEntityId = newEntityId;
            Behavior.TryLock();
            var task =
                new ObjectModelServiceProxy().GetFullTreeAsync(new EntityTreeGetParameterSet
                {
                    Filter = EntityFilter.Sites |
                             EntityFilter.CompStations |
                             EntityFilter.CompShops |
                             EntityFilter.CompUnits |
                             EntityFilter.DistrStations |
                             EntityFilter.MeasStations |
                             EntityFilter.ReducingStations |
                             EntityFilter.MeasLines |
                             EntityFilter.DistrStationOutlets |
                             EntityFilter.Consumers |
                             EntityFilter.MeasPoints |
                             EntityFilter.CoolingStations |
                             EntityFilter.CoolingUnits |
                             EntityFilter.PowerPlants |
                             EntityFilter.PowerUnits |
                             EntityFilter.BoilerPlants |
                             EntityFilter.Boilers |
                             EntityFilter.OperConsumers,
                    SystemId = GetSelectedGasTransport().Id
                });
            var data = await task;
            FillTree(data);
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
				if (value == null) return;
				SetSelectedGasTransport(this, value);
				Refresh();
				OnPropertyChanged(() => SelectedGasTransport);
			}
		}

		public Action<LockableViewModel, GasTransportSystemDTO> SetSelectedGasTransport;
	    public Func<GasTransportSystemDTO> GetSelectedGasTransport;
        private  DelegateCommand _updateEntityCommand;

        #endregion
    }
}