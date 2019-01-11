using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using Microsoft.Practices.Prism;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Segments
{
    public abstract class AddEditSegmentViewModelBase<TEnt> : AddEditLinearObjectViewModel<TEnt, int>
        where TEnt : BaseSegmentDTO, new()
    {
        #region Constr

        protected AddEditSegmentViewModelBase(Action<int> closeCallback, PipelineDTO pipeline)
            : base(closeCallback)
        {
            Pipeline = pipeline;
            LoadSegmentList();
            KilometerOfStartPoint = pipeline.KilometerOfStartPoint;
            KilometerOfEndPoint = pipeline.KilometerOfEndPoint;
            SaveCommand.RaiseCanExecuteChanged();
        }

        protected AddEditSegmentViewModelBase(Action<int> closeCallback, TEnt model, PipelineDTO pipeline)
            : base(closeCallback, model)
        {
            SegmentId = model.Id;
            Pipeline = pipeline;
            LoadSegmentList();
        }

        #endregion

        public ObservableCollection<TEnt> SegmentList = new ObservableCollection<TEnt>();
        protected PipelineDTO Pipeline { get; set; }
        protected int SegmentId { get; set; }


        #region WarningMessage

        private string _warningMessage;
        private bool _isWarningMessageVisible;

        public bool IsWarningMessageVisible
        {
            get { return _isWarningMessageVisible; }
            set
            {
                if (SetProperty(ref _isWarningMessageVisible, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string WarningMessage
        {
            get { return _warningMessage; }
            set
            {
                SetProperty(ref _warningMessage, value);
            }
        }

        #endregion


        protected override string CaptionEntityTypeName
        {
            get { return "сегмента"; }
        }
        

        protected virtual void LoadSegmentList()
        {
        }

        protected bool GetSegmentListCallback(IEnumerable<TEnt> segment)
        {
            if (!IsEdit)
            {
                SegmentList.AddRange(segment);
                CheckKilometer();
            }
            else
            {
                SegmentList.AddRange(segment.Where(s => s.Id != SegmentId).ToList());
                CheckKilometer();
            }
            return true;
        }

        protected bool CheckKilometer()
        {
            foreach (var segment in SegmentList.Where(s => s.Id != SegmentId))
            {
                if (Between(KilometerOfStartPoint.Value, segment.KilometerOfStartPoint, segment.KilometerOfEndPoint)
                    || Between(KilometerOfEndPoint.Value, segment.KilometerOfStartPoint, segment.KilometerOfEndPoint)
                    || Between(segment.KilometerOfStartPoint, KilometerOfStartPoint.Value, KilometerOfEndPoint.Value)
                    || Between(segment.KilometerOfEndPoint, KilometerOfStartPoint.Value, KilometerOfEndPoint.Value))
                {
                    WarningMessage = "Сегмент с указанными километрами начала и окончания пересекается с другими уже существующими сегментами. Возможно введены некорректные значения километров начала и окончания сегмента. Если значения километров введены верно, то необходимо исправить другие сегменты, чтобы устранить пересечение.";
                    IsWarningMessageVisible = true;
                    return false;        
                }
            }
            return true;
        }

        private static bool Between(double p, double b, double e)
        {
            return p > b && p < e;
        }

        protected virtual void SetValidationRules()
        {
            AddValidationFor(() => KilometerOfStartPoint)
                .When(() => !KilometerOfStartPoint.HasValue)
                .Show("Не задан километр начала сегмента");

            AddValidationFor(() => KilometerOfStartPoint)
                .When(() => KilometerOfStartPoint.HasValue && KilometerOfEndPoint.HasValue && KilometerOfStartPoint.Value >= KilometerOfEndPoint.Value)
                .Show("Километр начала сегмента должен быть меньше километра окончания сегмента");

            AddValidationFor(() => KilometerOfStartPoint)
                .When(() => KilometerOfStartPoint.HasValue
                    && (KilometerOfStartPoint.Value < Pipeline.KilometerOfStartPoint
                    || KilometerOfStartPoint.Value > Pipeline.KilometerOfEndPoint))
                .Show(string.Format("Недопустимое значение километра начала сегмента ({0} - {1})",
                Pipeline.KilometerOfStartPoint, Pipeline.KilometerOfEndPoint));



            AddValidationFor(() => KilometerOfEndPoint)
                .When(() => !KilometerOfEndPoint.HasValue)
                .Show("Не задан километр начала сегмента");

            AddValidationFor(() => KilometerOfEndPoint)
                .When(() => KilometerOfStartPoint.HasValue && KilometerOfEndPoint.HasValue && KilometerOfStartPoint.Value >= KilometerOfEndPoint.Value)
                .Show("Километр окончания сегмента должен быть больше километра начала сегмента");


            AddValidationFor(() => KilometerOfEndPoint)
                .When(() => KilometerOfEndPoint.HasValue
                    && (KilometerOfEndPoint.Value < Pipeline.KilometerOfStartPoint
                    || KilometerOfEndPoint.Value > Pipeline.KilometerOfEndPoint))
                .Show(string.Format("Недопустимое значение километра окончания сегмента ({0} - {1})",
                Pipeline.KilometerOfStartPoint, Pipeline.KilometerOfEndPoint));



        }

        //protected abstract bool CheckKilometer();

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            if (!HasErrors)
                CheckKilometer();
            return !HasErrors;
        }
    }
}