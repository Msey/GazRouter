using System;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.PowerPlants;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.PowerPlant
{
    public class AddEditPowerPlantViewModel : AddEditViewModelBase<PowerPlantDTO, Guid>
    {
        private readonly Guid _compStationId;

		public AddEditPowerPlantViewModel(Action<Guid> actionBeforeClosing, Guid compStationId, int sortorder)
            : base(actionBeforeClosing)
        {
            _compStationId = compStationId;
			_sortorder = sortorder;
        }

	    private int _sortorder;

        public AddEditPowerPlantViewModel(Action<Guid> actionBeforeClosing, PowerPlantDTO model)
            : base(actionBeforeClosing, model)
        {
			Id = model.Id;
			Name = model.Name;
        }

        protected override string CaptionEntityTypeName
        {
            get { return "ЭСН"; }
        }

        #region Commands

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

		protected override Task UpdateTask
		{
			get
			{
				return new ObjectModelServiceProxy()
					.EditPowerPlantAsync(new EditPowerPlantParameterSet
					{
						Id = Model.Id,
						Name = Name,
					});
			}
		}

		protected override Task<Guid> CreateTask
		{
			get
			{
				return new ObjectModelServiceProxy()
					.AddPowerPlantAsync(new AddPowerPlantParameterSet
						                    {
							                    Name = Name,
							                    ParentId = _compStationId,
							                    SortOrder = _sortorder
						                    });
			}
		}

        #endregion
    }
}