using System;
using System.Collections.Generic;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Balances.Commercial.Plan.Corrections;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.Docs;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Balances.Commercial.Plan.CorrectionDocs
{
    public class CorrectionDocsViewModel : ViewModelBase
    {
        private BalanceDataBase _data;

        public CorrectionDocsViewModel(BalanceDataBase data, bool isEditPermission, Action<int> showSummaryAction)
        {
            _data = data;
            
            AddDocCommand = new DelegateCommand(AddDoc, () => isEditPermission);
            EditDocCommand = new DelegateCommand(EditDoc, () => SelectedDoc != null && isEditPermission);
            DeleteDocCommand = new DelegateCommand(DeleteDoc, () => SelectedDoc != null && isEditPermission);
            ShowCorrectionSummaryCommand = new DelegateCommand(() => showSummaryAction(SelectedDoc.Id), () => SelectedDoc != null);
        }

        public List<DocDTO> DocList => _data.CorrectionDocs;

        private DocDTO _selectedDoc;
        public DocDTO SelectedDoc
        {
            get { return _selectedDoc; }
            set
            {
                if (SetProperty(ref _selectedDoc, value))
                {
                    EditDocCommand.RaiseCanExecuteChanged();
                    DeleteDocCommand.RaiseCanExecuteChanged();
                    ShowCorrectionSummaryCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public async void RefreshDocs()
        {
            _data.CorrectionDocs = await new BalancesServiceProxy().GetDocListAsync(_data.PlanContract.Id);
            OnPropertyChanged(() => DocList);
        }


        public DelegateCommand ShowCorrectionSummaryCommand { get; set; }


        public DelegateCommand AddDocCommand { get; set; }

        public void AddDoc()
        {
            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = obj as AddEditAttachmentViewModel;

                await new BalancesServiceProxy().AddDocAsync(
                    new AddDocParameterSet
                    {
                        Data = x.FileData,
                        FileName = x.FileName,
                        Description = x.Description,
                        ExternalId = _data.PlanContract.Id
                    });

                RefreshDocs();
            });
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }

        public DelegateCommand EditDocCommand { get; set; }
        public void EditDoc()
        {
            if (SelectedDoc == null) return;
            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = obj as AddEditAttachmentViewModel;

                await new BalancesServiceProxy().EditDocAsync(
                    new EditDocParameterSet
                    {
                        DocId = SelectedDoc.Id,
                        Data = x.FileData,
                        FileName = x.FileName,
                        Description = x.Description,
                        ExternalId = _data.PlanContract.Id
                    });

                RefreshDocs();
            }, SelectedDoc);
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }


        public DelegateCommand DeleteDocCommand { get; set; }
        public void DeleteDoc()
        {
            if (SelectedDoc == null) return;
            MessageBoxProvider.Confirm(
                "Внимание! Удаляем прикрепленный документ. После удаления все связанные корректировки также будут удалены. Необходимо Ваше подтверждение.",
                async (result) =>
                {
                    if (result)
                    {
                        await new BalancesServiceProxy().DeleteDocAsync(SelectedDoc.Id);
                        RefreshDocs();
                    }
                }, "Подтверждение", "Удалить", "Отмена");

        }

    }
}