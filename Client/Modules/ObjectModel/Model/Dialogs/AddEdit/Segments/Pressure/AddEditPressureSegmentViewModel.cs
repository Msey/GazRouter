using System;
using System.Threading.Tasks;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Pressure
{
    public class AddEditPressureSegmentViewModel : AddEditSegmentViewModelBase<PressureSegmentDTO>
    {
      
        #region Constr

        public AddEditPressureSegmentViewModel(Action<int> actionBeforeClosing, PressureSegmentDTO model, PipelineDTO pipeline)
            : base(actionBeforeClosing, model, pipeline)
        {
	        Id = model.Id;
			KilometerOfEndPoint = model.KilometerOfEndPoint;
			KilometerOfStartPoint = model.KilometerOfStartPoint;
			Pressure =Utils.Units.Pressure.FromKgh( model.Pressure);
            SetValidationRules();
        }

        public AddEditPressureSegmentViewModel(Action<int> actionBeforeClosing, PipelineDTO pipeline)
            : base(actionBeforeClosing, pipeline)
        {
            
            SetValidationRules();
        }

        #endregion

        #region Pressure

        private Utils.Units.Pressure? _pressure;

        public Utils.Units.Pressure? Pressure
        {
            get { return _pressure; }
            set
            {
                if (SetProperty(ref _pressure, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region SaveCommand

        

        protected async override void LoadSegmentList()
        {
            var list = await new ObjectModelServiceProxy().GetPressureSegmentListAsync(Pipeline.Id);
            GetSegmentListCallback(list);
        }

        protected override Task<int> CreateTask
        {
            get
            {
                return new ObjectModelServiceProxy()
                       .AddPressureSegmentAsync(
                           new AddPressureSegmentParameterSet
                           {
                               PipelineId = Pipeline.Id,
                               KilometerOfStartPoint = KilometerOfStartPoint.Value,
                               KilometerOfEndPoint = KilometerOfEndPoint.Value,
                               Pressure = Pressure.Value.Kgh
                           });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                ValidateAll();
                if (!HasErrors)
                {
                    return new ObjectModelServiceProxy()
                            .EditPressureSegmentAsync(
                            new EditPressureSegmentParameterSet
                            {
                                SegmentId = Model.Id,
                                PipelineId = Pipeline.Id,
                                KilometerOfStartPoint = KilometerOfStartPoint.Value,
                                KilometerOfEndPoint = KilometerOfEndPoint.Value,
                                Pressure = Pressure.Value.Kgh
                            });
                }
                return null;
            }
        }

        #endregion SaveCommand

        protected override void SetValidationRules()
        {
            AddValidationFor(() => Pressure)
                .When(() => !Pressure.HasValue)
                .Show("Введите давление");

            AddValidationFor(() => Pressure)
                .When(
                    () =>
                        Pressure.HasValue &&
                        (ValueRangeHelper.PressureRange.IsOutsideRange(Pressure.Value)))
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);

            base.SetValidationRules();

        }
    }
}