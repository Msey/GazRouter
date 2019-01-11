using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.DispatcherTasks.RecordNotes;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.Modes.DispatcherTasks.Dialogs.AddTaskRecordComment;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Modes.DispatcherTasks.Common.RecordComments
{
    public class RecordCommentsViewModel : LockableViewModel
    {
        private readonly BaseTaskRecord _record;

        public RecordCommentsViewModel(BaseTaskRecord record)
        {
            _record = record;

            RefreshCommand = new DelegateCommand(Load);
            AddCommand = new DelegateCommand(Add);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            DeleteCommand = new DelegateCommand(Delete, CanEdit);

            Load();
        }
        


        public List<RecordNoteDTO> NoteList { get; set; }

        private RecordNoteDTO _selectedNote;
        public RecordNoteDTO SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                if (SetProperty(ref _selectedNote, value))
                {
                    EditCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private GetRecordNoteListParameterSet GetPSet()
        {
            if (_record == null) return null;
            return new GetRecordNoteListParameterSet
            {
                TaskId = _record.TaskId,
                EntityId = _record.Entity.Id,
                PropertyTypeId = _record.PropertyTypeId
            };
        }

        public DelegateCommand RefreshCommand { get; set; }

        public async void Load()
        {
            Lock();
            NoteList = _record != null
                ? await new DispatcherTaskServiceProxy().GetRecordNoteListAsync(GetPSet())
                : new List<RecordNoteDTO>();
            OnPropertyChanged(() => NoteList);
            Unlock();
        }

        public DelegateCommand AddCommand { get; set; }
        protected void Add()
        {
            var viewModel = new AddTaskRecordCommentViewModel(GetPSet(), Load);
            var view = new AddTaskRecordCommentView { DataContext = viewModel };
            view.ShowDialog();
        }


        public DelegateCommand EditCommand { get; set; }
        protected void Edit()
        {
            var viewModel = new AddTaskRecordCommentViewModel(SelectedNote, Load);
            var view = new AddTaskRecordCommentView { DataContext = viewModel };
            view.ShowDialog();
        }


        public DelegateCommand DeleteCommand { get; set; }
        protected void Delete()
        {
            if (SelectedNote == null) return;

            MessageBoxProvider.Confirm(
                "Необходимо Ваше подтверждение. Удалить комментарий?",
                async result =>
                {
                    if (result)
                    {
                        await new DispatcherTaskServiceProxy().RemoveTaskRecordNoteAsync(SelectedNote.Id);
                        Load();
                    }
                },
                "Удаление комментария",
                "Удалить");
        }

        public bool CanEdit()
        {
            return SelectedNote != null && Application.UserProfile.Current.Id == SelectedNote.CreateUserId;
        }
    }
}