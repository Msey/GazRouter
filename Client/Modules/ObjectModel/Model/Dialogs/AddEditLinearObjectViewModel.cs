using System;
using GazRouter.DTO;

namespace GazRouter.ObjectModel.Model.Dialogs
{
    public abstract class AddEditLinearObjectViewModel<TEnt, TKey> : AddEditViewModelBase<TEnt, TKey>
        where TEnt : BaseDto<TKey>,
            new()
        where TKey : struct
    {
        #region Constr

        protected AddEditLinearObjectViewModel(Action<TKey> closeCallback)
            : base(closeCallback)
        {
        }

        protected AddEditLinearObjectViewModel(Action<TKey> closeCallback, TEnt model)
            : base(closeCallback, model)
        {
        }

        #endregion

        #region KilometerOfStart

        private double? _kilometerOfStart = 0;

        public virtual double? KilometerOfStartPoint
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

        private double? _kilometerOfEnd = 0;

        public virtual double? KilometerOfEndPoint
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

        
        
    }
}