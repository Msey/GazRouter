using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using DataExchange.CustomSource.Dialogs;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DataExchange.CustomSource.Dialogs;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using EntityPickerDialogView = GazRouter.Controls.Dialogs.EntityPicker.EntityPickerDialogView;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
namespace GazRouter.DataExchange.CustomSource
{
    public class CustomSourceViewModel : LockableViewModel
    {
        
        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand AddDataSourceCommand { get; private set; }
        public DelegateCommand AddExchangeTaskCommand { get; private set; }
        public DelegateCommand AddExchangeEntityCommand { get; private set; }
        public DelegateCommand EditCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand RunCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CheckXslCommand { get; set; }

        private Period _selectedPeriod = new Period(SeriesHelper.GetCurrentSession().AddDays(-1), SeriesHelper.GetCurrentSession());
        public Period SelectedPeriod
        {
            get
            {
                return _selectedPeriod;
            }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                    GetExchangeLog(_selectedItem); 
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(() => IsEnabled);
            }
        }
        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(() => IsReadOnly);
            }
        }
        public CustomSourceViewModel()
        {
            IsEnabled  = Authorization2.Inst.IsEditable(LinkType.CustomSource);
            IsReadOnly = !IsEnabled;

            Timestamp = SeriesHelper.GetLastCompletedSession();

            RefreshCommand = new DelegateCommand(Refresh, () => IsEnabled);
            AddDataSourceCommand = new DelegateCommand(AddDataSource, () => IsEnabled);
            AddExchangeTaskCommand = new DelegateCommand(AddExchangeTask, () => IsEnabled && SelectedItem is DataSourceItem);
            AddExchangeEntityCommand = new DelegateCommand(AddExchangeEntity, () => IsEnabled && SelectedItem is TaskItem);
            EditCommand = new DelegateCommand(Edit, () => IsEnabled && SelectedItem != null);
            DeleteCommand = new DelegateCommand(Delete, () => IsEnabled && SelectedItem != null && !(SelectedItem is PropertyItem));
            RunCommand = new DelegateCommand(Run, () => IsEnabled && IsExportTaskSelected);
            SaveCommand = new DelegateCommand(SaveAs, () => IsEnabled && IsExportTaskSelected);
            CheckXslCommand = new DelegateCommand(CheckXsl, () => IsEnabled && SelectedItem is TaskItem && (SelectedItem as TaskItem).IsImport);
            
            Refresh();
        }

        private void CheckXsl()
        {
            var vm = new CheckXslViewModel(((TaskItem)SelectedItem).Dto);
            var v = new CheckXslView(){ DataContext = vm };
            v.ShowDialog();
        }

        private async void Refresh()
        {

            Timestamp = SeriesHelper.GetLastCompletedSession();
            try
            {
                Behavior.TryLock();

                var oldSelection = SelectedItem;
                ItemBase restoredSelection = null;
                
                // Получить список источников данных
                var sourceList = await new DataExchangeServiceProxy().GetDataSourceListAsync(null);

                // Получить список заданий
                var taskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(null);

                // Получить список объектов для обмена
                var entityList = await new DataExchangeServiceProxy().GetExchangeEntityListAsync(
                    new GetExchangeEntityListParameterSet
                    {
                        ExchangeTaskIdList = taskList.Select(t => t.Id).ToList()
                    });

                // Получить список биндингов для свойств объекта
                var propList = await new DataExchangeServiceProxy().GetExchangePropertyListAsync(
                    new GetExchangeEntityListParameterSet
                    {
                        ExchangeTaskIdList = taskList.Select(t => t.Id).ToList()
                    });


                
                Items = new List<ItemBase>();
                
                foreach (var srcDto in sourceList)
                {
                    var srcItem = new DataSourceItem(srcDto) {IsEnabled = IsEnabled};
                    Items.Add(srcItem);

                    // восстановление выделения
                    if (restoredSelection == null && srcItem.IsSame(oldSelection))
                        restoredSelection = srcItem;

                    foreach (var taskDto in taskList.Where(t => t.DataSourceId == srcDto.Id))
                    {
                        var taskItem = new TaskItem(taskDto) { IsEnabled = IsEnabled };
                        srcItem.Children.Add(taskItem);

                        // восстановление выделения
                        if (restoredSelection == null && taskItem.IsSame(oldSelection))
                            restoredSelection = taskItem;

                        foreach (var entityDto in entityList.Where(e => e.ExchangeTaskId == taskDto.Id))
                        {
                            var entityItem = new EntityItem(entityDto, UpdateEntityBinding) { IsEnabled = IsEnabled };
                            taskItem.Children.Add(entityItem);

                            // восстановление выделения
                            if (restoredSelection == null && entityItem.IsSame(oldSelection))
                                restoredSelection = entityItem;

                            var ptList =
                                ClientCache.DictionaryRepository.EntityTypes.Single(
                                    et => et.Id == (int)entityDto.EntityTypeId).EntityProperties;
                            foreach (var pt in ptList)
                            {
                                var propDto =
                                    propList.FirstOrDefault(
                                        b =>
                                            b.ExchangeTaskId == entityDto.ExchangeTaskId &&
                                            b.EntityId == entityDto.EntityId && b.PropertyTypeId == pt.PropertyType);
                                var propItem = new PropertyItem(entityDto, pt.PropertyType,
                                    propDto != null ? propDto.ExtId : "", UpdatePropertyBinding){ IsEnabled = IsEnabled };
                                
                                entityItem.Children.Add(propItem);

                                // восстановление выделения
                                if (restoredSelection == null && propItem.IsSame(oldSelection))
                                    restoredSelection = propItem;
                            }
                        }
                    }
                }
                
                OnPropertyChanged(() => Items);
                SelectedItem = restoredSelection;


            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void RefreshCommands()
        {
            AddExchangeTaskCommand.RaiseCanExecuteChanged();
            AddExchangeEntityCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }


        private void AddDataSource()
        {
            var vm = new AddEditDataSourceViewModel(x => Refresh());
            var v = new AddEditDataSourceView {DataContext = vm};
            v.ShowDialog();
        }


        private void AddExchangeTask()
        {
            var vm = new AddEditExchangeTaskViewModel(x => Refresh(), ((DataSourceItem)SelectedItem).Dto.Id);
            var v = new AddEditExchangeTaskView {DataContext = vm};
            v.ShowDialog();
        }

        private EntityPickerDialogViewModel _epvm;
        private void AddExchangeEntity()
        {
            _epvm = new EntityPickerDialogViewModel(async () =>
            {
                if (_epvm.DialogResult.HasValue && _epvm.DialogResult.Value && _epvm.SelectedItem != null)
                {
                    var task = SelectedItem as TaskItem;
                    // Здесь проверяем на всякий случай, что такого объета еще нет в задании,
                    // что бы не нарваться на ошибку целостности
                    // если есть, просто переходим на него
                    if (task != null)
                    {
                        if (task.Children.OfType<EntityItem>().All(e => e.Dto.EntityId != _epvm.SelectedItem.Id))
                        {
                            await new DataExchangeServiceProxy().AddExchangeEntityAsync(
                                new AddEditExchangeEntityParameterSet
                                {
                                    EntityId = _epvm.SelectedItem.Id,
                                    ExchangeTaskId = task.Dto.Id,
                                    IsActive = true
                                });
                            Refresh();
                        }
                        else
                        {
                            SelectedItem =
                                task.Children.Single(e => ((EntityItem) e).Dto.EntityId == _epvm.SelectedItem.Id);

                            MessageBoxProvider.Alert(
                                "Объект, который Вы хотели добавить, уже был добавлен ранее. Сейчас выделение установлено на него.",
                                "Невозможно добавить объект");
                        }
                    }
                }
            }, 
            null);
            var v = new EntityPickerDialogView {DataContext = _epvm};
            v.ShowDialog();
        }


        private void Edit()
        {
            // Редактирование источников данных
            if (SelectedItem is DataSourceItem)
            {
                var vm = new AddEditDataSourceViewModel(x => Refresh(), ((DataSourceItem)SelectedItem).Dto);
                var v = new AddEditDataSourceView {DataContext = vm};
                v.ShowDialog();
            }

            // Редактирование заданий
            if (SelectedItem is TaskItem)
            {
                var vm = new AddEditExchangeTaskViewModel(x => Refresh(), ((TaskItem)SelectedItem).Dto);
                var v = new AddEditExchangeTaskView { DataContext = vm };
                v.ShowDialog();
            }
        }

        private void Delete()
        {
            // Удаление источников данных
            var item = SelectedItem as DataSourceItem;
            if (item != null)
            {
                MessageBoxProvider.Confirm(new TextBlock
                {
                    Text =
                        "Внимание! Удаляем источник данных, со всеми вложенными объектами. Необходимо Ваше подтверждение.",
                    TextWrapping = TextWrapping.Wrap,
                    Width = 250
                }, async confirmed =>
                {
                    if (!confirmed) return;

                    await new DataExchangeServiceProxy().DeleteDataSourceAsync(item.Dto.Id);
                    Refresh();
                },  "Подтверждение", "Удалить", "Отмена");
               
            }

            // Удаление задания 
            if (SelectedItem is TaskItem)
            {
                MessageBoxProvider.Confirm( new TextBlock
                {
                    Text = "Внимание! Удаляем задание, со всеми вложенными объектами. Необходимо Ваше подтверждение.",
                    TextWrapping = TextWrapping.Wrap,
                    Width = 250
                }, async result =>
                {
                    if (result)
                    {
                        await new DataExchangeServiceProxy().DeleteExchangeTaskAsync(
                            ((TaskItem) SelectedItem).Dto.Id);
                        Refresh();
                    }
                });
          /*      {
                    Header = ,
                    Content = new TextBlock
                    {
                        Text = "Внимание! Удаляем задание, со всеми вложенными объектами. Необходимо Ваше подтверждение.",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    OkButtonContent = "Удалить",
                    CancelButtonContent = "Отмена",
                    Closed = async (obj, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            
                        }
                    }

                });*/
            }


            // Удаление объекта 
            if (SelectedItem is EntityItem)
            {
                MessageBoxProvider.Confirm( new TextBlock
                    {
                        Text = "Внимание! Удаляем объект обмена. Необходимо Ваше подтверждение.",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                   
                   async ( args) =>
                    {
                        if (args)
                        {
                            await new DataExchangeServiceProxy().DeleteExchangeEntityAsync(
                                new AddEditExchangeEntityParameterSet
                                {
                                    EntityId = ((EntityItem) SelectedItem).Dto.EntityId,
                                    ExchangeTaskId = ((EntityItem) SelectedItem).Dto.ExchangeTaskId
                                });
                                    
                            Refresh();
                        }
                    },"Подтверждение", "Удалить","Отмена");
            }
        }

        public bool UpdateEntityBinding(ExchangeEntityDTO dto)
        {
            // Проверка - нельзя вводить идентификатор для объекта, если он уже введен для другого объекта этой же задачи.
            // Если этого не делать, то валиться ошибка от сервера по поводу целосности
            var src = Items.Single(i => i.Children.OfType<TaskItem>().Any(t => t.Dto.Id == dto.ExchangeTaskId));
            var isOdd =
                src.Children.OfType<TaskItem>()
                    .Single(t => t.Dto.Id == dto.ExchangeTaskId)
                    .Children.OfType<EntityItem>()
                    .Any(e => e.Dto.EntityId != dto.EntityId && e.ExtId == dto.ExtId);
            if (isOdd)
            {
                MessageBoxProvider.Alert(
                    "Невозможно присвоить идентификатор, т.к. он уже присвоен другому объекту этой задачи",
                    "Недопустимый идентификатор");
                return false;
            }

            new DataExchangeServiceProxy().EditExchangeEntityAsync(
                new AddEditExchangeEntityParameterSet
                {
                    EntityId = dto.EntityId,
                    ExchangeTaskId = dto.ExchangeTaskId,
                    ExtId = dto.ExtId,
                    IsActive = dto.IsActive
                });

            return true;

            //Refresh();
        }

        public void UpdatePropertyBinding(ExchangeEntityDTO dto, PropertyType propType, string extId)
        {
            new DataExchangeServiceProxy().SetExchangePropertyAsync(
                new SetExchangePropertyParameterSet
                {
                    EntityId = dto.EntityId,
                    ExchangeTaskId = dto.ExchangeTaskId,
                    PropertyTypeId = propType,
                    ExtId = extId
                });

            //Refresh();
        }



        public List<ItemBase> Items { get; set; }

        private ItemBase _selectedItem;
        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    RefreshCommands();
                    OnPropertyChanged(() => IsExportTaskSelected);
                    OnPropertyChanged(() => IsExchangeDatepickerVisible);
                    OnPropertyChanged(() => IsExchangeDatepickerTwoHourVisible);

                    RaiseCommands();
                    // Загрузка лога по выбранной задаче (если выбрана конечно)
                    GetExchangeLog(value);
                }
            }
        }

        public bool IsExchangeDatepickerVisible
        {
            get
            {
                return IsDayExportSelected ;
            }
        }

        public bool IsExchangeDatepickerTwoHourVisible
        {
            get
            {
                return IsHourExportSelected;
            }
        }

        public bool IsHourExportSelected
        {
            get
            {
                var item = _selectedItem as TaskItem;
                return item != null && item.IsExport && item.Dto.PeriodTypeId == PeriodType.Twohours;
            }
        }


        public bool IsDayExportSelected
        {
            get
            {
                var item = _selectedItem as TaskItem;
                return item != null && item.IsExport && item.Dto.PeriodTypeId == PeriodType.Day;
            }
        }

        public bool IsExportTaskSelected
        {
            get
            {
                var item = _selectedItem as TaskItem;
                return item != null && item.IsExport;
            }
        }

        private DateTime _timestamp;

        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { SetProperty(ref _timestamp, value); }
        }


        private void RaiseCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            RunCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            CheckXslCommand.RaiseCanExecuteChanged();
            //CloneCommand.RaiseCanExecuteChanged();
        }


        // Получение xml или текстового файла обмена
        private void SaveAs()
        {
            var task = ((TaskItem)SelectedItem).Dto;
            HtmlPage.Window.Navigate(UriBuilder.GetSpecificExchangeHandlerUri(task.Id,
                IsDayExportSelected ? Timestamp.Date : Timestamp.ToLocal(), periodType: SelectedPeriodType,  xmlOnly: !task.IsTransform));
        }

        public PeriodType SelectedPeriodType => IsDayExportSelected ? PeriodType.Day : IsHourExportSelected ? PeriodType.Twohours : PeriodType.None;

        private async void Run()
        {
            var dto = ((TaskItem) SelectedItem).Dto;
            await
                new DataExchangeServiceProxy().RunExchangeTaskAsync(new RunExchangeTaskParameterSet
                {
                    Id = dto.Id,
                    TimeStamp = IsDayExportSelected ? Timestamp.Date : Timestamp.ToLocal(),
                    PeriodTypeId = SelectedPeriodType
                });
        }


        public List<ExchangeLogDTO> Log { get; set; }
        public List<LogGroup> Log2 { get; set; }
        
        private async void GetExchangeLog(ItemBase selectedItem)
        {
            var item = selectedItem as TaskItem;
            if (item == null) return;
            try
            {
                Behavior.TryLock();

                switch (item.Dto.PeriodTypeId)
                {
                    case PeriodType.Twohours:
                    {
                        var currentSession = SeriesHelper.GetCurrentSession();
                        var seriesList = await new SeriesDataServiceProxy().GetSeriesListAsync(
                            new GetSeriesListParameterSet
                            {
                                PeriodStart =  _selectedPeriod.Begin,
                                PeriodEnd =  _selectedPeriod.End,
                                PeriodType = PeriodType.Twohours
                            });
                        
                        var log = await new DataExchangeServiceProxy().GetExchangeLogAsync(
                            new GetExchangeLogParameterSet
                            {
                                ExchangeTaskId = item.Dto.Id,
                                StartDate =  _selectedPeriod.Begin,
                                EndDate =  _selectedPeriod.End
                            });

                        Log2 = new List<LogGroup>();
                        foreach (var series in seriesList)
                        {
                            var groupItem = new LogGroup
                            {
                                Name = series.KeyDate.ToString("HH:mm"),
                                Name2 = series.KeyDate.ToString("dd.MM.yyyy"), 
                            };
                            groupItem.Children.AddRange(log.Where(l => l.SerieId == series.Id).Select(l => new LogItem(l)));
                            Log2.Add(groupItem);
                        }

                        break;
                    }

                    case PeriodType.Day:
                    {
                        var seriesList = await new SeriesDataServiceProxy().GetSeriesListAsync(
                            new GetSeriesListParameterSet
                            {
                                PeriodStart =  _selectedPeriod.Begin,
                                PeriodEnd =  _selectedPeriod.End,
                                PeriodType = PeriodType.Day
                            });

                        var log = await new DataExchangeServiceProxy().GetExchangeLogAsync(
                            new GetExchangeLogParameterSet
                            {
                                ExchangeTaskId = item.Dto.Id,
                                StartDate =  _selectedPeriod.Begin,
                                EndDate =_selectedPeriod.End
                            });

                        Log2 = new List<LogGroup>();
                        foreach (var series in seriesList)
                        {
                            var groupItem = new LogGroup
                            {
                                Name = series.KeyDate.ToString("dd.MM.yyyy"),
                            };
                            groupItem.Children.AddRange(log.Where(l => l.SerieId == series.Id).Select(l => new LogItem(l)));
                            Log2.Add(groupItem);
                        }

                        break;
                    }

                    //case PeriodType.Month:
                    //{
                    //    var seriesList = await new SeriesDataServiceProxy().GetSeriesListAsync(
                    //        new GetSeriesListParameterSet
                    //        {
                    //            PeriodStart = DateTime.Now.AddMonths(-3),
                    //            PeriodEnd = DateTime.Now,
                    //            PeriodType = PeriodType.Month,
                    //            TargetId = Target.Fact
                    //        });

                    //    var log = await new DataExchangeServiceProxy().GetExchangeLogAsync(
                    //        new GetExchangeLogParameterSet
                    //        {
                    //            ExchangeTaskId = item.Dto.Id,
                    //            StartDate = DateTime.Now.AddMonths(-3),
                    //            EndDate = DateTime.Now
                    //        });

                    //    Log2 = new List<LogGroup>();
                    //    foreach (var series in seriesList)
                    //    {
                    //        var groupItem = new LogGroup
                    //        {
                    //            Name = series.KeyDate.ToString("M")
                    //        };
                    //        groupItem.Children.AddRange(log.Where(l => l.SerieId == series.Id).Select(l => new LogItem(l)));
                    //        Log2.Add(groupItem);
                    //    }

                    //    break;
                    //}
                }

                OnPropertyChanged(() => Log2);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }
        
    }

    public class ItemBase : PropertyChangedBase
    {
        public ItemBase()
        {
            Children = new List<ItemBase>();
            IsExpanded = true;
        }

        public List<ItemBase> Children { get; set; }
        
        public virtual string Name { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsEnabled { get; set; }

        /// <summary>
        /// Это нужно для востановления позиции курсора,
        /// т.к. при обновлении создается новый перечень объектов,
        /// то эта функция помогает определить, что два объекта ссылаются на одну и ту же запись в БД
        /// Соотв. переопределяется в каждом наследнике по своему
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IsSame(ItemBase other)
        {
            return false;
        }
    }


    public class DataSourceItem : ItemBase
    {
        private DataSourceDTO _dto;

        public DataSourceItem(DataSourceDTO dto)
        {
            _dto = dto;
        }

        public DataSourceDTO Dto
        {
            get { return _dto; }
        }

        public override string Name
        {
            get { return _dto.Name; }
        }

        public override bool IsSame(ItemBase other)
        {
            var item = other as DataSourceItem;
            return item != null && item.Dto.Id == Dto.Id;
        }
    }

    public class TaskItem : ItemBase
    {
        private ExchangeTaskDTO _dto;

        public TaskItem(ExchangeTaskDTO dto)
        {
            _dto = dto;
        }

        public ExchangeTaskDTO Dto
        {
            get { return _dto; }
        }

        public override string Name
        {
            get { return _dto.Name; }
        }

        public override bool IsSame(ItemBase other)
        {
            var item = other as TaskItem;
            return item != null && item.Dto.Id == Dto.Id;
        }

        public bool IsImport
        {
            get { return _dto.ExchangeTypeId == ExchangeType.Import; }
        }

        public bool IsExport
        {
            get { return _dto.ExchangeTypeId == ExchangeType.Export; }
        }
    }

    public class EntityItem : ItemBase
    {
        private ExchangeEntityDTO _dto;
        private Func<ExchangeEntityDTO, bool> _saveAction;

        public EntityItem(ExchangeEntityDTO dto, Func<ExchangeEntityDTO, bool> saveAction)
        {
            _dto = dto;
            _saveAction = saveAction;

            IsExpanded = false;
        }

        public ExchangeEntityDTO Dto
        {
            get { return _dto; }
        }

        public override string Name
        {
            get { return _dto.EntityPath; }
        }

        public override bool IsSame(ItemBase other)
        {
            var item = other as EntityItem;
            return item != null && item.Dto.ExchangeTaskId == Dto.ExchangeTaskId && item.Dto.EntityId == Dto.EntityId;
        }

        public string ExtId
        {
            get { return _dto.ExtId; }
            set
            {
                if (_dto.ExtId != value)
                {
                    var oldValue = _dto.ExtId;
                    _dto.ExtId = value;
                    if (!_saveAction(_dto))
                        _dto.ExtId = oldValue;
                    
                    OnPropertyChanged(() => ExtId);
                }

            }
        }

        public bool IsActive
        {
            get { return _dto.IsActive; }
            set
            {
                if (_dto.IsActive != value)
                {
                    _dto.IsActive = value;
                    OnPropertyChanged(() => IsActive);
                    _saveAction(_dto);
                }
            }
        }
    }

    public class PropertyItem : ItemBase
    {
        private readonly ExchangeEntityDTO _entityDto;
        private readonly PropertyType _propType;
        private string _extId;
        private Action<ExchangeEntityDTO, PropertyType, string> _saveAction;

        public PropertyItem(ExchangeEntityDTO dto, PropertyType pt, string extId, Action<ExchangeEntityDTO, PropertyType, string> saveAction)
        {
            _entityDto = dto;
            _propType = pt;
            _extId = extId;
            _saveAction = saveAction;
        }

        public ExchangeEntityDTO EntityDto
        {
            get { return _entityDto; }
        }

        public override string Name
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == _entityDto.EntityTypeId)
                        .EntityProperties.Single(pt => pt.PropertyType == _propType).Name;
            }
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public override bool IsSame(ItemBase other)
        {
            var item = other as PropertyItem;
            return item != null && item.EntityDto.ExchangeTaskId == EntityDto.ExchangeTaskId &&
                   item.EntityDto.EntityId == EntityDto.EntityId && item._propType == _propType;
        }

        public string ExtId
        {
            get { return _extId; }
            set
            {
                if (_extId != value)
                {
                    _extId = value;
                    OnPropertyChanged(() => ExtId);
                    _saveAction(_entityDto, _propType, value);
                }

            }
        }
    }


    public class LogGroup
    {
        public LogGroup()
        {
            Children = new List<LogItem>();
        }

        public string Name { get; set; }
        public string Name2 { get; set; }

        public List<LogItem> Children { get; set; }

        public bool IsOk
        {
            get { return Children.Any(i => i.Dto.IsOk); }
        }
    }

    public class LogItem
    {
        private ExchangeLogDTO _dto;

        public LogItem(ExchangeLogDTO dto)
        {
            _dto = dto;
        }

        public ExchangeLogDTO Dto
        {
            get { return _dto; }
        }
    }
}