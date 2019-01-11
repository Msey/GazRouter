using System;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.BoilerPlants;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.BoilerPlant
{
    public class AddEditBoilerPlantViewModel : AddEditViewModelBase<BoilerPlantDTO, Guid>
    {
        private readonly Guid _compStationId;

		public AddEditBoilerPlantViewModel(Action<Guid> actionBeforeClosing, Guid compStationId, int sortorder)
            : base(actionBeforeClosing)
        {
            _compStationId = compStationId;
			_sortorder = sortorder;
        }

	    private int _sortorder;

        public AddEditBoilerPlantViewModel(Action<Guid> actionBeforeClosing, BoilerPlantDTO model)
            : base(actionBeforeClosing, model)
        {
	        Id = model.Id;
			Name = model.Name;
        }

        protected override string CaptionEntityTypeName
        {
            get { return "котельной"; }
        }

        #region Commands

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }


        protected override Task<Guid> CreateTask
        {
            get
            {
                return new ObjectModelServiceProxy()
                    .AddBoilerPlantAsync(new AddBoilerPlantParameterSet
                    {
                        Name = Name,
                        ParentId = _compStationId,
                        SortOrder = _sortorder
                    });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return new ObjectModelServiceProxy()
                    .EditBoilerPlantAsync(new EditBoilerPlantParameterSet
                    {
                        Id = Model.Id,
                        Name = Name
                    });
            }
        }

        #endregion
    }
}