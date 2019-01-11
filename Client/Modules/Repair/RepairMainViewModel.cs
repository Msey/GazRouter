using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.Ui.Behaviors;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Dialogs;
using GazRouter.Repair.Enums;
using GazRouter.Repair.Gantt;
using GazRouter.Repair.UpdateHistory;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Scheduling;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Repair
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class RepairMainViewModel : MainViewModelBase, ITabItem
    {
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

        private RepairGroupingType _grouppingType = RepairGroupingType.ByPipeline;
        private PeriodType _periodType = PeriodType.Whole;
        private string _planningStageUpdateInfo;
        private PlanningStage _planningStage = PlanningStage.Filling;
        private List<RepairItem> _repairList = new List<RepairItem>();
        private int _selectedYear
#if DEBUG
            = 2013;
#else
          = DateTime.Now.Year;
#endif
        private RepairItem _selectedRepair;
        private bool _showNonCritical = true;
        private GasTransportSystemDTO _selectedSystem;

        public RepairMainViewModel()
        {

            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.RepairPlan);
            //
            const int startYear = 2010;

            YearList = new List<int>(Enumerable.Range(startYear, DateTime.Now.Year - startYear + 2).Reverse());

            RefreshCommand = new DelegateCommand(Refresh);
            AddRepairCommand = new DelegateCommand(OnAddRepairCommandExecuted, 
                                                   () => ChangesAllowed && IsEditPermission);
            AddExternalConditionCommand = new DelegateCommand(OnAddExternalConditionCommandExecuted,
                                                              () => ChangesAllowed && IsEditPermission);
            EditRepairCommand = new DelegateCommand<RepairItem>(OnEditRepairCommandExecuted,
                repairItem => ChangesAllowed && ( /*SelectedRepair != null ||*/ repairItem != null) && IsEditPermission);
            RemoveRepairCommand = new DelegateCommand<RepairItem>(OnRemoveRepairCommandExecuted,
                repairItem => ChangesAllowed && (repairItem != null /*|| SelectedRepair != null*/ && IsEditPermission));
            ShowUpdateHistoryCommand = new DelegateCommand(OnShowUpdateHistoryCommandExecuted,
                () => ChangesAllowed && SelectedRepair != null && IsEditPermission);
            EditComplexBySelectedRepairCommand = new DelegateCommand(
                OnEditComplexBySelectedRepairCommandExecuted,
                () =>
                    ChangesAllowed && SelectedRepair != null &&
                    SelectedRepair.Dto.Complex != null && SelectedRepair.Dto.Complex.Id > 0 && SelectedRepair.Dto.Complex.IsLocal && 
                    IsEditPermission);

            AddToComplexCommand = new DelegateCommand<object>(
                OnAddToComplexCommandExecuted,
                x => ChangesAllowed && SelectedRepair != null && SelectedRepair.IsCritical && IsEditPermission);

            AddToNewComplexCommand = new DelegateCommand(OnAddToNewComplexCommandExecuted,
                () => ChangesAllowed && SelectedRepair != null && !SelectedRepair.IsExternalCondition && IsEditPermission);

            RemoveFromComplexCommand = new DelegateCommand(
                OnRemoveFromComplexCommandExecuted,
                () => ChangesAllowed && SelectedRepair != null && SelectedRepair.ComplexId > 0 && IsEditPermission);

            ExportExcelCommand = new DelegateCommand(ExportToExcel, () => RepairList.Count() > 0/*true*/);

            _selectedSystem = SystemList.FirstOrDefault(p => p.Name == "ЕСГ");

            RepairSchemeViewModel = new RepairSchemeViewModel(this);   
            PlanGanttViewModel = new PlanGanttViewModel(this);
            ComplexViewModel = new ComplexViewModel(this);
        }

        public ObservableCollection<RepairItem> RepairList { get; } = new ObservableCollection<RepairItem>();

        public RepairItem SelectedRepair
        {
            get { return _selectedRepair; }
            set
            {
                if (!SetProperty(ref _selectedRepair, value))
                {
                    return;
                }

                RefreshCommands();

                if (value == null)
                {
                    return;
                }

                PlanGanttViewModel.SelectedGanttRepair = (GanttRepairTask) PlanGanttViewModel.GanttRepairList
                    .Single(g => g.Title == value.GroupObject)
                    .Children.Single(r => ((GanttRepairTask) r).RepairItem == value);

                ComplexViewModel.SelectComplexById(value.ComplexId);
            }
        }

        /// <summary>
        ///     Отображать работы не влияющие на транспорт газа
        /// </summary>
        public bool ShowNonCritical
        {
            get { return _showNonCritical; }
            set
            {
                if (SetProperty(ref _showNonCritical, value))
                {
                    UpdateClientRepairList();
                }
            }
        }

        /// <summary>
        ///     Способ группировки ремонтов
        /// </summary>
        public RepairGroupingType GrouppingType
        {
            get { return _grouppingType; }
            set
            {
                if (!SetProperty(ref _grouppingType, value))
                {
                    return;
                }

                _repairList.ForEach(r => r.GroupingType = value);
                _repairList.Sort();

                UpdateClientRepairList();
            }
        }

        /// <summary>
        ///     Способ группировки ремонтов
        /// </summary>
        public PeriodType PeriodType
        {
            get { return _periodType; }
            set
            {
                if (!SetProperty(ref _periodType, value))
                {
                    return;
                }

                UpdateClientRepairList();
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
                SetProperty(ref _planningStage, value);
                /*if (!SetProperty(ref _planningStage, value))
                {
                    return;
                }*/

                UpdatePlanningStage();
            }
        }

        /// <summary>
        ///     Информация о дате и пользователе изменившем этап планирования
        /// </summary>
        public string PlanningStageUpdateInfo
        {
            get { return _planningStageUpdateInfo; }
            set { SetProperty(ref _planningStageUpdateInfo, value); }
        }

        /// <summary>
        ///     Разрешено ли изменение плана в зависимости от этапа планирования и профиля пользователя
        ///     (используется для блокировки ввода работ на ЛПУ)
        /// </summary>
        public bool ChangesAllowed
        {
            get
            {
                if (!IsEditPermission) return false;
                       
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

        public DelegateCommand RefreshCommand { get; }

        /// <summary>
        ///     Добавление ремонта
        /// </summary>
        public DelegateCommand AddRepairCommand { get; }

        /// <summary>
        ///     Добавление внешнего условия
        /// </summary>
        public DelegateCommand AddExternalConditionCommand { get; private set; }

        /// <summary>
        ///     Редактирование ремонта
        /// </summary>
        public DelegateCommand<RepairItem> EditRepairCommand { get; }

        /// <summary>
        ///     Удаление ремонта
        /// </summary>
        public DelegateCommand<RepairItem> RemoveRepairCommand { get; }

        /// <summary>
        ///     Показать историю изменения ремонтной работы
        /// </summary>
        public DelegateCommand ShowUpdateHistoryCommand { get; }

        /// <summary>
        ///     Изменить данные по локальному комплексу для выбранного ремонта
        /// </summary>
        public DelegateCommand EditComplexBySelectedRepairCommand { get; }

        public DelegateCommand<object> AddToComplexCommand { get; }

        /// <summary>
        ///     Создать локальный комплекс и добавить туда выбранную работу
        /// </summary>
        public DelegateCommand AddToNewComplexCommand { get; }

        /// <summary>
        ///     Исключить работу из текущего комплекса
        /// </summary>
        public DelegateCommand RemoveFromComplexCommand { get; }

        public DelegateCommand ExportExcelCommand { get; set; }

        public int SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (!SetProperty(ref _selectedYear, value))
                {
                    return;
                }
                OnPropertyChanged(() => IsCurrentYearSelected);
                Refresh();
            }
        }

        public List<int> YearList { get; set; }

        /// <summary>
        ///     Выбран ли текущий год
        /// </summary>
        public bool IsCurrentYearSelected => SelectedYear == DateTime.Today.Year;

        /// <summary>
        ///     Список газотранспортных систем
        /// </summary>
        public List<GasTransportSystemDTO> SystemList => ClientCache.DictionaryRepository.GasTransportSystems;

        /// <summary>
        ///     Выбранная газотранспортная система
        /// </summary>
        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (!SetProperty(ref _selectedSystem, value))
                {
                    return;
                }
                Refresh();
            }
        }

        public RepairSchemeViewModel RepairSchemeViewModel { get; }

        public PlanGanttViewModel PlanGanttViewModel { get; }

        public ComplexViewModel ComplexViewModel { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Refresh();
        }

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public async void Refresh()
        {
            Behavior.TryLock("Загрузка ремонтов");

            try
            {
                var data = await new RepairsServiceProxy().GetRepairPlanAsync(
                    new GetRepairPlanParameterSet {Year = SelectedYear, SystemId = SelectedSystem.Id});
                _repairList = data.RepairList.Select(r => new RepairItem(r, GrouppingType)).ToList();
                _repairList.Sort();

                ComplexViewModel.RefreshComplexes(data.ComplexList);

                _planningStage = data.PlanningStage.Stage;
                OnPropertyChanged(() => PlanningStage);
                OnPropertyChanged(() => ChangesAllowed);
                PlanningStageUpdateInfo = !string.IsNullOrEmpty(data.PlanningStage.UserName)
                    ? data.PlanningStage.ToString()
                    : string.Empty;

                UpdateClientRepairList();
                PlanGanttViewModel.VisibleRange = new VisibleRange(new DateTime(SelectedYear, 1, 1),
                    new DateTime(SelectedYear, 12, 31));

                ComplexViewModel.CheckComplexes();
            }
            finally
            {
                Behavior.TryUnlock();
                ExportExcelCommand.RaiseCanExecuteChanged();
            }
        }

        public override void Lock(string lockMessage = null)
        {
            base.Lock(lockMessage);
            ComplexViewModel.IsBusyLoading = true;
            PlanGanttViewModel.IsBusyLoading = true;
            RepairSchemeViewModel.IsBusyLoading = true;
        }

        public override void Unlock()
        {
            ComplexViewModel.IsBusyLoading = false;
            PlanGanttViewModel.IsBusyLoading = false;
            RepairSchemeViewModel.IsBusyLoading = false;
            base.Unlock();
        }

        public void RefreshScheme()
        {
            RepairSchemeViewModel.LoadRepairScheme();
        }

        public void RefreshGantt(List<RepairItem> repairs)
        {
            PlanGanttViewModel.Refresh(repairs);
        }

        public async void EditRepairDates(EditRepairDatesParameterSet editRepairDatesParameterSet, Action onSuccess,
            IClientBehavior behavior)
        {
            Behavior.TryLock();
            try
            {
                await new RepairsServiceProxy().EditRepairDatesAsync(editRepairDatesParameterSet);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            onSuccess();
        }

        /// <summary>
        ///     Добавляет работу в комплекс
        /// </summary>
        /// <param name="repair">Ремонт</param>
        /// <param name="complexId">Идентификатор комплекса</param>
        internal async void AddRepairToComplex(RepairItem repair, int? complexId)
        {
            Behavior.TryLock();
            try
            {
                await new RepairsServiceProxy().AddRepairToComplexAsync(new AddRepairToComplexParameterSet
                {
                    ComplexId = complexId,
                    RepairId = repair.Id
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            Refresh();
        }

        /// <summary>
        ///     Передвинуть комплекс и все включенные в него работы
        /// </summary>
        /// <param name="complex">Комплекс</param>
        /// <param name="start">Новая дата начала</param>
        /// <param name="end">Новая дата завершения</param>
        internal void MoveComplex(ComplexDTO complex, DateTime start, DateTime end)
        {
            // Дата поставки МТР не согласуется с датой начала комплекса
            if (_repairList.Any(r => r.ComplexId == complex.Id && start < r.Dto.PartsDeliveryDate))
            {
                MessageBoxProvider.Alert(
                    "Комплекс невозможно передвинуть, т.к. даты поставки МТР одного или нескольких ремонтов включенных в комплекс не согласуются с новыми датами проведения комплекса.",
                    "Ошибка");
                return;
            }

            Behavior.TryLock();
            try
            {
                new RepairsServiceProxy().MoveComplexAsync(new EditComplexParameterSet
                {
                    Id = complex.Id,
                    ComplexName = complex.ComplexName,
                    IsLocal = complex.IsLocal,
                    SystemId = complex.SystemId,
                    StartDate = start,
                    EndDate = end
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            Refresh();
        }

        /// <summary>
        ///     Изменение дат проведения ремонтных работ
        /// </summary>
        /// <param name="repair">Ремонт</param>
        /// <param name="start">Новая дата начала</param>
        /// <param name="end">Новая дата окончания</param>
        internal void UpdateRepairDates(RepairItem repair, DateTime start, DateTime end)
        {
            repair.StartDatePlan = start;
            repair.EndDatePlan = end;

            EditRepairDates(new EditRepairDatesParameterSet
            {
                DateStart = start,
                DateEnd = end,
                DateType = DateTypes.Plan,
                RepairId = repair.Id
            }, () =>
            {
                ComplexViewModel.CheckComplexes();
                UpdateClientRepairList();
            },
                Behavior);
        }

        /// <summary>
        ///     Добавить ремонт в комплекс ПАО "Газпром"
        /// </summary>
        /// <param name="complex">комплекс</param>
        private void AddToEnterpriseComplex(ComplexItem complex)
        {
            // Если комплекс ПАО Газпром и даты ремонта не соответсвуют датам проведения комплекса - изменяем даты ремонта
            if (SelectedRepair.Dto.StartDate < complex.StartDate || SelectedRepair.Dto.EndDate > complex.EndDate)
            {
                var msg = "Вы добавляете ремонт в комплекс ПАО \"Газпром\". "
                          + "При этом сроки проведения ремонта не соответсвуют срокам проведения комплекса."
                          + "В случае добавления дата начала ремонта и при необходимости его продолжительность "
                          + "будут автоматически изменены в соответсвии со сроками проведения комплекса."
                          + Environment.NewLine + Environment.NewLine
                          + "Добавить ремонт в комплекс?";

                RadWindow.Confirm(
                    new DialogParameters
                    {
                        Closed = (s, e) =>
                        {
                            if (!e.DialogResult.HasValue || !e.DialogResult.Value)
                            {
                                return;
                            }

                            // Изменить даты ремонта
                            var start = SelectedRepair.Dto.StartDate >= complex.StartDate &&
                                        SelectedRepair.Dto.StartDate < complex.EndDate
                                ? SelectedRepair.Dto.StartDate
                                : complex.StartDate;
                            var end = SelectedRepair.Dto.EndDate > complex.StartDate &&
                                      SelectedRepair.Dto.EndDate <= complex.EndDate
                                ? SelectedRepair.Dto.EndDate
                                : complex.EndDate;
                            EditRepairDates(new EditRepairDatesParameterSet
                            {
                                DateStart = start,
                                DateEnd = end,
                                DateType = DateTypes.Plan,
                                RepairId = SelectedRepair.Id
                            }, () => { }, Behavior);

                            // Добавить ремонт в комплекс
                            AddRepairToComplex(SelectedRepair, complex.Id);
                        },
                        Content = new TextBlock
                        {
                            TextWrapping = TextWrapping.Wrap,
                            Width = 300,
                            Text = msg
                        },
                        Header = "Добавление ремонта в комплекс",
                        OkButtonContent = "Да",
                        CancelButtonContent = "Нет"
                    });
            }
            else
            {
                // просто добавить и все
                AddRepairToComplex(SelectedRepair, complex.Id);
            }
        }

        /// <summary>
        ///     Добавить ремонт в локальный комплекс
        /// </summary>
        /// <param name="complex">комплекс</param>
        private void AddToLocalComplex(ComplexItem complex)
        {
            if (SelectedRepair.Dto.StartDate < complex.StartDate || SelectedRepair.Dto.EndDate > complex.EndDate)
            {
                var msg = "Вы добавляете ремонт в комплекс, "
                          + "при этом сроки проведения ремонта не соответсвуют срокам проведения комплекса."
                          + Environment.NewLine + Environment.NewLine
                          + "Что делать?";

                var vm = new QuestionViewModel(
                    num =>
                    {
                        switch (num)
                        {
                            case 1:
                                AddRepairToComplex(SelectedRepair, complex.Id);
                                break;

                            case 2:
                            {
                                // Изменить даты ремонта
                                var start = SelectedRepair.Dto.StartDate >= complex.StartDate &&
                                            SelectedRepair.Dto.StartDate < complex.EndDate
                                    ? SelectedRepair.Dto.StartDate
                                    : complex.StartDate;

                                var end = SelectedRepair.Dto.EndDate > complex.StartDate &&
                                          SelectedRepair.Dto.EndDate <= complex.EndDate
                                    ? SelectedRepair.Dto.EndDate
                                    : complex.EndDate;

                                EditRepairDates(new EditRepairDatesParameterSet
                                {
                                    DateStart = start,
                                    DateEnd = end,
                                    DateType = DateTypes.Plan,
                                    RepairId = SelectedRepair.Id
                                }, () => { }, Behavior);

                                // Добавить ремонт в комплекс
                                AddRepairToComplex(SelectedRepair, complex.Id);
                            }
                                break;

                            case 3:
                            {
                                var start = SelectedRepair.Dto.StartDate;
                                var end = SelectedRepair.Dto.EndDate - SelectedRepair.Dto.StartDate >
                                          complex.EndDate - complex.StartDate
                                    ? SelectedRepair.Dto.EndDate
                                    : start + (complex.EndDate - complex.StartDate);

                                MoveComplex(complex.Dto, start, end);

                                // Добавить ремонт в комплекс
                                AddRepairToComplex(SelectedRepair, complex.Id);
                                break;
                            }
                        }
                    })
                {
                    Header = "Добавление ремонта в комплекс",
                    Question = msg,
                    AnswerList = new List<Answer>
                    {
                        new Answer
                        {
                            Num = 1,
                            Text = "Только добавить"
                        },
                        new Answer
                        {
                            Num = 2,
                            Text = "Добавить и изменить даты ремонта"
                        },
                        new Answer
                        {
                            Num = 3,
                            Text = "Добавить и изменить даты комплекса"
                        },
                        new Answer
                        {
                            Num = 4,
                            Text = "Не добавлять"
                        }
                    }
                };

                var view = new QuestionView {DataContext = vm};
                view.ShowDialog();
            }
            else
            {
                AddRepairToComplex(SelectedRepair, complex.Id);
            }
        }

        private void OnAddToNewComplexCommandExecuted()
        {
            var viewModel = new AddEditComplexViewModel(
                id => AddRepairToComplex(SelectedRepair, id),
                SelectedYear,
                SelectedSystem.Id)
            {
                StartDate = SelectedRepair.StartDatePlan,
                EndDate = SelectedRepair.EndDatePlan
            };

            var view = new AddEditComplexView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnAddToComplexCommandExecuted(object param)
        {
            var complex = ComplexViewModel.GetComplexById((int) param);

            // При попытке включить в тот же комплекс
            if (SelectedRepair.ComplexId == complex.Id)
            {
                return;
            }

            // Дата поставки МТР не согласуется с датой начала комплекса
            if (SelectedRepair.Dto.PartsDeliveryDate > complex.EndDate)
            {
                MessageBoxProvider.Alert(
                    "Ремонт нельзя добавить в выбранный комплекс, так как даты проведения комплекса  не согласуются с датой поставки МТР.",
                    "Ошибка");
                return;
            }

            if (SelectedRepair.ComplexId > 0)
            {
                RadWindow.Confirm(
                    new DialogParameters
                    {
                        Closed = (s, e) =>
                        {
                            if (e.DialogResult.HasValue && e.DialogResult.Value)
                            {
                                if (complex.IsLocal)
                                {
                                    AddToLocalComplex(complex);
                                }
                                else
                                {
                                    AddToEnterpriseComplex(complex);
                                }
                            }
                        },
                        Content = new TextBlock
                        {
                            Text =
                                "Ремонт уже включен в другой комплекс, при добавлении в новый комплекс, он будет исключен из текущего комплекса. Добавить ремонт в новый комплекс?",
                            Width = 250,
                            TextWrapping = TextWrapping.Wrap
                        },
                        Header = string.Empty,
                        OkButtonContent = "Да",
                        CancelButtonContent = "Нет"
                    });
            }
            else if (complex.IsLocal)
            {
                AddToLocalComplex(complex);
            }
            else
            {
                AddToEnterpriseComplex(complex);
            }
        }

        private void OnAddExternalConditionCommandExecuted()
        {
            var viewModel = new AddEditExternalConditionViewModel(id => Refresh(), SelectedYear);
            var view = new AddEditExternalConditionView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnAddRepairCommandExecuted()
        {
            var viewModel = new AddEditRepairViewModel(id => Refresh(), SelectedYear);
            var view = new AddEditRepairView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnEditComplexBySelectedRepairCommandExecuted()
        {
            var viewModel = new AddEditComplexViewModel(id => Refresh(), SelectedRepair.Dto.Complex, SelectedYear);
            var view = new AddEditComplexView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void OnEditRepairCommandExecuted(RepairItem repairItem)

        {
            if (repairItem == null)
            {
                repairItem = SelectedRepair;
            }

            if (repairItem.IsExternalCondition)
            {
                var viewModel = new AddEditExternalConditionViewModel(id => Refresh(), repairItem.Dto, SelectedYear);
                var view = new AddEditExternalConditionView {DataContext = viewModel};
                view.ShowDialog();
            }
            else
            {
                var viewModel = new AddEditRepairViewModel(id => Refresh(), repairItem.Dto, SelectedYear);
                var view = new AddEditRepairView {DataContext = viewModel};
                view.ShowDialog();
            }
        }

        private void OnRemoveFromComplexCommandExecuted()
        {
            RadWindow.Confirm(
                new DialogParameters
                {
                    Closed = (s, e) =>
                    {
                        if (e.DialogResult.HasValue && e.DialogResult.Value)
                        {
                            AddRepairToComplex(SelectedRepair, null);
                        }
                    },
                    Content = "Вы уверены что хотите исключить работу из комплекса?",
                    Header = "Исключить из комплекса",
                    OkButtonContent = "Да",
                    CancelButtonContent = "Нет"
                });
        }

        private void OnRemoveRepairCommandExecuted(RepairItem repairItem)
        {
            var dp = new DialogParameters
            {
                Closed = async (s, e) =>
                {
                    if (e.DialogResult.HasValue && e.DialogResult.Value)
                    {
                        var repairId = repairItem.Id;
                        Behavior.TryLock();
                        try
                        {
                            await new RepairsServiceProxy().DeleteRepairAsync(repairId);
                            var sr = _repairList.Single(r => r.Id == repairId);
                            _repairList.Remove(sr);
                            // Удалить работу из грида
                            if (RepairList.Contains(repairItem))
                            {
                                RepairList.Remove(repairItem);
                            }

                            // Удалить с диаграммы Ганта
                            var group = PlanGanttViewModel.GanttRepairList.Single(g => g.Title == sr.GroupObject);
                            group.Children.Remove(
                                group.Children.Single(r => ((GanttRepairTask) r).RepairItem == sr));
                        }
                        finally
                        {
                            Behavior.TryUnlock();
                        }
                    }
                },
                Content = "Вы уверены что хотите удалить ремонт?",
                Header = "Удаление ремонта",
                OkButtonContent = "Да",
                CancelButtonContent = "Нет"
            };

            RadWindow.Confirm(dp);
        }

        private void OnShowUpdateHistoryCommandExecuted()
        {
            var viewModel = new RepairUpdateHistoryViewModel(SelectedRepair.Id);
            var view = new RepairUpdateHistoryView {DataContext = viewModel};
            view.ShowDialog();
        }

        private void RefreshCommands()
        {
            EditRepairCommand.RaiseCanExecuteChanged();
            RemoveRepairCommand.RaiseCanExecuteChanged();
            EditComplexBySelectedRepairCommand.RaiseCanExecuteChanged();
            ShowUpdateHistoryCommand.RaiseCanExecuteChanged();

            ComplexViewModel.RefreshCommands();

            AddToComplexCommand.RaiseCanExecuteChanged();
            AddToNewComplexCommand.RaiseCanExecuteChanged();
            RemoveFromComplexCommand.RaiseCanExecuteChanged();
        }

        private void ExportToExcel()
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
                var excelReport = new ExcelReport("ППР");
                var date = DateTime.Now;
                excelReport.Write("Дата:").Write(date.Date).NewRow();
                excelReport.Write("Время:").Write(date.ToString("HH:mm")).NewRow();
                excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
                excelReport.Write("План ремонтных работ:").Write(_selectedSystem.Name + ", " + _selectedYear).NewRow();
                excelReport.NewRow();
                excelReport.WriteHeader("Гр.объект", 150);
                excelReport.WriteHeader("Объект", 150);
                excelReport.WriteHeader("Работы", 250);
                excelReport.WriteHeader("ЛПУ", 250);   
                excelReport.WriteHeader("Вид", 120);
                excelReport.WriteHeader("Начало", 80);
                excelReport.WriteHeader("Окончание", 80);
                excelReport.WriteHeader("Длительность", 100);
                excelReport.WriteHeader("Описание", 450);
                excelReport.WriteHeader("Технологический коридор", 450);
                excelReport.WriteHeader("Комплес", 150);
                excelReport.WriteHeader("Способ ведения работ", 150);
                excelReport.WriteHeader("Дата поставки МТР", 250);
                excelReport.WriteHeader("Объем стравливаемого газа, млн.м³", 250);
                excelReport.WriteHeader("Объем выработанного газа, млн.м³", 250);
                excelReport.WriteHeader("Достигнутый объем транспорта газа на участке, млн.м³/сут\nЗима\nЛето\nМежсезонье", 450);
                excelReport.WriteHeader("Расчетная пропускная способность участка, млн.м³/сут\nЗима\nЛето\nМежсезонье", 450);
                excelReport.WriteHeader("Расчетный объем транспорта газа на период проведения работ, млн.м³/сут", 250);
                excelReport.WriteHeader("Примечания от ГТП", 300);
                excelReport.WriteHeader("Изменено", 250);
                excelReport.NewRow();
                foreach (RepairItem r in RepairList)
                {
                    excelReport.WriteCell(r.DefaultGroupName);
                    excelReport.WriteCell(r.ObjectName);
                    excelReport.WriteCell(r.RepairWorks);
                    excelReport.WriteCell(r.SiteName);
                    excelReport.WriteCell(r.RepairTypeName);
                    excelReport.WriteCell(r.StartDatePlan);
                    excelReport.WriteCell(r.EndDatePlan);
                    excelReport.WriteCell(String.Format("{0:d} ч.", r.DurationPlan));
                    excelReport.WriteCell(r.Description);
                    excelReport.WriteCell(r.PipelineGroupName);
                    excelReport.WriteCell(r.ComplexName);
                    excelReport.WriteCell(r.ExecutionMeansName);
                    excelReport.WriteCell(r.PartsDeliveryDateString);
                    excelReport.WriteCell(r.BleedAmount);
                    excelReport.WriteCell(r.SavingAmount);
                    excelReport.WriteCell(r.MaxTransferWinter + "\n" + r.MaxTransferSummer + "\n" + r.MaxTransferTransition);
                    excelReport.WriteCell(r.CapacityWinter + "\n" + r.CapacitySummer + "\n" + r.CapacityTransition);
                    excelReport.WriteCell(r.CalculatedTransfer);
                    excelReport.WriteCell(r.CommentGto);
                    excelReport.WriteCell(String.Format("{0:dd.MM.yyyy} {0:HH:mm}", r.LastUpdate) + "\n" + r.UserName + "\n" + r.UserSiteName);
                    excelReport.NewRow();
                }

                using (var stream = dialog.OpenFile())
                {
                    excelReport.Save(stream);
                }
            }
        }

        /// <summary>
        ///     Обновляет коллекции ремонтных работ на клиенте
        /// </summary>
        private void UpdateClientRepairList()
        {
            var sr = SelectedRepair;

            RepairList.Clear();
            var rl = ShowNonCritical ? _repairList : _repairList.Where(r => r.IsCritical);
            if (IsCurrentYearSelected && PeriodType == PeriodType.NearFuture)
            {
                rl = rl.Where(r => r.StartDatePlan < DateTime.Today.AddDays(14) && r.EndDatePlan > DateTime.Today);
            }
            RepairList.AddRange(rl);

            RefreshGantt(RepairList.ToList());
            RefreshScheme();
            SelectedRepair = sr != null && RepairList.Count > 0 ? RepairList.SingleOrDefault(r => r.Id == sr.Id) : null;
        }

        /// <summary>
        ///     Сохранить выбранный этап планнирования в БД
        /// </summary>
        private async void UpdatePlanningStage()
        {
            Behavior.TryLock();
            try
            {
                await new RepairsServiceProxy().SetPlanningStageAsync(new SetPlanningStageParameterSet
                {
                    Year = SelectedYear,
                    SystemId = SelectedSystem.Id,
                    Stage = PlanningStage
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            Refresh();
        }
    }
}