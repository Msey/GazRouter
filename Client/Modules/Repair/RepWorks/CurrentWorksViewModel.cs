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
using GazRouter.Repair.Gantt;
using GazRouter.Repair.Plan.Gantt;
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
using AddEditRepairView = GazRouter.Repair.ReqWorks.Dialogs.AddEditRepairView;
using AddEditRepairViewModel = GazRouter.Repair.ReqWorks.Dialogs.AddEditRepairViewModel;
using AddEditRepairReportView = GazRouter.Repair.RepWorks.Dialogs.AddEditRepairReportView;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.Repair.ReqWorks.Dialogs;
using GazRouter.Controls.Attachment;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Repairs.Workflow;

namespace GazRouter.Repair.RepWorks
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class CurrentWorksViewModel : MainViewModelBase
    {
        private int? _selectedYear;
        private GasTransportSystemDTO _selectedSystem;
        private Plan.Repair _selectedRepair;
        private PlanningStage _planningStage = PlanningStage.Filling;

        private bool _showAllSitesAllowed = true;
        public bool ShowAllSitesAllowed
        {
            get { return _showAllSitesAllowed; }
            set
            {
                _showAllSitesAllowed = value;
                OnPropertyChanged(() => _showAllSitesAllowed);
            }
        }
        /// <summary>
        /// Список ЛПУ
        /// </summary>
        public List<SiteDTO> SiteList { get; set; }
        private SiteDTO _selectedSite;
        /// <summary>
        /// Выбранное ЛПУ
        /// </summary>
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    LoadData();
                }
            }
        }

        private async void LoadSites()
        {
            if (UserProfile.Current.Site.IsEnterprise)
            {
                ShowAllSitesAllowed = true;
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.Id
                });
            }
            else
            {
                ShowAllSitesAllowed = false;
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    SiteId = UserProfile.Current.Site.Id
                });
                foreach (var site in SiteList)
                {
                    if (site.Id == UserProfile.Current.Site.Id)
                    {
                        SelectedSite = site;
                        break;
                    }
                }
            }
            OnPropertyChanged(() => SiteList);
        }

        private bool _isAttachReportVisible = true;
        public bool IsAttachReportVisible
        {
            get { return _isAttachReportVisible; }
            set { _isAttachReportVisible = value;
                OnPropertyChanged(() => IsAttachReportVisible);
            }
        }

        public int SpanColumns { get; set; } = 2;
        public bool ShowComplex { get; set; } = false;
        //public PlanSchemeViewModel RepairSchemeViewModel { get; set; }

        private bool _isEditPermission;
        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }

        public WorkStateDTO.Stages Stage = WorkStateDTO.Stages.Current;
        public CurrentWorksViewModel()
        {
            //MonthColorList.ColorChanged += MonthColorListOnColorChanged;
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.RepairInProgress);
            
            AddRepairCommand = new DelegateCommand(AddRepair, () => IsPlanSelected && IsChangesAllowed && IsEditPermission && false);

            AddReportCommand = new DelegateCommand(AddReport, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission);
            EditReportCommand = new DelegateCommand(EditReport, () => SelectedRepair != null && SelectedReport != null && IsChangesAllowed && IsEditPermission);
            DeleteReportCommand = new DelegateCommand(DelReport, () => SelectedReport != null && IsChangesAllowed && IsEditPermission);

            AddAttachmentCommand = new DelegateCommand(AddAttach, () => SelectedReport != null && IsChangesAllowed && IsEditPermission);
            DeleteAttachmentCommand = new DelegateCommand(DelAttach, () => SelectedAttachment != null && IsChangesAllowed && IsEditPermission);

            EditRepairCommand = new DelegateCommand(EditRepair, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission);
            RemoveRepairCommand = new DelegateCommand(RemoveRepair, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission && false);

            DocCommand = new DelegateCommand(PrintDoc);
            AgreeFaxCommand = new DelegateCommand(PrintFax);

            _selectedYear = IsolatedStorageManager.Get<int?>("RepairPlanLastSelectedYear") ?? DateTime.Today.Year;
            _selectedSystem = SystemList.First();

            LoadSites();

            LoadData();

            SetStatusCommand = new DelegateCommand<WorkStateDTO>(SetStatus);
        }

        

        /// <summary>
        /// Список годов, для элемента выбора
        /// </summary>
        public IEnumerable<int> YearList => Enumerable.Range(2013, DateTime.Now.Year - 2013 + 1).Reverse();

        public int? SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (SetProperty(ref _selectedYear, value))
                {
                    IsolatedStorageManager.Set("RepairPlanLastSelectedYear", value);
                    LoadData();
                    OnPropertyChanged(() => IsPlanSelected);

                }
            }
        }

        /// <summary>
        /// Список ГТС
        /// </summary>
        public List<GasTransportSystemDTO> SystemList => ClientCache.DictionaryRepository.GasTransportSystems;

        /// <summary>
        /// Выбранная ГТС
        /// </summary>
        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (SetProperty(ref _selectedSystem, value))
                {
                    LoadData();
                    OnPropertyChanged(() => IsPlanSelected);
                }
            }
        }


        public bool IsPlanSelected => _selectedSystem != null && _selectedYear.HasValue;




        public virtual async void LoadData(int? repairId = null, int? complexId = null)
        {
            Lock();

            var param = new GetRepairWorkflowsParameterSet
            {
                Year = _selectedYear.Value,
                SystemId = _selectedSystem.Id,
                Stage = Stage
            };
            if (!UserProfile.Current.Site.IsEnterprise)
            {
                param.SiteId = UserProfile.Current.Site.Id;
            }
            else
            if (_selectedSite != null)
                param.SiteId = _selectedSite.Id;

            var plan = await new RepairsServiceProxy().GetWorkflowListAsync(param);

            RepairList = plan.RepairList.Select(Plan.Repair.Create).OrderBy(r => r.SortOrder).ToList();
            OnPropertyChanged(() => RepairList);

            


            // Выставляем этап планирования
            //_planningStage = plan.PlanningStage.Stage;
            //OnPropertyChanged(() => PlanningStage);


            //RefreshCommands();

            Unlock();

            if (repairId.HasValue)
            {
                _selectedRepair = RepairList.FirstOrDefault(r => r.Id == repairId.Value);
                OnPropertyChanged(() => SelectedRepair);
            }
            

            RefreshCommands();
        }

        public async void LoadReports(int? repairId = null)
        {
            if (SelectedRepair != null)
                repairId = SelectedRepair.Id;

            if (repairId.HasValue)
            {
                var plan = await new RepairsServiceProxy().GetRepairReportsByRepairAsync(repairId.Value);
                ReportsList = plan;
            }
            else
            {
                ReportsList = null;
            }

            OnPropertyChanged(() => ReportsList);
        }

        public async void LoadAttachments(int? repairId = null)
        {
            if (SelectedReport != null)
                repairId = SelectedReport.Id;

            if (repairId.HasValue)
            {
                var plan = await new RepairsServiceProxy().GetRepairReportAttachmentsAsync(repairId.Value);
                ReportAttachmentsList = plan;
            }
            else
            {
                ReportAttachmentsList = null;
            }

            OnPropertyChanged(() => ReportAttachmentsList);
        }
        

        private void RefreshCommands()
        {
            AddRepairCommand.RaiseCanExecuteChanged();
            EditRepairCommand.RaiseCanExecuteChanged();
            RemoveRepairCommand.RaiseCanExecuteChanged();

            AddReportCommand.RaiseCanExecuteChanged();
            EditReportCommand.RaiseCanExecuteChanged();
            DeleteReportCommand.RaiseCanExecuteChanged();

            AddAttachmentCommand.RaiseCanExecuteChanged();
            DeleteAttachmentCommand.RaiseCanExecuteChanged();

            RefreshStatusMenu();
            IsPrintingAllowed = SelectedRepair != null;
        }

        #region REPAIRS

        private bool _isTableVisible;

        /// <summary>
        /// Активна вкладка с таблицей
        /// </summary>
        public bool IsTableActive
        {
            get { return _isTableVisible; }
            set { SetProperty(ref _isTableVisible, value); }
        }

        /// <summary>
        /// Список ремонтных работ
        /// </summary>
        public List<Plan.Repair> RepairList { get; set; }

        //private List<RepairReportDTO>
        public List<RepairReportDTO> ReportsList { get; set; }

        public List<RepairReportAttachmentDTO> ReportAttachmentsList { get; set; }


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
                    LoadReports(value?.Id);
                    
                    RefreshCommands();
                }
                
            }
        }

        private RepairReportDTO _selectedReport;
        public RepairReportDTO SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                if (SetProperty(ref _selectedReport, value))
                {
                    LoadAttachments(value?.Id);
                    
                    RefreshCommands();
                }
            }
        }

        public DelegateCommand<WorkStateDTO> SetStatusCommand { get; set; }
        private List<SetStatusItem> _setStatusItemList = new List<SetStatusItem>();
        public List<SetStatusItem> SetStatusItemList
        {
            get { return _setStatusItemList; }
            set
            {
                if (SetProperty(ref _setStatusItemList, value))
                    IsSetStatusAllowed = value == null ? false : value.Count > 0;
            }
        }

        private void RefreshStatusMenu()
        {
            SetStatusItemList = SelectedRepair?.Dto.WFWState.GetTransfers(UserProfile.Current.Site.IsEnterprise).Select(s => new SetStatusItem(SetStatusCommand, s)).ToList();
        }

        private RepairReportAttachmentDTO _selectedAttachment;
        public RepairReportAttachmentDTO SelectedAttachment
        {
            get { return _selectedAttachment; }
            set
            {
                if (SetProperty(ref _selectedAttachment, value))
                {
                    RefreshCommands();
                }
            }
        }

        private bool _isSetStatusAllowed;
        public bool IsSetStatusAllowed
        {
            get { return _isSetStatusAllowed; }
            set
            {
                SetProperty(ref _isSetStatusAllowed, value);
            }
        }

        private bool _isPrintingAllowed;
        public bool IsPrintingAllowed
        {
            get { return _isPrintingAllowed; }
            set
            {
                SetProperty(ref _isPrintingAllowed, value);
            }
        }

        public bool IsPds
        {
            get { return UserProfile.Current.Site.IsEnterprise; }
        }


        public DelegateCommand AddRepairCommand { get; }
        public DelegateCommand EditRepairCommand { get; set; }
        public DelegateCommand RemoveRepairCommand { get; }

        public DelegateCommand AddReportCommand { get; }
        public DelegateCommand EditReportCommand { get; }
        public DelegateCommand DeleteReportCommand { get; }

        public DelegateCommand AddAttachmentCommand { get; }
        public DelegateCommand DeleteAttachmentCommand { get; }

        public DelegateCommand DocCommand { get; }
        public DelegateCommand AgreeFaxCommand { get; }

        private void AddRepair()
        {
            var viewModel = new AddEditRepairViewModel(id => LoadData(id, null), SelectedYear.Value);
            var view = new AddEditRepairView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void AddReport()
        {
            var viewModel = new AddEditRepairReportViewModel(id => LoadReports(id), SelectedRepair.Dto);
            var view = new AddEditRepairReportView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void EditReport()
        {
            var viewModel = new AddEditRepairReportViewModel(id => LoadReports(id), SelectedRepair.Dto, SelectedReport);
            var view = new AddEditRepairReportView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void DelReport()
        {
            if (SelectedReport == null) return;

            MessageBoxProvider.Confirm(
                "Необходимо Ваше подтверждение. Удалить отчет?",
                async result =>
                {
                    if (result)
                    {
                        await new RepairsServiceProxy().DeleteRepairReportAsync(SelectedReport.Id);
                        LoadReports();
                    }
                },
                "Удаление отчета",
                "Удалить");
        }


        private void AddAttach()
        {
            if (SelectedReport == null) return;

            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = obj as AddEditAttachmentViewModel;
                if (x == null) return;

                await new RepairsServiceProxy().AddRepairReportAttachmentAsync(
                    new RepairReportAttachmentParamentersSet 
                    {
                        RepairReportId = SelectedReport.Id,
                        Filename = x.FileName,
                        Description = x.Description,
                        Data = x.FileData                        
                    });

                LoadAttachments(SelectedReport?.Id);
            });
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }
        private void DelAttach()
        {
            if (SelectedAttachment == null) return;

            MessageBoxProvider.Confirm(
                "Необходимо Ваше подтверждение. Удалить приложение?",
                async result =>
                {
                    if (result)
                    {
                        await new RepairsServiceProxy().DeleteRepairReportAttachmentAsync(SelectedAttachment.Id);
                        LoadAttachments();
                    }
                },
                "Удаление приложения",
                "Удалить");
        }

        public void EditRepair()
        {
            var viewModel = new AddEditRepairViewModel(id => LoadData(id, null), SelectedRepair.Dto, SelectedYear.Value);
            var view = new AddEditRepairView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void RemoveRepair()
        {
            var dp = new DialogParameters
            {
                Closed = async (s, e) =>
                {
                    if (!e.DialogResult.HasValue || !e.DialogResult.Value) return;
                    await new RepairsServiceProxy().DeleteRepairAsync(SelectedRepair.Id);
                    LoadData();
                },
                Content = "Вы уверены что хотите удалить ремонт?",
                Header = "Удаление ремонта",
                OkButtonContent = "Да",
                CancelButtonContent = "Нет"
            };

            RadWindow.Confirm(dp);
        }

        private void PrintDoc()
        {
            var viewModel = new AddEditRepairViewModel(null, SelectedRepair.Dto, SelectedYear.Value);
            viewModel.PrintDoc();
        }

        private void PrintFax()
        {
            var viewModel = new AddEditRepairViewModel(null, SelectedRepair.Dto, SelectedYear.Value);
            viewModel.PrintFax();
        }

        private void SetStatus(WorkStateDTO targetStatus)
        {
            RadWindow.Confirm(new DialogParameters
            {
                Header = "Изменение статуса ремонтных работ",
                Content = "Подтвердите пожалуйста установку статуса \"" +
                    targetStatus.Name
                + "\"",
                Closed = async (o, even) =>
                {
                    if (even.DialogResult.HasValue && even.DialogResult.Value)
                    {
                        await new RepairsServiceProxy().ChangeWorkflowStateAsync(new ChangeRepairWfParametrSet
                        {
                            RepairId = SelectedRepair.Id,
                            WFState = targetStatus.WFState,
                            WState = targetStatus.WState,
                            repair = SelectedRepair.Dto
                        });

                        LoadData(SelectedRepair.Id);
                    }
                }
            });
        }

        internal async void ChangeRepairDates(Plan.Repair repair, DateTime start, DateTime end)
        {
            await new RepairsServiceProxy().EditRepairDatesAsync(
                new EditRepairDatesParameterSet
                {
                    DateStart = start,
                    DateEnd = end,
                    DateType = DateTypes.Plan,
                    RepairId = repair.Id
                });
            LoadData(repair.Id);
        }

        #endregion


        #region PLANNING STAGES

        /// <summary>
        /// Список этапов планирования
        /// </summary>
        public IEnumerable<PlanningStage> PlanningStageList
        {
            get
            {
                yield return PlanningStage.Filling;
                yield return PlanningStage.Optimization;
                yield return PlanningStage.Approved;
            }
        }


        /// <summary>
        ///     Этап планирования
        /// </summary>
        public PlanningStage PlanningStage
        {
            get { return _planningStage; }
            set
            {
                if (SetProperty(ref _planningStage, value))
                {
                    OnPropertyChanged(() => IsChangesAllowed);
                    UpdatePlanningStage();
                }
            }
        }

        /// <summary>
        ///     Информация о дате и пользователе изменившем этап планирования
        /// </summary>
        //public string PlanningStageUpdateInfo
        //{
        //    get { return _planningStageUpdateInfo; }
        //    set { SetProperty(ref _planningStageUpdateInfo, value); }
        //}



        /// <summary>
        ///     Разрешено ли изменение плана в зависимости от этапа планирования и профиля пользователя
        ///     (используется для блокировки ввода работ на ЛПУ)
        /// </summary>
        public bool IsChangesAllowed
        {
            get
            {
                if (!IsEditPermission) return false;
                //
                switch (PlanningStage)
                {
                    case PlanningStage.Filling:
                        return true;

                    case PlanningStage.Optimization:
                        return UserProfile.Current.Site.IsEnterprise;

                    case PlanningStage.Approved:
                        return false;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        ///     Сохранить выбранный этап планнирования в БД
        /// </summary>
        private async void UpdatePlanningStage()
        {
            if (!SelectedYear.HasValue) return;

            await new RepairsServiceProxy().SetPlanningStageAsync(
                new SetPlanningStageParameterSet
                {
                    Year = SelectedYear.Value,
                    SystemId = SelectedSystem.Id,
                    Stage = PlanningStage
                });

            LoadData(SelectedRepair?.Id, null);
        }

        #endregion

        
    }
}