using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.CoolingUnitTypes;
using GazRouter.DTO.ObjectModel.CoolingUnit;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CoolingUnit
{
	public class AddEditCoolingUnitViewModel : AddEditViewModelBase<CoolingUnitDTO, Guid>
    {
        private readonly Guid _coolingStationId;

        public AddEditCoolingUnitViewModel(Action<Guid> actionBeforeClosing, Guid coolingStationId)
            : base(actionBeforeClosing)
        {
            _coolingStationId = coolingStationId;

            SetValidationRules();
        }

        public AddEditCoolingUnitViewModel(Action<Guid> actionBeforeClosing, CoolingUnitDTO model)
            : base(actionBeforeClosing, model)
        {
            Name = model.Name;
            SelectedCoolingUnitType = CoolingUnitTypeList.FirstOrDefault(t => t.Id == model.CoolingUnitTypeId);

            SetValidationRules();
        }

        protected override string CaptionEntityTypeName
        {
            get { return "установки охлаждения газа"; }
        }

		
		public List<CoolingUnitTypeDTO> CoolingUnitTypeList
		{
			get { return ClientCache.DictionaryRepository.CoolingUnitTypes; }
		}

		private CoolingUnitTypeDTO _type;

		public CoolingUnitTypeDTO SelectedCoolingUnitType
		{
			get { return _type; }
			set 
            { 
                _type = value; 
                OnPropertyChanged(() => SelectedCoolingUnitType); 
                SaveCommand.RaiseCanExecuteChanged(); 
            }
		}

		

        #region Commands

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)
				&& SelectedCoolingUnitType != null;
        }

        protected override Task<Guid> CreateTask
        {
	        get
	        {
		        return new ObjectModelServiceProxy().AddCoolingUnitAsync(
			        new AddCoolingUnitParameterSet
				        {
					        Name = Name,
					        ParentId = _coolingStationId,
					        CoolintUnitType = SelectedCoolingUnitType.Id
				        });
	        }
        }

        protected override Task UpdateTask
        {
            get
	        {
		        return new ObjectModelServiceProxy().EditCoolingUnitAsync(
			        new EditCoolingUnitParameterSet
				        {
					        Id = Model.Id,
					        Name = Name,
					        CoolintUnitType = SelectedCoolingUnitType.Id,
				        });
	        }
        }

        #endregion


        private void SetValidationRules()
        {
            AddValidationFor(() => Name)
                .When(() => string.IsNullOrEmpty(Name))
                .Show("Не указано наименование");

            AddValidationFor(() => SelectedCoolingUnitType)
                .When(() => SelectedCoolingUnitType == null)
                .Show("Не выбран тип установки");
        }
    }
}