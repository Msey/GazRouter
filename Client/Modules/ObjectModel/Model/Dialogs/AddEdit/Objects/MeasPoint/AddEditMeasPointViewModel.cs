using System;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.MeasPoint;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.MeasPoint
{
    public class AddEditMeasPointViewModel : AddEditViewModelBase<MeasPointDTO>
    {
        private double _chromatographConsumptionRate;
        private int _chromatographTestTime;
        private CommonEntityDTO _parent;
        private int _sortOrder;
        
        public AddEditMeasPointViewModel(Action<Guid> actionBeforeClosing, MeasPointDTO model)
            : base(actionBeforeClosing, model)
        {
			Id = model.Id;
			Name = model.Name;
            ChromatographConsumptionRate = model.ChromatographConsumptionRate;
            ChromatographTestTime = model.ChromatographTestTime; 
            
            SetValidationRules();
        }

        public AddEditMeasPointViewModel(Action<Guid> actionBeforeClosing, CommonEntityDTO parent, int sortOrder)
            : base(actionBeforeClosing)
        {
            _parent = parent;
            _sortOrder = sortOrder;
            SetValidationRules();
        }

	    
        public double ChromatographConsumptionRate
        {
            get { return _chromatographConsumptionRate; }
            set { SetProperty(ref _chromatographConsumptionRate, value); }
        }

        public int ChromatographTestTime
        {
            get { return _chromatographTestTime; }
            set { SetProperty(ref _chromatographTestTime, value); }
        }

        protected override string CaptionEntityTypeName
        {
            get { return "точки измерения параметров газа"; }
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => ChromatographConsumptionRate)
                .When(() => ChromatographConsumptionRate < 0 || ChromatographConsumptionRate > 10)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 10)");

            AddValidationFor(() => ChromatographTestTime)
                .When(() => ChromatographTestTime < 0 || ChromatographTestTime > 60)
                .Show("Недопустимое значение (интервал допустимых значений от 1 до 60)");
        }

        protected override bool OnSaveCommandCanExecute()
        {
            return !HasErrors;
        }

        protected override Task UpdateTask
        {
	        get
	        {
		        return new ObjectModelServiceProxy().EditMeasPointAsync(
			        new EditMeasPointParameterSet
				        {
                            Id = Id,
                            ChromatographConsumptionRate = ChromatographConsumptionRate,
                            ChromatographTestTime = ChromatographTestTime
				        });
	        }
        }

        protected override Task<Guid> CreateTask
        {
	        get
	        {
		        return new ObjectModelServiceProxy().AddMeasPointAsync(
			        new AddMeasPointParameterSet
				        {
					        Name = "Точка измерения параметров газа",
                            CompShopId = _parent.EntityType == EntityType.CompShop ? _parent.Id : (Guid?)null,
                            DistrStationId = _parent.EntityType == EntityType.DistrStation ? _parent.Id : (Guid?)null,
                            MeasLineId = _parent.EntityType == EntityType.MeasLine ? _parent.Id : (Guid?)null,
                            ChromatographConsumptionRate = ChromatographConsumptionRate,
                            ChromatographTestTime = ChromatographTestTime,
                            SortOrder = _sortOrder
				        });
	        }
        }
    }
}