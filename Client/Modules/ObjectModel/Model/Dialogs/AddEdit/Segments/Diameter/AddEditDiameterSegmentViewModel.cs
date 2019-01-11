using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.Diameters;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments.Diameter
{
    public sealed class AddEditDiameterSegmentViewModel : AddEditSegmentViewModelBase<DiameterSegmentDTO>
    {
        private DiameterDTO _selectedDiameter;
        private ExternalDiameterDTO _selectedExternalDiameter;
        private List<ValveDTO> _valveList = new List<ValveDTO>();
        private List<DiameterDTO> _diameterList;
        private List<ExternalDiameterDTO> _externalDiameterList;

        public AddEditDiameterSegmentViewModel(Action<int> actionBeforeClosing, DiameterSegmentDTO model,
            PipelineDTO pipeline)
            : base(actionBeforeClosing, model, pipeline)
        {
            Id = model.Id;
            KilometerOfEndPoint = model.KilometerOfEndPoint;
            KilometerOfStartPoint = model.KilometerOfStartPoint;
            DiameterList = ClientCache.DictionaryRepository.Diameters;
            ExternalDiameterList = ClientCache.DictionaryRepository.ExternalDiameters;
            SelectedDiameter = DiameterList.FirstOrDefault(p => p.Id == model.DiameterId);
            UpdateExternalDiameters();
            SelectedExternalDiameter = ExternalDiameterList.FirstOrDefault(p => p.Id == model.ExternalDiameterId);

            LoadValveList();
            SetValidationRules();

            PropertyChanged += UpdateExternalDiameters;
        }

        public AddEditDiameterSegmentViewModel(Action<int> actionBeforeClosing, PipelineDTO pipeline)
            : base(actionBeforeClosing, pipeline)
        {
            DiameterList = ClientCache.DictionaryRepository.Diameters;
            ExternalDiameterList = ClientCache.DictionaryRepository.ExternalDiameters;

            LoadValveList();
            SetValidationRules();

            PropertyChanged += UpdateExternalDiameters;
        }

        public List<DiameterDTO> DiameterList
        {
            get { return _diameterList; }
            set { SetProperty(ref _diameterList, value); }
        }

        public List<ExternalDiameterDTO> ExternalDiameterList
        {
            get { return _externalDiameterList; }
            set { SetProperty(ref _externalDiameterList, value); }
        }

        public DiameterDTO SelectedDiameter
        {
            get { return _selectedDiameter; }
            set
            {
                if (SetProperty(ref _selectedDiameter, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ExternalDiameterDTO SelectedExternalDiameter
        {
            get { return _selectedExternalDiameter; }
            set
            {
                if (SetProperty(ref _selectedExternalDiameter, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        protected override Task<int> CreateTask => new ObjectModelServiceProxy().AddDiameterSegmentAsync(
            new AddDiameterSegmentParameterSet
            {
                PipelineId = Pipeline.Id,
                KilometerOfStartPoint = KilometerOfStartPoint.Value,
                KilometerOfEndPoint = KilometerOfEndPoint.Value,
                DiameterId = SelectedDiameter.Id,
                ExternalDiameterId = SelectedExternalDiameter.Id,
            });

        protected override Task UpdateTask
        {
            get
            {
                ValidateAll();
                if (!HasErrors)
                {
                    return new ObjectModelServiceProxy().EditDiameterSegmentAsync(
                        new EditDiameterSegmentParameterSet
                        {
                            SegmentId = Model.Id,
                            PipelineId = Pipeline.Id,
                            KilometerOfStartPoint = KilometerOfStartPoint.Value,
                            KilometerOfEndPoint = KilometerOfEndPoint.Value,
                            DiameterId = SelectedDiameter.Id,
                            ExternalDiameterId = SelectedExternalDiameter.Id,
                        });
                }
                return null;
            }
        }

        protected override async void LoadSegmentList()
        {
            try
            {
                Behavior.TryLock();
                var list = await new ObjectModelServiceProxy().GetDiameterSegmentListAsync(Pipeline.Id);
                GetSegmentListCallback(list);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        protected override bool OnSaveCommandCanExecute()
        {
            CheckDiameter();
            return base.OnSaveCommandCanExecute();
        }

        protected override void SetValidationRules()
        {
            AddValidationFor(() => SelectedDiameter)
                .When(() => SelectedDiameter == null)
                .Show("Укажите диаметр сегмента");

            AddValidationFor(() => SelectedExternalDiameter)
                .When(() => SelectedExternalDiameter == null)
                .Show("Укажите внешний диаметр сегмента");

            base.SetValidationRules();
        }

        private async void LoadValveList()
        {
            try
            {
                _valveList =
                    await
                        new ObjectModelServiceProxy().GetValveListAsync(new GetValveListParameterSet
                        {
                            PipelineId = Pipeline.Id
                        });
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void CheckDiameter()
        {
            if (SelectedDiameter == null)
            {
                return;
            }

            if (
                _valveList
                    .Where(v => v.Kilometer >= KilometerOfStartPoint && v.Kilometer <= KilometerOfEndPoint)
                    .Any(
                        v =>
                            ClientCache.DictionaryRepository.ValveTypes.Single(vt => vt.Id == v.ValveTypeId)
                                .DiameterConv != SelectedDiameter.DiameterConv))
            {
                WarningMessage =
                    "На указанном сегменте установлены краны с диаметром отличном от диаметра сегмента. Возможно вы указали неверный диаметр сегмента. Если диаметр сегмента указан правильно, то необходимо проверить и исправить диаметры кранов.";
                IsWarningMessageVisible = true;
            }
        }

        private void UpdateExternalDiameters(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SelectedDiameter))
            {
                UpdateExternalDiameters();
            }
        }

        private void UpdateExternalDiameters()
        {
            ExternalDiameterList = ClientCache.DictionaryRepository.ExternalDiameters.Where(ed => ed.InternalDiameterId == SelectedDiameter.Id).ToList();

            if (!ExternalDiameterList.Contains(SelectedExternalDiameter))
                SelectedExternalDiameter = null;
        }
    }
}