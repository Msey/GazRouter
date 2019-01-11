using System;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.AggregatorTypes;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.BoilerPlants;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Aggregator
{
    public class AddEditAggregatorViewModel : AddEditViewModelBase<AggregatorDTO, Guid>
    {
        public AddEditAggregatorViewModel(Action<Guid> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            
        }
        
        public AddEditAggregatorViewModel(Action<Guid> actionBeforeClosing, AggregatorDTO model)
            : base(actionBeforeClosing, model)
        {
	        Id = model.Id;
			Name = model.Name;
        }

        protected override string CaptionEntityTypeName => "расчетного объекта";

        #region Commands

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }


        protected override Task<Guid> CreateTask
        {
            get
            {
                return new ObjectModelServiceProxy().AddAggregatorAsync(
                    new AddAggregatorParameterSet
                    {
                        Name = Name,
                        AggregatorType = AggregatorType.Custom
                    });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return new ObjectModelServiceProxy().EditAggregatorAsync(
                    new EditAggregatorParameterSet
                    {
                        Id = Model.Id,
                        Name = Name,
                        AggregatorType = AggregatorType.Custom
                    });
            }
        }

        #endregion
    }
}