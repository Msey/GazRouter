using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.DispatcherTasks.Attachments;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.DispatcherTasks.Common.AttachmentsView
{
    public class AttachmentsViewModel : LockableViewModel
    {
        private readonly Guid? _taskId;
        
        public AttachmentsViewModel(Guid? taskId, bool isReadOnly)
        {
            _taskId = taskId;
            
            AddCommand = new DelegateCommand(Add, () => !isReadOnly && _taskId.HasValue);
            DeleteCommand = new DelegateCommand(Delete, () => !isReadOnly && SelectedAttachment != null);
            
            Refresh();
        }
        

        public List<TaskAttachmentDTO> AttachmentList { get; set; }

        public DelegateCommand<TaskAttachmentDTO> OpenAttachmentCommand { get; protected set; }



        private TaskAttachmentDTO _selectedAttachment;
        public TaskAttachmentDTO SelectedAttachment
        {
            get { return _selectedAttachment; }
            set
            {
                SetProperty(ref _selectedAttachment, value);
                RaiseCommands();
            }
        }
        

        protected async void Refresh()
        {
            Lock();
            AttachmentList = _taskId.HasValue
                ? await new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(_taskId.Value)
                : new List<TaskAttachmentDTO>();
            OnPropertyChanged(() => AttachmentList);
            Unlock();
        }
        

        protected void RaiseCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }


        public DelegateCommand AddCommand { get; set; }
        protected void Add()
        {
            if (!_taskId.HasValue) return;

            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = obj as AddEditAttachmentViewModel;
                if (x == null) return;

                await new DispatcherTaskServiceProxy().AddTaskAttachmentAsync(
                    new AddTaskAttachmentParameterSet
                    {
                        TaskId = _taskId.Value,
                        FileName = x.FileName,
                        Description = x.Description,
                        Data = x.FileData
                    });

                Refresh();
            });
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }


        public DelegateCommand DeleteCommand { get; set; }
        protected void Delete()
        {
            if (SelectedAttachment == null) return;
            MessageBoxProvider.Confirm( 
                "Необходимо Ваше подтверждение. Удалить вложение?",
                async result =>
                {
                    if (result)
                    {
                        await new DispatcherTaskServiceProxy().DeleteTaskAttachmentAsync(SelectedAttachment.Id);
                        Refresh();
                    }
                },
                "Удаление вложения",
                "Удалить");

            
        }
    }
}