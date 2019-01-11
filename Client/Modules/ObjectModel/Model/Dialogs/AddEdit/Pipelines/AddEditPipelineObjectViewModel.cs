using System;
using GazRouter.DTO;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines
{
    public abstract class AddEditPipelineObjectViewModel<TEnt, TKey> : AddEditViewModelBase<TEnt, TKey>
        where TEnt : BaseDto<TKey>,
            new()
        where TKey : struct
    {
        #region Constr

        protected AddEditPipelineObjectViewModel(Action<TKey> closeCallback)
            : base(closeCallback)
        {
        }

        protected AddEditPipelineObjectViewModel(Action<TKey> closeCallback, TEnt model)
            : base(closeCallback, model)
        {
        }

        #endregion

        #region KilometerOfStart

        private double? _kilometerOfStart = 0;

        public double? KilometerOfStartPoint
        {
            get { return _kilometerOfStart; }
            set
            {
                if (SetProperty(ref _kilometerOfStart, value))
                {
                    OnPropertyChanged(() => KilometerOfEndPoint);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region KilometerOfEnd

        private double? _kilometerOfEnd = 1;

        public double? KilometerOfEndPoint
        {
            get { return _kilometerOfEnd; }
            set
            {
                if (SetProperty(ref _kilometerOfEnd, value))
                {
                    OnPropertyChanged(() => KilometerOfStartPoint);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region KilometerOfStartPointConn

        private double? _kilometerOfStartConn = 0;
        public double? KilometerOfStartPointConn
        {
            get { return _kilometerOfStartConn; }
            set
            {
                if (SetProperty(ref _kilometerOfStartConn, value))
                {
                    OnPropertyChanged(() => KilometerOfEndPointConn);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region KilometerOfEndPointConn

        private double? _kilometerOfEndConn = 0;
        public double? KilometerOfEndPointConn
        {
            get { return _kilometerOfEndConn; }
            set
            {
                if (SetProperty(ref _kilometerOfEndConn, value))
                {
                    OnPropertyChanged(() => KilometerOfStartPointConn);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region StartEntity

        private CommonEntityDTO _startEntity;
        public CommonEntityDTO StartEntity
        {
            get { return _startEntity; }
            set
            {
                _startEntity = value;
                OnPropertyChanged(() => StartEntity);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region EndEntity

        private CommonEntityDTO _endEntity;
        public virtual CommonEntityDTO EndEntity
        {
            get { return _endEntity; }
            set
            {
                _endEntity = value;
                OnPropertyChanged(() => EndEntity);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region StartEntityId

        private Guid? _startEntityId;
        public Guid? StartEntityId
        {
            get { return _startEntityId; }
            set
            {
                _startEntityId = value;
                OnPropertyChanged(() => StartEntityId);
            }
        }

        #endregion

        #region EndEntityId

        private Guid? _endEntityId;
        public Guid? EndEntityId
        {
            get { return _endEntityId; }
            set
            {
                _endEntityId = value;
                OnPropertyChanged(() => EndEntityId);
            }
        }

        #endregion
    }
}