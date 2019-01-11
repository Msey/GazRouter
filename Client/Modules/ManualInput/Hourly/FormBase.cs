using System;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Utils.Extensions;

namespace GazRouter.ManualInput.Hourly
{
    public abstract class FormBase : ValidationViewModel
    {
        protected DateTime Date;
        protected PeriodType PeriodType;
        protected int SerieId;
        
        protected FormBase(DateTime date, int serieId)
        {
            Date = date.ToLocal();
            PeriodType = PeriodType.Twohours;
            SerieId = serieId;
        }
        
        private bool _isInputAllowed;
        /// <summary>
        /// Разрешен ли ввод данных.
        /// Нужно для задания свойства IsReadOnly на формах ввода, 
        /// в случае если ввод данных запрещен.
        /// </summary>
        public bool IsInputAllowed
        {
            get { return _isInputAllowed; }
            set
            {
                _isInputAllowed = value;
                OnPropertyChanged(() => IsInputAllowed);
            }
        }


        public abstract void Load();
        public abstract void Save();

        /// <summary>
        /// Вызывается после сохранения данных и выполнения проверок,
        /// для обновления статуса в дереве, как это сделать подругому, я не знаю.
        /// </summary>
        public Action UpdateNodeStatus { get; set; }
        
    }
}