using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.SeriesData.SerieChecks;

namespace GazRouter.ManualInput.Settings.EntityProperties
{
    public class PropertyTypeItem : ViewModelBase
    {
        private readonly int _entityTypeId;
        public PropertyTypeItem(int entityTypeId, EntityTypePropertyDTO propType)
        {
            _entityTypeId = entityTypeId;
            Dto = propType;
        }

        public EntityTypePropertyDTO Dto { get; set; }

        public string Name { get; set; }

        public bool IsMandatory
        {
            get { return Dto.IsMandatory; }
            set
            {
                if (Dto.IsMandatory != value)
                {
                    Dto.IsMandatory = value;
                    new SeriesDataServiceProxy().UpdateEntityTypePropertyAsync(
                        new UpdateEntityPropertyTypeParameterSet
                        {
                            EntityTypeId = _entityTypeId,
                            PropertyTypeId = (int)Dto.PropertyType,
                            IsMandatory = value
                        });

                    // Если свойство обязательное и признак РВ не задан, то выставляет его автоматом
                    if (value && !IsInput)
                        IsInput = true;

                    OnPropertyChanged(() => IsMandatory);
                }
            }
        }

        public bool IsInput
        {
            get { return Dto.IsInput; }
            set
            {
                if (Dto.IsInput != value)
                {
                    if (!value && IsMandatory)
                    {
                        MessageBoxProvider.Alert(
                            "Данный параметр помечен как обязательный, поэтому должен присутствовать в ручном вводе.", 
                            "Некорректное действие");
                        OnPropertyChanged(() => IsInput);
                        return;
                    }


                    Dto.IsInput = value;
                    new SeriesDataServiceProxy().UpdateEntityTypePropertyAsync(
                        new UpdateEntityPropertyTypeParameterSet
                        {
                            EntityTypeId = _entityTypeId,
                            PropertyTypeId = (int)Dto.PropertyType,
                            IsInput = value
                        });

                    OnPropertyChanged(() => IsInput);
                }
            }
        }

        public bool IsEnabled { get; set; }
    }
}