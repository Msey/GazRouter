using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Dialogs;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;
using Telerik.Windows.Data;
using Telerik.Windows.Diagrams.Core;
using AddEditComplexView = GazRouter.Repair.Plan.Dialogs.AddEditComplexView;
using AddEditComplexViewModel = GazRouter.Repair.Plan.Dialogs.AddEditComplexViewModel;
using AddEditRepairView = GazRouter.Repair.Plan.Dialogs.AddEditRepairView;
using AddEditRepairViewModel = GazRouter.Repair.Plan.Dialogs.AddEditRepairViewModel;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.Repairs.Workflow;
using GazRouter.DTO.Repairs.Agreed;
using GazRouter.DataProviders.Authorization;

namespace GazRouter.Repair.Agreement
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class UserAgreementsListViewModel : MainViewModelBase
    {
        private Plan.Repair _selectedRepair;

        private AgreedUserDTO _CurrentAgreementPerson;

        private readonly DelegateCommand _AgreeCommand;
        public DelegateCommand AgreeCommand => _AgreeCommand;

        private readonly DelegateCommand _DisagreeCommand;
        public DelegateCommand DisagreeCommand => _DisagreeCommand;

        public UserAgreementsListViewModel()
        {
            MonthColorList.ColorChanged += MonthColorListOnColorChanged;

            _AgreeCommand = new DelegateCommand(Agree);
            _DisagreeCommand = new DelegateCommand(Disagree);

            LoadData();
        }


        private void Agree()
        {
            EditAgreedRepairRecor(true);
        }

        private void Disagree()
        {
            EditAgreedRepairRecor(false);
        }

        

    public MonthToColorList MonthColorList { get; set; } = new MonthToColorList();

        private void MonthColorListOnColorChanged(object sender, EventArgs eventArgs)
        {
            foreach (Plan.Repair rep in RepairList)
            {
                rep.Dto.StartDate = rep.Dto.StartDate;
            }
        }
        

        private async void LoadData(int? repairId = null, int? complexId = null)
        {

            Lock();

            var _AgreementPersons = await new UserManagementServiceProxy().GetAgreedUsersAsync(new GetAgreedUsersAllParameterSet() { TargetDate = DateTime.Now, SiteId = UserProfile.Current.Site.Id });
            if (_AgreementPersons == null)
                _AgreementPersons = new List<AgreedUserDTO>();

            _CurrentAgreementPerson = _AgreementPersons.FirstOrDefault(person => person.UserID == Application.UserProfile.Current.Id);

            var plan = await new RepairsServiceProxy().GetWorkflowListAsync(
                new GetRepairWorkflowsParameterSet
                {
                    UserId = UserProfile.Current.Id,
                    Year = DateTime.Now.Year
                });

            RepairList = plan.RepairList.Select(Plan.Repair.Create).OrderBy(r => r.SortOrder).ToList();
            OnPropertyChanged(() => RepairList);

            

            

            Unlock();

            if (repairId.HasValue)
            {
                _selectedRepair = RepairList.FirstOrDefault(r => r.Id == repairId.Value);
                OnPropertyChanged(() => SelectedRepair);
            }
           
            
        }


        private void EditAgreedRepairRecor(bool IsAgreed)
        {
            if (SelectedRepair != null)
            {
                string dialogHeaderText = "Добавить комментарий для " + (IsAgreed ? "Утверждения" : "Отклонения");
                var ViewModel = new Dialogs.AgreementCommentDialogViewModel(dialogHeaderText, async comment =>
                {
                    Lock();
                    await new RepairsServiceProxy().EditAgreedRepairRecordAsync(new AddEditAgreedRepairRecordParameterSet()
                    {
                        Id = SelectedRepair.Dto.AgreedRecordID,
                        RepairID = SelectedRepair.Id,
                        AgreedUserId = SelectedRepair.Dto.AgreedUserID,
                        RealAgreedUserId = SelectedRepair.Dto.AgreedUserID == _CurrentAgreementPerson.AgreedUserId ? null : (int?)_CurrentAgreementPerson.AgreedUserId,
                        AgreedDate = DateTime.Now,
                        CreationDate = SelectedRepair.Dto.AgreedCreationDate.Value,
                        Comment = comment,
                        AgreedResult = IsAgreed,
                    });
                    LoadData();
                    Unlock();
                });
                var View = new Dialogs.AgreementCommentDialogView() { DataContext = ViewModel };
                View.ShowDialog();
            }
        }


    #region REPAIRS

    /// <summary>
    /// Список ремонтных работ
    /// </summary>
    public List<Plan.Repair> RepairList { get; set; }


        /// <summary>
        /// Выбранный ремонт
        /// </summary>
        public Plan.Repair SelectedRepair
        {
            get { return _selectedRepair; }
            set
            {
                if (SetProperty(ref _selectedRepair, value))
                {
                    //RefreshCommands();
                }
            }
        }


        public DelegateCommand AddRepairCommand { get; }
        public DelegateCommand EditRepairCommand { get; }
        public DelegateCommand RemoveRepairCommand { get; }



        private void EditRepair()
        {
            //var viewModel = new AddEditRepairViewModel(id => LoadData(id, null), SelectedRepair.Dto, SelectedYear.Value);
            //var view = new AddEditRepairView {DataContext = viewModel};
            //view.ShowDialog();
        }

        private void RemoveRepair()
        {
            //var dp = new DialogParameters
            //{
            //    Closed = async (s, e) =>
            //    {
            //        if (!e.DialogResult.HasValue || !e.DialogResult.Value) return;
            //        await new RepairsServiceProxy().DeleteRepairAsync(SelectedRepair.Id);
            //        LoadData();
            //    },
            //    Content = "Вы уверены что хотите удалить ремонт?",
            //    Header = "Удаление ремонта",
            //    OkButtonContent = "Да",
            //    CancelButtonContent = "Нет"
            //};

            //RadWindow.Confirm(dp);
        }
        

        #endregion

        

    }
}