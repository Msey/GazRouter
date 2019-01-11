using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Controls.Dialogs
{
    public class SelectEntityPropertyViewModel : EntityPropertySelectorViewModel
    {
        public SelectEntityPropertyViewModel(Action onClose, List<EntityType> allowedTypes = null, List<PhysicalType> allowedPhisicalTypes = null)
            : base(onClose, allowedTypes, allowedPhisicalTypes)
        {
            SaveCommand = new DelegateCommand(() => DialogResult = true,
                                              () => SelectedEntity != null && SelectedEntityProperty != null);
        }

        public SelectEntityPropertyViewModel() : this(null)
        {
        }

        public string ButtonSaveCaption
        {
            get { return "Ок"; }
        }

        public DelegateCommand SaveCommand { get; private set; }

        public override PropertyTypeDTO SelectedEntityProperty
        {
            get { return base.SelectedEntityProperty; }
            set
            {
                base.SelectedEntityProperty = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}