using System;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.DispatcherTasks.Dialogs.AddTaskRecordComment
{
    public class AddTaskRecordCommentViewModel : DialogViewModel
    {
        private readonly GetRecordNoteListParameterSet _recordNote;
        private readonly RecordNoteDTO _recordDto = null;

        private string _text;

        public AddTaskRecordCommentViewModel(GetRecordNoteListParameterSet recordNote, Action actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _recordNote = recordNote;
            SaveCommand = new DelegateCommand(OnSaveCommandExecuted, OnSaveCommandCanExecute);
        }

        public AddTaskRecordCommentViewModel(RecordNoteDTO recordNote, Action actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            _recordDto = recordNote;
            SaveCommand = new DelegateCommand(OnSaveCommandExecuted, OnSaveCommandCanExecute);
            Text = _recordDto.Note;
            Caption = "Изменить комментарий";
            
        }

        private string _caption = "Добавить комментарий";
        public string Caption
        {
            get { return _caption; }
            set {
                _caption = value;
                OnPropertyChanged(() => Caption);
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(() => Text);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SaveCommand { get; }

        private async void OnSaveCommandExecuted()
        {
            Behavior.TryLock();

            try
            {
                if (_recordDto != null)
                {
                    await new DispatcherTaskServiceProxy().EditTaskRecordNoteAsync(new EditRecordNoteParameterSet
                    {
                        RecordNoteId = _recordDto.Id,
                        Note = Text
                    });
                }
                else
                {
                    await new DispatcherTaskServiceProxy().AddRecordNoteAsync(new AddRecordNoteParameterSet
                    {
                        TaskId = _recordNote.TaskId,
                        EntityId = _recordNote.EntityId,
                        PropertyTypeId = _recordNote.PropertyTypeId,
                        Note = Text
                    });
                }
            }
            finally
            {
                Behavior.TryUnlock();
            }
            DialogResult = true;
        }

        private bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Text);
        }
    }
}