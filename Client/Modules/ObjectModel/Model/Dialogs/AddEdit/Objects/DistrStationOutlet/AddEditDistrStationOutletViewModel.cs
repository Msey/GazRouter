using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using Utils.Units;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.DistrStationOutlet
{
    public class AddEditDistrStationOutletViewModel : AddEditViewModelBase<DistrStationOutletDTO>
    {
        private readonly Guid _distrStationId;
        private double _capacityRated;
        private Pressure _pressureRated;

        public AddEditDistrStationOutletViewModel(Action<Guid> actionBeforeClosing, DistrStationOutletDTO model)
            : base(actionBeforeClosing, model)
        {
			Id = model.Id;
			Name = model.Name;
			if (model.PressureRated.HasValue) PressureRated = Pressure.FromKgh(model.PressureRated.Value);
			if (model.CapacityRated.HasValue) CapacityRated = model.CapacityRated.Value; 
            _distrStationId = model.ParentId.Value;
            SetValidationRules();
            LoadConsumerList();
        }

		public AddEditDistrStationOutletViewModel(Action<Guid> actionBeforeClosing, Guid distrStationId, int sortorder)
            : base(actionBeforeClosing)
        {
            _distrStationId = distrStationId;
			_sortorder = sortorder;
            SetValidationRules();
		    LoadConsumerList();
        }

	    private readonly int _sortorder;
        public Pressure PressureRated
        {
            get { return _pressureRated; }
            set { SetProperty(ref _pressureRated, value); }
        }

        public double CapacityRated
        {
            get { return _capacityRated; }
            set { SetProperty(ref _capacityRated, value); }
        }



        public List<ConsumerDTO> ConsumerList { get; set; }

        private async void LoadConsumerList()
        {
            ConsumerList = await new ObjectModelServiceProxy().GetConsumerListAsync(
                new GetConsumerListParameterSet
                {
                    DistrStationId = _distrStationId
                });
            OnPropertyChanged(() => ConsumerList);

            SelectedConsumer = ConsumerList.SingleOrDefault(c => c.Id == Model.ConsumerId);
        }

        private ConsumerDTO _selectedConsumer;

        public ConsumerDTO SelectedConsumer
        {
            get { return _selectedConsumer; }
            set { SetProperty(ref _selectedConsumer, value); }
        }





        protected override string CaptionEntityTypeName => "выхода ГРС";

        private void SetValidationRules()
        {
            AddValidationFor(() => PressureRated)
                .When(() => ValueRangeHelper.PressureRange.IsOutsideRange(PressureRated))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            AddValidationFor(() => CapacityRated)
                .When(() => CapacityRated < 0 || CapacityRated > 2000)
                .Show("Значение должно быть в диапозоне от 0 до 2000");
        }

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

        protected override Task UpdateTask
        {
	        get
	        {
		        return new ObjectModelServiceProxy().EditDistrStationOutletAsync(
			        new EditDistrStationOutletParameterSet
                    {
					    Id = Model.Id,
					    ParentId = _distrStationId,
					    Name = Name,
					    CapacityRated = CapacityRated,
					    PressureRated = PressureRated.Kgh,
                        ConsumerId = SelectedConsumer?.Id,
                        SortOrder = Model.SortOrder
                    });
	        }
        }

        protected override Task<Guid> CreateTask
        {
	        get
	        {
		        return new ObjectModelServiceProxy().AddDistrStationOutletAsync(
			        new AddDistrStationOutletParameterSet
                    {
					    Name = Name,
					    ParentId = _distrStationId,
					    CapacityRated = CapacityRated,
					    PressureRated = PressureRated.Kgh,
                        ConsumerId = SelectedConsumer?.Id,
					    SortOrder = _sortorder
                    });
	        }
        }
    }
}