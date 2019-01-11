using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Microsoft.Practices.Prism.Regions;
using GazRouter.Modes.DispatcherTasks.Common.AttachmentsView;

namespace GazRouter.Modes.DispatcherTasks.Enterprise
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class EnterpriseViewModel : LockableViewModel
    {
        public EnterpriseViewModel()
        {
            TasksViewModel = new TasksViewModel(SelectedTaskChanged);
        }


        // Список заданий
        public TasksViewModel TasksViewModel { get; }
        

        private void SelectedTaskChanged()
        {
            OnPropertyChanged(() => TaskStatusList);

            AttachmentsViewModel = new AttachmentsViewModel(TasksViewModel?.SelectedTask?.Dto?.Id,
                TasksViewModel?.SelectedTask?.Dto?.IsArchive ?? false);
            OnPropertyChanged(() => AttachmentsViewModel);

            RecordsViewModel = new TaskRecordsViewModel(TasksViewModel?.SelectedTask);
            OnPropertyChanged(() => RecordsViewModel);
        }



        public List<TaskStatusDTO> TaskStatusList => TasksViewModel?.SelectedTask?.Dto?.StatusList;

        public AttachmentsViewModel AttachmentsViewModel { get; set; }

        public TaskRecordsViewModel RecordsViewModel { get; set; }



/*
#region EXPORT TO EXCEL

        private ExcelReport _excelReport;
        private DateTime _reportDate;                // отчет и его дата и время

        public DelegateCommand ExportExcelCommand { get; set; }// Команда формирования отчета в Ms Excel,

        private async void ExportToExcel()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                //DefaultFileName = Header
            };
            if (dialog.ShowDialog() == true)
            {
                _excelReport = new ExcelReport("Текущие задания");
                _reportDate = DateTime.Now;
                ReportHeader();
                foreach (var item in TaskListVM.TaskList)
                {
                    if (!item.Dto.IsArchive)
                    {
                        var tasknumber = item.Dto.TaskNumber;
                        var subject = item.Dto.Subject;
                        var status = item.Dto.StatusTypeName;
                        var setdate = String.Format("{0:dd.MM.yyyy HH: mm}", item.Dto.StatusSetDate);
                        var details = "";
                        foreach (var st in item.Statuses)
                        {
                            if (details != "")
                                details += "\n";
                            details += st.StatusTypeName + " / " +
                            String.Format("{0:dd.MM.yyyy HH: mm}", st.CreateDate) + " / " +
                            st.CreateUserName + " / " +
                            st.Reason;
                        }
                        var description = item.Dto.Description;
                        var att = "";
                        var list = await new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(item.Dto.Id);
                        foreach (var at in list)
                        {
                            if (att != "")
                                att += "\n";
                            att += String.Format("{0:dd.MM.yyyy HH: mm}", at.CreateDate) + " / " +
                            at.Description + " / " + at.FileName + " / " + at.CreateUserName;
                        }
                        var taskLines = await new DispatcherTaskServiceProxy().GetTaskRecordCPDDListAsync(new GetTaskRecordsCpddParameterSet
                        {
                            TaskVersionId = item.SelectedStatus.Id,
                            IsCpdd = false
                        });
                        bool printed = false;
                        foreach (TaskRecordDTO record in taskLines)
                        {
                            var cm = "";
                            var comments = await new DispatcherTaskServiceProxy().GetRecordNoteListAsync(new GetRecordNoteListParameterSet
                            {
                                TaskId = record.TaskId,
                                EntityId = record.EntityId,
                                PropertyTypeId = record.PropertyTypeId
                            });
                            foreach (var comment in comments)
                            {
                                if (cm != "") cm += "\n";
                                cm += comment.Note + " / " + comment.CreateUserName + " / " + String.Format("{0:dd.MM.yyyy HH: mm}", comment.CreateDate);
                            }
                            _excelReport.NewRow();
                            _excelReport.WriteCell(tasknumber);
                            _excelReport.WriteCell(subject);
                            _excelReport.WriteCell(status);
                            _excelReport.WriteCell(setdate);
                            _excelReport.WriteCell(details);
                            _excelReport.WriteCell(description);
                            _excelReport.WriteCell(att);
                            _excelReport.WriteCell(record.SiteName);
                            _excelReport.WriteCell(record.Description);
                            _excelReport.WriteCell(record.Parameter);
                            _excelReport.WriteCell(record.TargetValue);
                            _excelReport.WriteCell(new PropertyTypeToUnitNameConverter().Convert(record.PropertyTypeId, typeof(string), null, null));
                            _excelReport.WriteCell(record.CompletionDate);
                            _excelReport.WriteCell(record.ExecutedDate);
                            _excelReport.WriteCell(record.ExecutedUserName);
                            _excelReport.WriteCell(record.CreateUserName);
                            _excelReport.WriteCell(cm);
                            printed = true;
                        }
                        if (!printed)
                        {
                            _excelReport.NewRow();
                            _excelReport.WriteCell(tasknumber);
                            _excelReport.WriteCell(subject);
                            _excelReport.WriteCell(status);
                            _excelReport.WriteCell(setdate);
                            _excelReport.WriteCell(details);
                            _excelReport.WriteCell(description);
                            _excelReport.WriteCell(att);
                            for (int i = 0; i < 9; i++) _excelReport.WriteCell("");
                        }
                    }
                } 
                _excelReport.Move(0, 0, "Архив");
                ReportHeader();
                foreach (var item in TaskListVM.TaskList)
                {
                    if (item.Dto.IsArchive)
                    {
                        var tasknumber = item.Dto.TaskNumber;
                        var subject = item.Dto.Subject;
                        var status = item.Dto.StatusTypeName;
                        var setdate = String.Format("{0:dd.MM.yyyy HH: mm}", item.Dto.StatusSetDate);
                        var details = "";
                        foreach (var st in item.Statuses)
                        {
                            if (details != "")
                                details += "\n";
                            details += st.StatusTypeName + " / " +
                            String.Format("{0:dd.MM.yyyy HH: mm}", st.CreateDate) + " / " +
                            st.CreateUserName + " / " +
                            st.Reason;
                        }
                        var description = item.Dto.Description;
                        var att = "";
                        var list = await new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(item.Dto.Id);
                        foreach (var at in list)
                        {
                            if (att != "")
                                att += "\n";
                            att += String.Format("{0:dd.MM.yyyy HH: mm}", at.CreateDate) + " / " +
                            at.Description + " / " + at.FileName + " / " + at.CreateUserName;
                        }
                        var taskLines = await new DispatcherTaskServiceProxy().GetTaskRecordCPDDListAsync(new GetTaskRecordsCpddParameterSet
                        {
                            TaskVersionId = item.SelectedStatus.Id,
                            IsCpdd = false
                        });
                        bool printed = false;
                        foreach (TaskRecordDTO record in taskLines)
                        {
                            var cm = "";
                            var comments = await new DispatcherTaskServiceProxy().GetRecordNoteListAsync(new GetRecordNoteListParameterSet
                            {
                                TaskId = record.TaskId,
                                EntityId = record.EntityId,
                                PropertyTypeId = record.PropertyTypeId
                            });
                            foreach (var comment in comments)
                            {
                                if (cm != "") cm += "\n";
                                cm += comment.Note + " / " + comment.CreateUserName + " / " + String.Format("{0:dd.MM.yyyy HH: mm}", comment.CreateDate);
                            }
                            _excelReport.NewRow();
                            _excelReport.WriteCell(tasknumber);
                            _excelReport.WriteCell(subject);
                            _excelReport.WriteCell(status);
                            _excelReport.WriteCell(setdate);
                            _excelReport.WriteCell(details);
                            _excelReport.WriteCell(description);
                            _excelReport.WriteCell(att);
                            _excelReport.WriteCell(record.SiteName);
                            _excelReport.WriteCell(record.Description);
                            _excelReport.WriteCell(record.Parameter);
                            _excelReport.WriteCell(record.TargetValue);
                            _excelReport.WriteCell(new PropertyTypeToUnitNameConverter().Convert(record.PropertyTypeId, typeof(string), null, null));
                            _excelReport.WriteCell(record.CompletionDate);
                            _excelReport.WriteCell(record.ExecutedDate);
                            _excelReport.WriteCell(record.ExecutedUserName);
                            _excelReport.WriteCell(record.CreateUserName);
                            _excelReport.WriteCell(cm);
                            printed = true;
                        }
                        if (!printed)
                        {
                            _excelReport.NewRow();
                            _excelReport.WriteCell(tasknumber);
                            _excelReport.WriteCell(subject);
                            _excelReport.WriteCell(status);
                            _excelReport.WriteCell(setdate);
                            _excelReport.WriteCell(details);
                            _excelReport.WriteCell(description);
                            _excelReport.WriteCell(att);
                            for (int i = 0; i < 9; i++) _excelReport.WriteCell("");
                        }
                    }
                }
                using (var stream = dialog.OpenFile())
                {
                    _excelReport.Save(stream);
                }
            }
        }

        private void ReportHeader()
        {
            _excelReport.Write("Дата:").Write(_reportDate.Date).NewRow();
            _excelReport.Write("Время:").Write(_reportDate.ToString("HH:mm")).NewRow();
            _excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
            _excelReport.Write("ДЗ ПДС:").Write(SelectedSite.Name).NewRow();
            _excelReport.NewRow();
            _excelReport.WriteHeader("№ ДЗ", 80);
            _excelReport.WriteHeader("Тема", 150);
            _excelReport.WriteHeader("Статус", 120);
            _excelReport.WriteHeader("Последнее изменение", 120);
            _excelReport.WriteHeader("Детали\nСтатус / Дата / Создал / Причина аннулирования", 450);
            _excelReport.WriteHeader("Комментарий", 250);
            _excelReport.WriteHeader("Вложенные файлы\nДата добавления / Описание / Файл / Добавил", 450);
            _excelReport.WriteHeader("Для ЛПУ", 150);
            _excelReport.WriteHeader("Задание", 150);
            _excelReport.WriteHeader("Параметр", 150);
            _excelReport.WriteHeader("Значение", 100);
            _excelReport.WriteHeader("Ед.изм.", 100);
            _excelReport.WriteHeader("Срок выполнения", 150);
            _excelReport.WriteHeader("Выполнено", 150);
            _excelReport.WriteHeader("Выполнил", 150);
            _excelReport.WriteHeader("Создал", 150);
            _excelReport.WriteHeader("Комментарии\nКомментарий / Создал / Время", 450);
        }
#endregion
*/
    }
}