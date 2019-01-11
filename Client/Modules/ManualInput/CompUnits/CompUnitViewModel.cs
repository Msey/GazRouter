using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.Common.Events;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.Controls.Converters;

namespace GazRouter.ManualInput.CompUnits
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class CompUnitViewModel : LockableViewModel
    {
        private bool _isEditPermission;
        void ExportToExcel()
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
                var date = DateTime.Now;
                var excelReport = new ExcelReport("Состояния ГПА ");
                excelReport.Write("Дата:").Write(date.Date).NewRow();
                excelReport.Write("Время:").Write(date.ToString("HH:mm")).NewRow();
                excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
                excelReport.Write("ЛПУ:").Write(SelectedSite.Name).NewRow();
                excelReport.NewRow();
                excelReport.WriteHeader("КС", 200);
                excelReport.WriteHeader("КЦ", 100);
                excelReport.WriteHeader("ГПА", 100);
                excelReport.WriteHeader("Состояние", 100);
                excelReport.WriteHeader("Дата изменения состояния", 110);
                excelReport.WriteHeader("Дополнительная информация", 150);
                excelReport.WriteHeader("Информация о вынужденном или аварийном останове", 150);
                excelReport.WriteHeader("Изменено", 150);
                excelReport.WriteHeader("Внешнее проявление", 250);
                excelReport.WriteHeader("Описание причины", 250);
                excelReport.WriteHeader("Выполненные работы", 250);
                excelReport.WriteHeader("Зависимые пуски", 250);
                excelReport.WriteHeader("Прикрепленные документы", 250);
                var stname = "";
                var shname = "";
                foreach (var station in Items)
                {
                    var stationItem = station as GridItem;
                    stname = stationItem.Name;
                    foreach (var shop in stationItem.Children)
                    {
                        var shopItem = shop as GridItem;
                        shname = shopItem.Name;
                        foreach (var unit in shopItem.Children.Where(s => s.StateDto!=null))
                        {
                            var unitItem = unit as GridItem;
                            excelReport.NewRow();
                            excelReport.WriteCell(stname);
                            excelReport.WriteCell(shname);
                            excelReport.WriteCell(unitItem.Name + "\n" + unitItem.CompUnitTypeName);
                            excelReport.WriteCell(new CompUnitStateToNameConverter().Convert(unitItem.State, typeof(string), null, null));
                            excelReport.WriteCell($"{unitItem.StateChangeDate:dd.MM.yyyy HH:mm}");
                            var dop = "";
                            if (unitItem.IsRepair)
                            {
                                dop += new CompUnitRepairTypeToNameConverter().Convert(unitItem.StateDto.RepairType, typeof(string), null, null);
                                if (unitItem.StateDto.CompletionDatePlan != null)
                                    dop += $"\nдо {unitItem.StateDto.CompletionDatePlan:dd.MM.yyyy}";
                                if (unitItem.StateDto.IsDelayed) dop += "\n(Нарушены сроки)";
                            }
                            if (unitItem.IsReserve)
                                if (unitItem.StateDto.IsRepairNext)
                                    dop += "\nРезерв с последующим ремонтом";
                            var fail = "";
                            if (unitItem.IsFailure)
                            {
                                fail += new CompUnitStopTypeToNameConverter().Convert(unitItem.StateDto.StopType, typeof(string), null, null) + " останов";
                                if (unitItem.IsFailure)
                                {
                                    fail += "\n" + new CompUnitFailureFeatureToNameConverter().Convert(unitItem.StateDto.FailureDetails.FailureFeature, typeof(string), null, null)
                                             + "\n" + new CompUnitFailureCauseToNameConverter().Convert(unitItem.StateDto.FailureDetails.FailureCause, typeof(string), null, null);
                                    if (unitItem.StateDto.FailureDetails.IsCritical)
                                        fail += "\nВлияет на транспорт газа";
                                }
                            }

                            var changed = unitItem.StateDto.UserName + "\n" + unitItem.StateDto.UserSite;
                            excelReport.WriteCell(dop);
                            excelReport.WriteCell(fail);
                            excelReport.WriteCell(changed);

                            if (unit.StateDto.FailureDetails!=null)
                            {
                                var ext = unitItem.StateDto.FailureDetails.FailureExternalView;
                                var cause = unitItem.StateDto.FailureDetails.FailureCauseDescription;
                                var work = unitItem.StateDto.FailureDetails.FailureWorkPerformed;
                                var start = "";
                                foreach (var v in unitItem.StateDto.FailureDetails.UnitStartList)
                                {
                                    start += v.StateChangeDate.ToString("dd.MM.yyyy HH:mm") + "\n" + v.CompUnitName + "\n" + new CompUnitTypeToNameConverter().Convert(v.CompUnitTypeId, typeof(string), null, null) + "\n";
                                }
                                var docs = "";
                                foreach (var v in unitItem.StateDto.FailureDetails.AttachmentList)
                                {
                                    docs += v.Description + "\n" + v.FileName + "\n" + DataLengthConverter.Convert(v.DataLength) + "\n";
                                }
                                excelReport.WriteCell(ext);
                                excelReport.WriteCell(cause);
                                excelReport.WriteCell(work);
                                excelReport.WriteCell(start);
                                excelReport.WriteCell(docs);
                            }
                        }
                    }
                }
                using (var stream = dialog.OpenFile())
                {
                    excelReport.Save(stream);
                }
            }
        }

        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }

        public CompUnitViewModel()
        {
            // Todo: Нужно добавить во все команды проверку на закрытие сеанса и запретить менять данные если сеанс уже закрыт.
            // Для этой цели нужно реализовать панель управления сеансом и статусную схему сеанса.
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.CompUnit);

            RefreshCommand = new DelegateCommand(() => Refresh(SelectedItem?.Id ?? Guid.Empty));

            ExportExcelCommand = new DelegateCommand(() => ExportToExcel());

            AddNewStateCommand = new DelegateCommand(AddNewState,
                () => SelectedItem != null && SelectedItem.EntityType == EntityType.CompUnit && IsEditPermission);

            UpdateStateInfoCommand = new DelegateCommand(UpdateStateInfo,
                () => SelectedItem != null && SelectedItem.EntityType == EntityType.CompUnit && IsEditPermission);

            ToPrevStateCommand = new DelegateCommand(ToPrevState,
                () =>
                    SelectedItem != null && SelectedItem.EntityType == EntityType.CompUnit &&
                    SelectedItem.CanDeleteState && IsEditPermission);

            FindFailureDependencesCommand = new DelegateCommand(() => Refresh(SelectedItem.Id),
                () => SelectedItem != null && SelectedItem.IsFailure && IsEditPermission);

            AddFailureAttachmentCommand = new DelegateCommand(AddFailureAttachment,
                () => SelectedItem != null && SelectedItem.IsFailure && IsEditPermission);

            DeleteFailureAttachmentCommand = new DelegateCommand<AttachmentBaseDTO>(DeleteFailureAttachment);

            DeleteFailureDependencyCommand = new DelegateCommand<int?>(DeleteFailureDependency);

            LoadSiteList();
        }



        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand ExportExcelCommand { get; set; }
        public DelegateCommand AddNewStateCommand { get; set; }
        public DelegateCommand UpdateStateInfoCommand { get; set; }
        public DelegateCommand ToPrevStateCommand { get; set; }
        public DelegateCommand FindFailureDependencesCommand { get; set; }
        public DelegateCommand AddFailureAttachmentCommand { get; set; }
        public DelegateCommand<AttachmentBaseDTO> DeleteFailureAttachmentCommand { get; set; }
        public DelegateCommand<int?> DeleteFailureDependencyCommand { get; set; }




        private async void LoadSiteList()
        {

            if (UserProfile.Current.Site.IsEnterprise)
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet {EnterpriseId = UserProfile.Current.Site.Id});
            else
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet {SiteId = UserProfile.Current.Site.Id});

            OnPropertyChanged(() => SiteList);
            if (SiteList.Any()) SelectedSite = SiteList.First();

        }





        private async void Refresh(Guid id, bool findDependences = false)
        {
            if (SelectedSite == null) return;

            try
            {
                Behavior.TryLock();

                // Получить список ГПА
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);

                // Получить текущие состояния ГПА
                var states = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                    new GetCompUnitStateListParameterSet
                    {
                        SiteId = SelectedSite.Id
                    });

                if (findDependences && id != Guid.Empty)
                    FindDependences(stationTree, states, states.Single(s => s.CompUnitId == id).Id);

                // Сформировать дерево
                Items = new List<GridItem>();
                GridItem tmp = null;

                foreach (var station in stationTree.CompStations)
                {
                    var stationItem = new GridItem(station);
                    Items.Add(stationItem);

                    foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                    {
                        var shopItem = new GridItem(shop);
                        stationItem.Children.Add(shopItem);

                        foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                        {
                            shopItem.Children.Add(new GridItem(unit,
                                states.SingleOrDefault(s => s.CompUnitId == unit.Id)));
                            if (id != Guid.Empty && tmp == null)
                                tmp = shopItem.Children.FirstOrDefault(i => i.Id == id);
                        }
                    }
                }

                OnPropertyChanged(() => Items);

                SelectedItem = tmp;

            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void RefreshState(Guid id, bool findDependences = false)
        {
            if (SelectedSite == null) return;

            try
            {
                Behavior.TryLock();

                // Получить список ГПА
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);

                // Получить текущие состояния ГПА
                var states = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                    new GetCompUnitStateListParameterSet
                    {
                        SiteId = SelectedSite.Id
                    });
                //GridItem tmp = new GridItem()
                foreach (var station in Items)
                    foreach (var shop in station.Children)
                        foreach (var unit in shop.Children.Where(s => s.Id == SelectedItem.Id))
                        {
                            var tmp = new GridItem(unit.EntityDto, states.SingleOrDefault(s => s.CompUnitId == unit.Id));
                            unit.StateDto = tmp.StateDto;
                            unit.StateChangeDate = tmp.StateChangeDate;
                            unit.IsCritical = tmp.IsCritical;
                            unit.IsFailure = tmp.IsFailure;
                            unit.IsReserve = tmp.IsReserve;
                            unit.IsRepair = tmp.IsRepair;
                            unit.State = tmp.State;
                        }
                RefreshCommands();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }


        private void FindDependences(TreeDataDTO tree, List<CompUnitStateDTO> states, int stateId)
        {
            if (stateId == 0) return;

            const int startDelayMax = 60;

            var state = states.Single(s => s.Id == stateId);

            #region Поиск зависимосте при пуске ГПА

            if (state.State == CompUnitState.Work)
            {
                var unit = tree.CompUnits.Single(u => u.Id == state.CompUnitId);

                // находим станцию на которой запущен агрегат
                var stationId = tree.CompShops.Single(s => s.Id == unit.ParentId).ParentId;

                // теперь нужно найти все агрегаты относящиеся к той же станции, что и запущенный агрегат
                var shopList = tree.CompShops.Where(s => s.ParentId == stationId);
                var unitList = tree.CompUnits.Where(u => shopList.Any(s => s.Id == u.ParentId));

                // найти по этим агрегатам недавние остановы
                var failures =
                    states.Where(
                        s =>
                            s.StopType.HasValue &&
                            s.StopType.Value != CompUnitStopType.Planned &&
                            s.StateChangeDate < state.StateChangeDate &&
                            (state.StateChangeDate - s.StateChangeDate).TotalMinutes < startDelayMax &&
                            unitList.Any(u => u.Id == s.CompUnitId)).ToList();

                if (!failures.Any()) return;

                var dp = failures.Select(f => new Dependency
                {
                    UnitName = tree.CompUnits.Single(u => u.Id == f.CompUnitId).ShortPath,
                    UnitType =
                        ClientCache.DictionaryRepository.CompUnitTypes.Single(
                            t => t.Id == tree.CompUnits.Single(u => u.Id == f.CompUnitId).CompUnitTypeId).Name,
                    Detail = f
                }).ToList();

                RadWindow.Confirm(new DialogParameters
                {
                    Content = new AddFailureDependencesView {DataContext = new AddFailureDependencesViewModel(dp, true)},
                    Header = "Возможная взаимосвязь",
                    OkButtonContent = "Сохранить",
                    CancelButtonContent = "Закрыть",
                    Closed = async (s, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            foreach (var f in dp.Where(o => o.IsDepend))
                            {
                                await new ManualInputServiceProxy().AddFailureRelatedCompUnitStartAsync(
                                    new AddFailureRelatedUnitStartParameterSet
                                    {
                                        StateChangeId = stateId,
                                        FailureDetailId = f.Detail.Id
                                    });
                            }
                            if (dp.Any(o => o.IsDepend))
                                Refresh(SelectedItem.Id);
                        }
                    }
                });
            }

            #endregion

            #region Поиск зависимостей при останове ГПА

            if (state.StopType.HasValue && state.StopType != CompUnitStopType.Planned)
            {
                var unit = tree.CompUnits.Single(u => u.Id == state.CompUnitId);

                // находим станцию на которой остановлен агрегат
                var stationId = tree.CompShops.Single(s => s.Id == unit.ParentId).ParentId;

                // теперь нужно найти все агрегаты относящиеся к той же станции, что и остановленный агрегат
                var shopList = tree.CompShops.Where(s => s.ParentId == stationId);
                var unitList = tree.CompUnits.Where(u => shopList.Any(s => s.Id == u.ParentId));

                // найти по этим агрегатам недавние пуски
                var starts = states.Where(s =>
                    s.State == CompUnitState.Work &&
                    s.StateChangeDate > state.StateChangeDate &&
                    (s.StateChangeDate - state.StateChangeDate).TotalMinutes < startDelayMax &&
                    unitList.Any(u => u.Id == s.CompUnitId)).ToList();

                if (!starts.Any()) return;

                var dp = starts.Select(s => new Dependency
                {
                    UnitName = tree.CompUnits.Single(u => u.Id == s.CompUnitId).ShortPath,
                    UnitType =
                        ClientCache.DictionaryRepository.CompUnitTypes.Single(
                            t => t.Id == tree.CompUnits.Single(u => u.Id == s.CompUnitId).CompUnitTypeId).Name,
                    Detail = s
                }).ToList();

                RadWindow.Confirm(new DialogParameters
                {
                    Content =
                        new AddFailureDependencesView {DataContext = new AddFailureDependencesViewModel(dp, false)},
                    Header = "Возможная взаимосвязь",
                    OkButtonContent = "Сохранить",
                    CancelButtonContent = "Закрыть",
                    Closed = async (s, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            foreach (var st in dp.Where(o => o.IsDepend))
                            {
                                await new ManualInputServiceProxy().AddFailureRelatedCompUnitStartAsync(
                                    new AddFailureRelatedUnitStartParameterSet
                                    {
                                        StateChangeId = st.Detail.Id,
                                        FailureDetailId = state.Id
                                    });
                            }
                            if (dp.Any(o => o.IsDepend))
                                Refresh(SelectedItem.Id);
                        }
                    }
                });

            }

            #endregion
        }


        private void AddNewState()
        {
            var vm = new AddEditCompUnitStateViewModel(SelectedItem.StateDto ?? new CompUnitStateDTO {CompUnitId = SelectedItem.EntityDto.Id}, id => RefreshState(SelectedItem.Id, true),
                false);
            var v = new AddEditCompUnitStateView {DataContext = vm};
            v.ShowDialog();
        }

        private void UpdateStateInfo()
        {
            var vm = new AddEditCompUnitStateViewModel(SelectedItem.StateDto, id => RefreshState(SelectedItem.Id), true);
            var v = new AddEditCompUnitStateView {DataContext = vm};
            v.ShowDialog();
        }

        private async void ToPrevState()
        {
            await new ManualInputServiceProxy().DeleteCompUnitStateAsync(SelectedItem.StateDto.Id);
            RefreshState(SelectedItem.Id);
        }


        private void AddFailureAttachment()
        {
            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = (AddEditAttachmentViewModel) obj;
                if (x.DialogResult.HasValue && x.DialogResult.Value)
                {
                    await new ManualInputServiceProxy().AddFailureAttachmentAsync(
                        new AddAttachmentParameterSet<int>
                        {
                            Description = x.Description,
                            Data = x.FileData,
                            FileName = x.FileName,
                            ExternalId = SelectedItem.StateDto.Id
                        });
                    Refresh(SelectedItem.Id);
                }
            });
            var v = new AddEditAttachmentView {DataContext = vm};
            v.ShowDialog();
        }


        private void DeleteFailureAttachment(AttachmentBaseDTO dto)
        {
            var d = dto as AttachmentDTO<int, int>;
            if (d != null)
            {
                RadWindow.Confirm(new DialogParameters
                {
                    Header = "Подтверждение",
                    Content = new TextBlock
                    {
                        Text = "Внимание! Удаляем прикрепленный документ. Необходимо Ваше подтверждение.",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    OkButtonContent = "Удалить",
                    CancelButtonContent = "Отмена",
                    Closed = async (obj, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            await new ManualInputServiceProxy().DeleteFailureAttachmentAsync(d.Id);
                            Refresh(SelectedItem.Id);
                        }
                    }
                });

            }
        }

        private void DeleteFailureDependency(int? id)
        {
            if (id.HasValue)
            {
                RadWindow.Confirm(new DialogParameters
                {
                    Header = "Подтверждение",
                    Content = new TextBlock
                    {
                        Text =
                            "Внимание! Разрываем связь между остановом и пуском агрегата. Необходимо Ваше подтверждение.",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    OkButtonContent = "Разоравать",
                    CancelButtonContent = "Отмена",
                    Closed = async (obj, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            await new ManualInputServiceProxy().DeleteFailureRelatedCompUnitStartAsync(
                                new AddFailureRelatedUnitStartParameterSet
                                {
                                    FailureDetailId = SelectedItem.StateDto.Id,
                                    StateChangeId = id.Value
                                });
                            Refresh(SelectedItem.Id);
                        }
                    }
                });

            }
        }

        private void RefreshCommands()
        {
            AddNewStateCommand.RaiseCanExecuteChanged();
            UpdateStateInfoCommand.RaiseCanExecuteChanged();
            ToPrevStateCommand.RaiseCanExecuteChanged();
            AddFailureAttachmentCommand.RaiseCanExecuteChanged();
            FindFailureDependencesCommand.RaiseCanExecuteChanged();
        }


        public List<SiteDTO> SiteList { get; set; }

        private SiteDTO _selectedSite;
        private DateTime _timestamp;
        private GridItem _selectedItem;

        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                _selectedSite = value;
                OnPropertyChanged(() => SelectedSite);
                Refresh(Guid.Empty);
            }
        }


        public List<GridItem> Items { get; set; }

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                _timestamp = value;
                OnPropertyChanged(() => Timestamp);
            }
        }

        public GridItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
                RefreshCommands();
            }
        }
    }

    public class GridItem : PropertyChangedBase
    {
        private CommonEntityDTO _entityDto;
        private CompUnitStateDTO _stateDto;

        public GridItem(CommonEntityDTO entityDto, CompUnitStateDTO stateDto = null)
        {
            _entityDto = entityDto;
            _stateDto = stateDto;
            _isFailure = _stateDto?.StopType == CompUnitStopType.Forced || _stateDto?.StopType == CompUnitStopType.Emergency;
            _isRepair = _stateDto?.State == CompUnitState.Repair;
            _isReserve = _stateDto?.State == CompUnitState.Reserve;
            _isCritical = IsFailure && _stateDto.FailureDetails.IsCritical;
            _stateChangeDate = _stateDto?.StateChangeDate;
            _state = _stateDto?.State == CompUnitState.Undefined ? null : _stateDto?.State;
            Children = new List<GridItem>();
        }

        public CommonEntityDTO EntityDto { get { return _entityDto; } set { _entityDto = value; } }
        public CompUnitStateDTO StateDto { get { return _stateDto; } set { _stateDto = value; OnPropertyChanged(() => StateDto); } }

        public Guid Id => _entityDto.Id;

        public string Name => _entityDto.Name;

        [Display(AutoGenerateField = false)]
        public List<GridItem> Children { get; set; }

        private CompUnitState? _state;
        public CompUnitState? State { get { return _state; } set { _state = value; OnPropertyChanged(() => State); } }

        private DateTime? _stateChangeDate;

        public DateTime? StateChangeDate { get { return _stateChangeDate; } set { _stateChangeDate = value; ; OnPropertyChanged(() => StateChangeDate); } } 

        /// <summary>
        /// Тип нагнетателя
        /// </summary>
        public string SuperchargerTypeName
        {
            get
            {
                if (_entityDto.EntityType != EntityType.CompUnit) return "";
                var unit = (CompUnitDTO) _entityDto;
                return
                    ClientCache.DictionaryRepository.SuperchargerTypes.Single(st => st.Id == unit.SuperchargerTypeId)
                        .Name;
            }
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        /// <summary>
        /// Тип ГПА
        /// </summary>
        public string CompUnitTypeName
        {
            get
            {
                if (_entityDto.EntityType != EntityType.CompUnit) return "";
                var unit = (CompUnitDTO) _entityDto;
                return ClientCache.DictionaryRepository.CompUnitTypes.Single(t => t.Id == unit.CompUnitTypeId).Name;
            }
        }

        public EntityType EntityType => _entityDto.EntityType;

        private bool _isFailure;
        public bool IsFailure { get { return _isFailure; } set { _isFailure = value; OnPropertyChanged(() => IsFailure); } }

        private bool _isRepair;
        public bool IsRepair { get { return _isRepair; } set { _isRepair = value; OnPropertyChanged(() => IsRepair); } }

        private bool _isReserve;
        public bool IsReserve { get { return _isReserve; } set { _isReserve = value; OnPropertyChanged(() => IsReserve); } }

        private bool _isCritical;
        public bool IsCritical { get { return _isCritical; } set { _isCritical = value; OnPropertyChanged(() => IsCritical); } }

        public bool CanDeleteState => (DateTime.Now - StateChangeDate)?.TotalHours < 12;
    }


}