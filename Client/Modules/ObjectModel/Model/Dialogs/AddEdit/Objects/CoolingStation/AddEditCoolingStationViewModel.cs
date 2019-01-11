using System;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.CoolingStations;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CoolingStation
{
	public class AddEditCoolingStationViewModel : AddEditViewModelBase<CoolingStationDTO, Guid>
    {
        private readonly Guid _compStationId;

		public AddEditCoolingStationViewModel(Action<Guid> actionBeforeClosing, Guid compStationId, int sortorder)
            : base(actionBeforeClosing)
        {
            _compStationId = compStationId;
			_sortorder = sortorder;
        }

	    private int _sortorder;

        public AddEditCoolingStationViewModel(Action<Guid> actionBeforeClosing, CoolingStationDTO model)
            : base(actionBeforeClosing, model)
        {
	        Id = model.Id;
			Name = model.Name;
        }

        protected override string CaptionEntityTypeName
        {
            get { return "станции охлаждения газа"; }
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
		        return
			        new ObjectModelServiceProxy()
				        .AddCoolingStationAsync(new AddCoolingStationParameterSet
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
		        return new ObjectModelServiceProxy().EditCoolingStationAsync(new EditCoolingStationParameterSet
			                                                                     {
				                                                                     Id = Model.Id,
				                                                                     Name = Name
			                                                                     });
	        }
        }

        #endregion
    }
}