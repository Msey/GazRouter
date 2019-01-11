using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.Series;
using Utils.Extensions;
using System.Windows.Threading;

namespace GazRouter.Controls.InputStory
{
    public class InputStoryViewModel : LockableViewModel
    {



        public InputStoryViewModel()
        {

        }



        public List<ItemBase> Items { get; set; }
        private List<ItemBase> prevItems { get; set; }

        private ItemBase _selectedItem;
        public ItemBase SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    //UpdateCommands();
                }
            }
        }

        private DispatcherTimer _autorefresh;
        private bool isActiveTimer;
        private int timerInterval;
        public void AutorefreshEnable()
        {
            _autorefresh = new DispatcherTimer();
            _autorefresh.Tick += QuickRefreshStatuses;
            if (isActiveTimer) ActivateTimer(timerInterval);
        }

        public void AutorefreshDisable()
        {
            _autorefresh.Tick -= QuickRefreshStatuses;
        }

        public void ActivateTimer(int secondsToRefresh = 5)
        {
            timerInterval = secondsToRefresh;
            isActiveTimer = true;
            if (_autorefresh == null)         
                _autorefresh = new DispatcherTimer();               
            
            _autorefresh.Stop();
            _autorefresh.Interval = TimeSpan.FromSeconds(timerInterval);
            _autorefresh.Start();
        }

        public void DeactivateTimer()
        {
            if (_autorefresh != null)
            {
                isActiveTimer = false;
                _autorefresh.Stop();
            }
        }

        private SeriesDTO _serie;
        public SeriesDTO CurrentSerie => _serie;

        private DateTime lastDate;
        private PeriodType lastPeriod;
        private int? lastSystemId;
        public async void Load(DateTime date, PeriodType period, bool lockUI, int? systemId = null)
        {
            if (!UserProfile.Current.Site.IsEnterprise) return;

            if (lockUI) Lock();
            lastDate = date;
            lastPeriod = period;
            lastSystemId = systemId;
            // Так как в дальнейшем оперировать придется с SerieId, 
            // поэтому здесь ее каждый раз при задании сеанса получаем(создаем) и храним. 
            _serie = await new SeriesDataServiceProxy().AddSerieAsync(
                new AddSeriesParameterSet
                {
                    KeyDate = date.ToLocal(),
                    Day = date.Day,
                    Month = date.Month,
                    Year = date.Year,
                    PeriodTypeId = period
                });



            // Список ЛПУ
            var siteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.Id,
                    SystemId = systemId
                });

            // Статус ввода по ЛПУ
            var inputStates = await new ManualInputServiceProxy().GetInputStateListAsync(
                new GetManualInputStateListParameterSet { SerieId = _serie.Id });

            // Получить список ошибок по объектам
            var statusList = await new ManualInputServiceProxy().GetEntityValidationStatusListAsync(
                new GetEntityValidationStatusListParameterSet
                {
                    SerieId = _serie.Id,
                });


            // Получить список источников данных
            var srcList = await new DataExchangeServiceProxy().GetDataSourceListAsync(null);

            // Получить список задач импорта влияющих на формирование серии
            var taskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(
                new GetExchangeTaskListParameterSet
                {
                    ExchangeTypeId = ExchangeType.Import,
                    PeriodTypeId = period
                });

            // Получить лог за выбранный период
            var neighbourLog = await new DataExchangeServiceProxy().GetExchangeLogAsync(
                new GetExchangeLogParameterSet
                {
                    StartDate = date.ToLocal(),
                    EndDate = date.ToLocal()
                });


            if (prevItems != null) prevItems = Items;
            else prevItems = new List<ItemBase>();

            Items = new List<ItemBase>();

            // РУЧНОЙ ВВОД
            // Добавляем статусы по ЛПУ
            foreach (var site in siteList)
            {
                var errorCount =
                    statusList.Count(s => s.SiteId == site.Id && s.Status == EntityValidationStatus.Error);

                var inputState = inputStates.SingleOrDefault(s => s.SiteId == site.Id);
                var status = inputState?.State == ManualInputState.Approved ? DataStatus.Ok : DataStatus.Waiting;
                if (status != DataStatus.Ok && errorCount > 0) status = DataStatus.Error;

                var siteItem = new SiteItem(site)
                {
                    ErrorCount = status != DataStatus.Ok ? errorCount : 0,
                    Status = status,
                    ChangeDate = inputState?.ChangeDate,
                    ChangeUser = inputState?.UserName,
                    NewIncomingErrors = new Tuple<int?, bool>(status != DataStatus.Ok ? errorCount : 0,false)
                };
                Items.Add(siteItem);
            }

            if (prevItems.Count>0)
            {
                for (int i = 0; i <= Items.Count - 1; i++)
                {
                    if (prevItems[i].ErrorCount != Items[i].ErrorCount || prevItems[i].Status != Items[i].Status)
                        Items[i].NewIncomingErrors = new Tuple<int?, bool>(Items[i].NewIncomingErrors.Item1, true);
                }
            }

            // СМЕЖНЫЕ ПРЕДПРИЯТИЯ
            foreach (var task in taskList.Where(t => t.IsCritical))
            {
                var src = srcList.SingleOrDefault(s => s.Id == task.DataSourceId);
                if (src == null) continue;

                var isOk = neighbourLog.Where(l => l.ExchangeTaskId == task.Id).Any(s => s.IsOk);

                var taskItem = new NeighborItem
                {
                    SourceName = $"{srcList.Single(s => s.Id == task.DataSourceId).Name},{Environment.NewLine}{task.Name}",
                    Status = isOk ? DataStatus.Ok : DataStatus.Waiting
                };
                Items.Add(taskItem);
            }

            //// ПВК АСТРА-ГАЗ
            //var astraItem = new GroupItem
            //{
            //    Name = "ПВК АСТРА-ГАЗ"
            //};
            //Items.Add(astraItem);

            //var exportItem = new StateItem
            //{
            //    Name = "Выгрузка исходных данных"
            //};
            //astraItem.Children.Add(exportItem);

            //var importItem = new StateItem
            //{
            //    Name = "Загрузка результатов расчета"
            //};
            //astraItem.Children.Add(importItem);
            if (lockUI) Unlock();
            OnPropertyChanged(() => Items);
        }


        private void QuickRefreshStatuses(object sender, EventArgs e)
        {
            Load(lastDate, lastPeriod, false, lastSystemId);
        }

    }
}
        