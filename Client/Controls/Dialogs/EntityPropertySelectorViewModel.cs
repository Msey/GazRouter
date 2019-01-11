using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Controls.Dialogs
{
	public class EntityPropertySelectorViewModel : EntityPickerDialogViewModel
	{
        public EntityPropertySelectorViewModel(Action onClose, List<EntityType> allowedTypes = null, List<PhysicalType> allowedPhisicalTypes = null)
			: base(onClose, allowedTypes)
        {
            AllowedPhisicalTypes = allowedPhisicalTypes ?? new List<PhysicalType>();

            LoadPeriodTypes();

			OnSelectedItemsChanges = () =>
			{
				SelectedEntity = SelectedItem;
			    if (SelectedEntity != null)
			    {
			        EntitySelector = new EntityPropertySelector
			        {
			            Entity = SelectedEntity,
			            PeriodType = SelectedPeriodType.PeriodType
			        };
			    }
			    else
			    {
			        EntitySelector = null;
			    }
			};
			SelectCommand = new DelegateCommand(() => DialogResult = true,
												() => SelectedEntity != null && SelectedEntityProperty != null);
		}

		public EntityPropertySelectorViewModel() : this(null)
        {
        }

        public List<PhysicalType> AllowedPhisicalTypes { get; private set; }

        private EntityPropertySelector _entitySelector = new EntityPropertySelector();

        public EntityPropertySelector EntitySelector
        {
            get { return _entitySelector; }
            set
            {
                if (_entitySelector == value) return;
                _entitySelector = value;
                OnPropertyChanged(() => EntitySelector);
            }
        }

        public class EntityPropertySelector
        {
            public CommonEntityDTO Entity;
            public PropertyType PropertyType;
            public PeriodType? PeriodType;
            public Target? TargetType;
        }

        #region Loading Lists
        
        private void LoadPeriodTypes()
        {
            ListPeriodTypes = ClientCache.DictionaryRepository.PeriodTypes;
            SelectedPeriodType = ListPeriodTypes.First(p => p.PeriodType == PeriodType.Twohours);
        }

        #endregion

        #region EntityProperty

        private CommonEntityDTO _selectedEntity;

        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (_selectedEntity == value) return;
                _selectedEntity = value;
                if (_selectedEntity != null)
                {
                    SelectedEntityProperty = null;
                    var allEntityProperties =
                        ClientCache.DictionaryRepository.EntityTypes.Single(
                            p => p.EntityType == _selectedEntity.EntityType)
                            .EntityProperties;

                    EntityProperties = AllowedPhisicalTypes.Count > 0
                        ? allEntityProperties.Where(p => AllowedPhisicalTypes.Contains(p.PhysicalType.PhysicalType)).ToList()
                        : allEntityProperties;
                }
                else
                {
                    EntityProperties = new List<PropertyTypeDTO>();
                }

                IsEnabledPropertyBox = EntityProperties.Count > 0;
                IsEnabledPropertyOtherBox = false;
                SelectedEntityProperty = null;
                OnPropertyChanged(() => SelectedEntity);
            }
        }

        private List<PropertyTypeDTO> _entityProperties;

        public List<PropertyTypeDTO> EntityProperties
        {
            get { return _entityProperties; }
            set
            {
                if (_entityProperties == value) return;
                
                _entityProperties = value;
                IsEnabledPropertyBox = true;
                OnPropertyChanged(() => EntityProperties);
                OnPropertyChanged(() => IsEnabledPropertyBox);
            }
        }

        #endregion

        #region SelectedEntityProperty

        private PropertyTypeDTO _selectedEntityProperty;

        public virtual PropertyTypeDTO SelectedEntityProperty
        {
            get { return _selectedEntityProperty; }
            set
            {
                if (_selectedEntityProperty == value) return;
                _selectedEntityProperty = value;
				if (_selectedEntityProperty != null)
					EntitySelector.PropertyType = _selectedEntityProperty.PropertyType;
                if(_selectedEntityProperty == null)
                {
                    IsEnabledPropertyOtherBox = false;
                }
                else
                {
                    IsEnabledPropertyOtherBox = true;
                }
                OnPropertyChanged(() => SelectedEntityProperty);
            }
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
                if (_selectedPeriodType == value) return;
                _selectedPeriodType = value;
				EntitySelector.PeriodType = SelectedPeriodType.PeriodType;
                OnPropertyChanged(() => SelectedPeriodType);
            }
        }

        #endregion

        #region IsEnabled

        private bool _isEnabledPropertyBox;

        public bool IsEnabledPropertyBox
        {
            get { return _isEnabledPropertyBox; }
            set
            {
                if (_isEnabledPropertyBox == value) return;
                _isEnabledPropertyBox = value;
                OnPropertyChanged(() => IsEnabledPropertyBox);
            }
        }

        private bool _isEnabledPropertyOtherBox;

        public bool IsEnabledPropertyOtherBox
        {
            get { return _isEnabledPropertyOtherBox; }
            set
            {
                if (_isEnabledPropertyOtherBox == value) return;
                _isEnabledPropertyOtherBox = value;
                OnPropertyChanged(() => IsEnabledPropertyOtherBox);
            }
        }

        #endregion

        public string Caption
        {
            get { return string.Format("Выбор свойств"); }
        }
    }
}
