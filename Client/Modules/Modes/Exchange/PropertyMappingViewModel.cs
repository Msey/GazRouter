using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Bindings;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.Modes.Exchange
{
    public class PropertyMappingViewModel : LockableViewModel
    {
        private readonly MappingViewModel _mappingViewModel;

        public PropertyMappingViewModel(MappingViewModel mappingViewModel)
        {
            _mappingViewModel = mappingViewModel;
            LoadPeriodTypes();
        }

        #region Loading Lists
        
        private void LoadPeriodTypes()
        {
            ListPeriodTypes = ClientCache.DictionaryRepository.PeriodTypes;
            SelectedPeriodType = ListPeriodTypes.First(p => p.PeriodType == PeriodType.Twohours);
        }

        #endregion

        #region ListPeriodTypes

        private List<PeriodTypeDTO> _listPeriodTypes = new List<PeriodTypeDTO>();

        public List<PeriodTypeDTO> ListPeriodTypes
        {
            get { return _listPeriodTypes; }
            set
            {
                if (_listPeriodTypes == value) return;
                _listPeriodTypes = value;
                OnPropertyChanged(() => ListPeriodTypes);
            }
        }

        #endregion

        #region SelectedPeriodType

        private PeriodTypeDTO _selectedPeriodType;

        public PeriodTypeDTO SelectedPeriodType
        {
            get { return _selectedPeriodType; }
            set
            {
                if (SetProperty(ref _selectedPeriodType, value))
                    RefreshPropertyMappingData();
            }
        }

        #endregion

        
        #region SelectedTarget

        private TargetDTO _selectedTarget;
        private Guid? _selectedEntityId;

        public TargetDTO SelectedTarget
        {
            get { return _selectedTarget; }
            set
            {
                if (SetProperty(ref _selectedTarget, value))
                {
                    RefreshPropertyMappingData();
                }
            }
        }

        public Guid? SelectedEntityId
        {
            private get { return _selectedEntityId; }
            set {
                
                
                _selectedEntityId = value;
				if (PropertyMappingList != null) PropertyMappingList.Clear();
                RefreshPropertyMappingData();
            }
        }

        #endregion



        public void RefreshPropertyMappingData()
        {
            SelectedPropertyMappingBinding = null;
           if (!SelectedEntityId.HasValue) return;
            if (_mappingViewModel.SelectedSource != null)
                new BindingsDataProvider().GetEntityPropertyBindingsList(
                    new GetEntityPropertyBindingsParameterSet
                    {
                        SourceId = _mappingViewModel.SelectedSource.SourceId,
                        EntityId = SelectedEntityId.Value,
                        PeriodTypeId =  SelectedPeriodType.PeriodType
                    }, (dto, exception) =>
                       {
                            PropertyMappingList = dto.Select(c=>new PropertyMappingItemViewModel(c, Behavior, SelectedEntityId.Value, _mappingViewModel.SelectedSource.SourceId, SelectedPeriodType.PeriodType, SelectedTarget.Target)).ToList();
                           return exception == null;
                       }, Behavior);
        }

        #region PropertyMappingList

        private List<PropertyMappingItemViewModel> _propertyMappingList;

        public List<PropertyMappingItemViewModel> PropertyMappingList
        {
            get { return _propertyMappingList; }
            set
            {
                _propertyMappingList = value;
                OnPropertyChanged(() => PropertyMappingList);
            }
        }

        #endregion BindingsList

        #region SelectedPropertyMappingBinding

        private PropertyMappingItemViewModel _selectedPropertyMappingBinding;

        public PropertyMappingItemViewModel SelectedPropertyMappingBinding
        {
            get { return _selectedPropertyMappingBinding; }
            set
            {
               

                _selectedPropertyMappingBinding = value;

               OnPropertyChanged(() => SelectedPropertyMappingBinding);
            }
        }



        #endregion SelectedPropertyMappingBinding


    }
}