using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Bindings;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.Exchange
{
    public class ObjectMappingViewModel : LockableViewModel
    {
        private readonly MappingViewModel _mainMappingVewModel;
        private List<MappingItemViewModel> _bindingList;

        public ObjectMappingViewModel(MappingViewModel mainMappingVewModel)
        {
            _bindingsDataProvider = new BindingsDataProvider();
            _mainMappingVewModel = mainMappingVewModel;
            RefreshCommand = new DelegateCommand(RefreshObjectMappingData);
            mainMappingVewModel.PropertyChanged+=MainMappingVewModel_PropertyChanged;            
            BindingList = new List<MappingItemViewModel>();
            LoadEntityTypeList();
            ConTypeVis = Visibility.Collapsed;
        }

        private void MainMappingVewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshObjectMappingData();
        }

        public List<MappingItemViewModel> BindingList
        {
            get { return _bindingList; }
            set
            {
                _bindingList = value;
                OnPropertyChanged(() => BindingList);
            }
        }

        #region SelectedBinding

        private MappingItemViewModel _selectedBinding;

        public MappingItemViewModel SelectedBinding
        {
            get { return _selectedBinding; }
            set
            {
              

                _selectedBinding = value;
   
                    _mainMappingVewModel.PropertyMapping.SelectedEntityId = _selectedBinding != null ? _selectedBinding.Model.EntityId : (Guid?) null;
              
                
                OnPropertyChanged(() => SelectedBinding);
           
             
            }
        }




        #region OtherEntityTypeList

        private List<EntityTypeDTO> _entityTypeList;

        public List<EntityTypeDTO> EntityTypeList
        {
            get { return _entityTypeList; }
            set
            {
                _entityTypeList = value;
                OnPropertyChanged(() => EntityTypeList);
            }
        }

        private void LoadEntityTypeList()
        {
            EntityTypeList = ClientCache.DictionaryRepository.EntityTypes;
           SelectedEntityType = EntityTypeList.FirstOrDefault();
            OnPropertyChanged(() => EntityTypeList);
//            OnPropertyChanged(() => SelectedEntityType);
        }

        public IEnumerable<PipelineTypesDTO> ConnectionTypeList
        {
            get { return ClientCache.DictionaryRepository.PipelineTypes.Values; }
        }


        #endregion

        #region SelectedEntityType

        private EntityTypeDTO _selectedEntityType;

        public EntityTypeDTO SelectedEntityType
        {
            get { return _selectedEntityType; }
            set
            {
                _selectedEntityType = value;
	            SelectedBinding = null;
                OnPropertyChanged(() => SelectedEntityType);
                if (value != null && value.Id == (int) EntityType.Pipeline)
                {
                    ConTypeVis =  Visibility.Visible ;
                }
                else
                {
                    ConTypeVis = Visibility.Collapsed;
                    RefreshObjectMappingData();
                }
            }
        }

        private PipelineType? _selectedPipelineType;
        public PipelineType? SelectedPipelineType
        {
            get { return _selectedPipelineType; }
            set
            {
                if (_selectedPipelineType == value) return;
                _selectedPipelineType = value;
                OnPropertyChanged(() => SelectedPipelineType);
                RefreshObjectMappingData();
            }
        }


	    public bool IsSelected
	    {
		    get { return _isSelected; }
		    set
		    {
			    _isSelected = value;
			    OnPropertyChanged(() => IsSelected);
		    }
	    }

	    private bool _isSelected;

	    #endregion SelectedEntityType

        public DelegateCommand RefreshCommand { get; private set; }
        private bool _showBindedOnly;

        public bool ShowBindedOnly
        {
            get { return _showBindedOnly; }
            set
            {
                _showBindedOnly = value;
                OnPropertyChanged(() => ShowBindedOnly);
                RefreshObjectMappingData();
            }
        }

        private Visibility _conTypeVis;
        private readonly BindingsDataProvider _bindingsDataProvider;

        public Visibility ConTypeVis
        {
            get { return _conTypeVis; }
            set
            {
                _conTypeVis = value;
                OnPropertyChanged(() => ConTypeVis);
                if (value == Visibility.Collapsed)
                {
                    SelectedPipelineType = null;
                }
            }
        }

        public void RefreshObjectMappingData()
        {
            SelectedBinding = null;
            if (SelectedEntityType == null) return;
            LoadItems();
        }


        private void LoadItems()
        {
			if (!IsSelected) return;
            BindingList = new List<MappingItemViewModel>();
            _bindingsDataProvider.GetEntityBindingsList(
            new GetEntityBindingsPageParameterSet
            {
                EntityType = SelectedEntityType.EntityType,
                ShowAll = !ShowBindedOnly,
                SourceId = _mainMappingVewModel.SelectedSource.SourceId,
                PipelineTypeId = SelectedPipelineType
                }, (result, ex) =>
                {
                    if (ex == null)
                    {
                        BindingList = result.Select(e => new MappingItemViewModel(e, Behavior, _mainMappingVewModel.SelectedSource.SourceId)).ToList();
                    }
                    return ex == null;
                }, Behavior);

        }

        #endregion SelectedBinding

    }
}
