using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.ObjectDetails.Measurings;
using GazRouter.DTO.ObjectModel;


namespace GazRouter.ObjectModel.Model.Tabs.PropertyValues
{
    public class PropertiesValuesViewModel : ViewModelBase, ITabItem
    {
        public PropertiesValuesViewModel(CommonEntityDTO parentEntity)
        {
            ParentEntity = parentEntity;
            Measurings = new MeasuringsViewModel();
        }

        public CommonEntityDTO ParentEntity { get; set; }

        public MeasuringsViewModel Measurings { get; set; }

        public string Header
        {
            get { return "Параметры"; }
        }
        
        public void Refresh()
        {
            if (ParentEntity == null) return;
            Measurings.SetEntity(ParentEntity.Id, ParentEntity.EntityType);

            //Measurings = new MeasuringsViewModel(ParentEntity.Id, ParentEntity.EntityType);
            OnPropertyChanged(() => Measurings);
        }

        

        


    }
}