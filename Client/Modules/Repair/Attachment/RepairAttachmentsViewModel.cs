using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Repairs.RepairWorks;
using Microsoft.Practices.Prism.Commands;
using GazRouter.Repair.ReqWorks.Dialogs;
using GazRouter.Repair.RepWorks.Dialogs;
using GazRouter.Repair.PrintForms;

namespace GazRouter.Repair.Attachment
{
    public class RepairAttachmentsViewModel : LockableViewModel
    {
        private readonly int? _repairId;

        public List<RepairWorkAttachmentDTO> AttachmentList { get; private set; }

        private RepairWorkAttachmentDTO _selectedAttachment;
        public RepairWorkAttachmentDTO SelectedAttachment
        {
            get { return _selectedAttachment; }
            set
            {
                SetProperty(ref _selectedAttachment, value);
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }

        

        private readonly DelegateCommand _AddCommand;
        public DelegateCommand AddCommand => _AddCommand;

        private readonly DelegateCommand _RemoveCommand;
        public DelegateCommand RemoveCommand => _RemoveCommand;

        private readonly DelegateCommand _RemoveAllCommand;
        public DelegateCommand RemoveAllCommand => _RemoveAllCommand;

        private readonly DelegateCommand<RepairWorkAttachmentDTO> _OpenAttachmentCommand;
        public DelegateCommand<RepairWorkAttachmentDTO> OpenAttachmentCommand => _OpenAttachmentCommand;

        public RepairAttachmentsViewModel(int? RepairId, bool isReadOnly)
        {
            _repairId = RepairId;

            _AddCommand = new DelegateCommand(Add, () => !isReadOnly && _repairId.HasValue);
            _RemoveCommand = new DelegateCommand(Remove, () => !isReadOnly && SelectedAttachment != null);
            _RemoveAllCommand = new DelegateCommand(RemoveAll, () => !isReadOnly && _repairId.HasValue && (AttachmentList!=null && AttachmentList.Count>0));

            

            Refresh();
        }               

        private async void Refresh()
        {
            Lock();
            AttachmentList = _repairId.HasValue
                ? await new RepairsServiceProxy().GetRepairAttachementsListAsync(_repairId.Value)
                : new List<RepairWorkAttachmentDTO>();
            OnPropertyChanged(() => AttachmentList);
            RemoveAllCommand.RaiseCanExecuteChanged();
            Unlock();
        }

        //protected void RaiseCommands()
        //{
        //    AddCommand.RaiseCanExecuteChanged();
        //}

        

        private void Add()
        {
            if (!_repairId.HasValue) return;

            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = obj as AddEditAttachmentViewModel;
                if (x == null) return;
                await new RepairsServiceProxy().AddRepairWorkAttachmentAsync(
                    new AddRepairWorkAttachmentParameterSet
                    {
                        ExternalId = _repairId.Value,
                        FileName = x.FileName,
                        Description = x.Description,
                        Data = x.FileData
                    });
                Refresh();
            });
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }

        private void Remove()
        {
            if (SelectedAttachment == null) return;
            MessageBoxProvider.Confirm(
                "Необходимо Ваше подтверждение. Удалить вложение?",
                async result =>
                {
                    if (result)
                    {
                        await new RepairsServiceProxy().RemoveRepairWorkAttachmentAsync(SelectedAttachment.Id);
                        Refresh();
                    }
                },
                "Удаление вложения",
                "Удалить");


        }

        private void RemoveAll()
        {
            if (!_repairId.HasValue) return;
            MessageBoxProvider.Confirm(
                "Необходимо Ваше подтверждение. Удалить все вложения?",
                async result =>
                {
                    if (result)
                    {
                        await new RepairsServiceProxy().RemoveAllRepairWorkAttachmentsAsync(_repairId.Value);
                        Refresh();
                    }
                },
                "Удаление всех вложений",
                "Удалить");
        }
    }

}
