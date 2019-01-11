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

namespace GazRouter.Repair.RepWorks
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class WorksTableViewModel : MainViewModelBase
    {
        private int? _selectedYear;
        private GasTransportSystemDTO _selectedSystem;
        private Plan.Repair _selectedRepair;
        private PlanningStage _planningStage = PlanningStage.Filling;
        

        public bool ShowComplex { get; set; } = false;
        public int SpanColumns { get; set; } = 2;

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
        public WorksTableViewModel()
        {
            MonthColorList.ColorChanged += MonthColorListOnColorChanged;
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.RepairInProgress);
            //if (IsEditPermission == false) _ganttRange = 1;
            //
            AddRepairCommand = new DelegateCommand(AddRepair, () => IsPlanSelected && IsChangesAllowed && IsEditPermission);
            EditRepairCommand = new DelegateCommand(EditRepair, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission);
            RemoveRepairCommand = new DelegateCommand(RemoveRepair, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission);

            
            _selectedYear = IsolatedStorageManager.Get<int?>("RepairPlanLastSelectedYear") ?? DateTime.Today.Year;
            _selectedSystem = SystemList.First();

            LoadData();
        }

        public MonthToColorList MonthColorList { get; set; } = new MonthToColorList();

        private void MonthColorListOnColorChanged(object sender, EventArgs eventArgs)
        {
            foreach (Plan.Repair rep in RepairList)
            {
                rep.Dto.StartDate = rep.Dto.StartDate;
            }
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


        private async void LoadData(int? repairId = null, int? complexId = null)
        {
            if (!IsPlanSelected) return;

            Lock();

            var plan = await new RepairsServiceProxy().GetRepairPlanAsync(
                new GetRepairPlanParameterSet
                {
                    Year = _selectedYear.Value,
                    SystemId = _selectedSystem.Id
                });

            RepairList = plan.RepairList.Select(Plan.Repair.Create).OrderBy(r => r.SortOrder).ToList();
            OnPropertyChanged(() => RepairList);

            
            // Выставляем этап планирования
            _planningStage = plan.PlanningStage.Stage;
            OnPropertyChanged(() => PlanningStage);


            RefreshCommands();

            Unlock();

            if (repairId.HasValue)
            {
                _selectedRepair = RepairList.FirstOrDefault(r => r.Id == repairId.Value);
                OnPropertyChanged(() => SelectedRepair);
            }
           

            RefreshCommands();
        }


        private void RefreshCommands()
        {
            AddRepairCommand.RaiseCanExecuteChanged();
            EditRepairCommand.RaiseCanExecuteChanged();
            RemoveRepairCommand.RaiseCanExecuteChanged();
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
                    RefreshCommands();
                }
            }
        }


        public DelegateCommand AddRepairCommand { get; }
        public DelegateCommand EditRepairCommand { get; }
        public DelegateCommand RemoveRepairCommand { get; }


        private void AddRepair()
        {
            var viewModel = new AddEditRepairViewModel(id => LoadData(id, null), SelectedYear.Value);
            var view = new AddEditRepairView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void EditRepair()
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