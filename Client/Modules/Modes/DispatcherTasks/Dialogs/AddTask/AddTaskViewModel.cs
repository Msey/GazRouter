using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.DispatcherTasks.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.DispatcherTasks.Dialogs.AddTask
{
    public class AddTaskViewModel : DialogViewModel<Action<string, string>>
    {
        private string _description;
        private string _subject;
        private bool _isEditMode;
        
        public AddTaskViewModel(Action<string, string> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _isEditMode = false;

            SaveCommand = new DelegateCommand(() => DialogResult = true);
        }

        public AddTaskViewModel(TaskDTO dto, Action<string, string> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _isEditMode = true;
            _subject = dto.Subject;
            _description = dto.Description;

            SaveCommand = new DelegateCommand(() => DialogResult = true, OnSaveCommandCanExecute);
        }

        public string Caption => _isEditMode ? "Редактирование ДЗ" : "Создание ДЗ";

        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Subject
        {
            get { return _subject; }
            set
            {
                if (SetProperty(ref _subject, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand { get; set; }
        
        protected bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Subject) && !string.IsNullOrEmpty(Description);
        }

        protected override void InvokeCallback(Action<string, string> closeCallback)
        {
            closeCallback?.Invoke(Subject, Description);
        }
    }
}