using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Common.ViewModel
{
    public abstract class CheckXslViewModelBase<TEntity, TId> : DialogViewModel<Action<TId>> where TEntity : class, new()
    {
        private string _name;

       
        protected CheckXslViewModelBase(Action<TId> closeCallback, TEntity model = null)
            : base(closeCallback)
        {
            _isEdit = model != null;
            _isAdd = !_isEdit;

            Model = model ?? new TEntity();
          
            InitInternal();
        }

        private void InitInternal()
        {
            SaveCommand = new DelegateCommand(OnSaveCommandExecuted, OnSaveCommandCanExecute);

        }

        protected override void InvokeCallback(Action<TId> closeCallback)
        {
            closeCallback(Id);
        }

        [NotNull]
        protected TEntity Model { get; private set; }

        protected TId Id { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if(SetProperty(ref _name, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _isEdit;

        public bool IsEdit
        {
            get { return _isEdit; }
            protected set
            {
                _isEdit = value;
                OnPropertyChanged(() => IsEdit);
                OnPropertyChanged(() => ButtonSaveCaption);
                IsAdd = !value;
            }
        }
        private bool _isAdd;
        public bool IsAdd
        {
            get { return _isAdd; }
            protected set
            {
                _isAdd = value;
                OnPropertyChanged(() => IsAdd);
            }
        }
        public DelegateCommand SaveCommand { get; private set; }

        protected abstract bool OnSaveCommandCanExecute();

        private void OnSaveCommandExecuted()
        {
            ValidateAll();

            if (HasErrors)
                return;

            if (IsEdit)
            {

                UpdateCurrent();
            }
            else
            {
                CreateNew();
            }
        }

        protected virtual async void CreateNew()
        {
            Behavior.TryLock();
            TId newId;
            try
            {
                newId = await CreateTask;
            }
            finally
            {
                Behavior.TryUnlock();
            }
            Id = newId;
            DialogResult = true;
            
        }

        protected virtual async void UpdateCurrent()
        {
            Behavior.TryLock();
            try
            {
                await UpdateTask;
            }
            finally
            {
                Behavior.TryUnlock();
            }

            DialogResult = true;
         
        }

        protected virtual Task<TId> CreateTask => null;

        protected virtual Task UpdateTask => null;

        #region Caption

        public virtual string Caption => $"{(IsEdit ? "Редактирование" : "Добавление")} {CaptionEntityTypeName}";

        protected abstract string CaptionEntityTypeName { get; }

        #endregion

        #region ButtonSaveCaption

        private string _buttonSaveCaption;
        public string ButtonSaveCaption
        {
            get
            {
                if (_buttonSaveCaption == null)
                    return IsEdit ? "Сохранить" : "Добавить";
                return _buttonSaveCaption;
            }
            protected set { _buttonSaveCaption = value; OnPropertyChanged(() => ButtonSaveCaption); }
        }

        #endregion

    }
}