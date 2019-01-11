using System;
using System.Windows.Controls;
using DataExchange.ASDU;
using DataExchange.RestServices;
using DataExchange.Typical;
using GazRouter.ActionsRolesUsers.Views;
using GazRouter.Balances.BalanceGroups;
using GazRouter.Balances.DayBalance;
using GazRouter.Balances.GasOwners;
using GazRouter.Balances.Routes;
using GazRouter.Client.Menu.EventsDrop;
using GazRouter.Client.Menu.LinkListDrop;
using GazRouter.Client.Menu.UserDrop;
using GazRouter.Common;
using GazRouter.Common.Events;
using GazRouter.Common.ViewModel;
using GazRouter.DataExchange.ASTRA;
using GazRouter.DataExchange.ASUTP;
using GazRouter.DataExchange.CustomSource;
using GazRouter.DataExchange.ExchangeLog;
using GazRouter.DTO.EventLog.EventRecipient;
using GazRouter.ManualInput.ChemicalTests;
using GazRouter.ManualInput.CompUnits;
using GazRouter.ManualInput.CompUnitTests;
using GazRouter.ManualInput.ContractPressures;
using GazRouter.ManualInput.Daily;
using GazRouter.ManualInput.Dashboard;
using GazRouter.ManualInput.Hourly;
using GazRouter.ManualInput.Settings;
using GazRouter.ManualInput.Valves;
using GazRouter.Modes.Calculations;
using GazRouter.Modes.EventLog;
using GazRouter.Modes.GasCosts;
using GazRouter.Modes.ProcessMonitoring.Dashboards;
using GazRouter.Modes.ProcessMonitoring.Reports;
using GazRouter.Modes.ProcessMonitoring.Reports.Forms.CompShops;
using GazRouter.Modes.ProcessMonitoring.Reports.Forms.CompUnitFailures;
using GazRouter.Modes.ProcessMonitoring.Reports.Forms.CompUnits;
using GazRouter.Modes.ProcessMonitoring.Reports.Forms.GasInPipes;
using GazRouter.Modes.ProcessMonitoring.Reports.Forms.Valves;
using GazRouter.ObjectModel.Views;
using GazRouter.Repair;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using FloViewControl = GazRouter.Modes.ProcessMonitoring.FloViewControl;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
using GazRouter.ObjectModel.DeviceConfig;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using System.Collections.Generic;
using GazRouter.Balances.Commercial.SiteInput;
using MainView = GazRouter.Balances.Commercial.Fact.MainView;
using PlanView = GazRouter.Balances.Commercial.Plan.PlanView;
using DataExchange.Integro.Summary;
using GazRouter.Balances.DistrNetworks;
using GazRouter.Client.Menu.TasksDrop;
using GazRouter.Modes.DispatcherTasks;
using GazRouter.Modes.GasCosts2;
using GazRouter.ManualInput.PipelineLimits;
using GazRouter.Modes.Infopanels;

namespace GazRouter.Client.Menu
{   
    public class LinkRegister
    {
        static LinkRegister()
        {
            Links = new Dictionary<LinkType, PermissionDTO2>
            {
#region МОНИТОРИНГ
                {LinkType.Monitoring, new PermissionDTO2{
                        Name  = "Мониторинг",
                        Image = "/Common;component/Images/48x48/monitoring.png"}},
                {LinkType.Schema, new PermissionDTO2{
                        Name = "Схема",
                        Uri = $"{typeof (FloViewControl).FullName}?reportId={typeof (FloViewControl).FullName}"}},
                {LinkType.Kc, new PermissionDTO2{
                        Name = "КЦ",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (CompShopsViewModel).FullName}"}},
                {LinkType.Gpa, new PermissionDTO2{
                        Name = "ГПА",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (CompUnitsViewModel).FullName}"}},
                {LinkType.Grs, new PermissionDTO2{
                        Name = "ГРС",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (Modes.ProcessMonitoring.Reports.Forms.DistrStations.DistrStationsViewModel).FullName}",
                        Image = "/Common;component/Images/16x16/EntityTypes/distr_station.png"}},
                {LinkType.Gis, new PermissionDTO2{
                        Name = "ГИС",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (Modes.ProcessMonitoring.Reports.Forms.MeasStations.MeasStationsViewModel).FullName}",
                        Image = "/Common;component/Images/16x16/EntityTypes/meas_line.png"}},
                {LinkType.Urg, new PermissionDTO2{
                        Name = "УРГ",
                        Uri =  $"{typeof (ReportsView).FullName}?reportId={typeof (Modes.ProcessMonitoring.Reports.Forms.ReducingStations.ReducingStationsViewModel).FullName}",
                        Image = "/Common;component/Images/16x16/EntityTypes/reducing_station.png"}},
                {LinkType.PhysChymParams, new PermissionDTO2{
                        Name = "Физ-хим. показатели",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (Modes.ProcessMonitoring.Reports.Forms.ChemicalTests.ChemicalTestsViewModel).FullName}",
                        Image = "/Common;component/Images/16x16/EntityTypes/meas_point.png"}},
                {LinkType.ZraLog, new PermissionDTO2{
                        Name = "Журнал переключений ЗРА",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (ValvesViewModel).FullName}",
                        Image = "/Common;component/Images/16x16/EntityTypes/valve.png"}},
                {LinkType.GpaStop, new PermissionDTO2{
                        Name = "Аварийные и вынужденные остановы ГПА",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (CompUnitFailuresViewModel).FullName}"}},
                {LinkType.GasReserve, new PermissionDTO2{
                        Name = "Запас газа",
                        Uri = $"{typeof (ReportsView).FullName}?reportId={typeof (GasInPipesViewModel).FullName}",
                        Image = "/Common;component/Images/16x16/EntityTypes/pipeline.png"}},
                {LinkType.Infopanel, new PermissionDTO2{
                        Name = "Инфопанели",
                        Uri = $"{typeof (InfopanelsView).FullName}?reportId={typeof (InfopanelsViewModel).FullName}",
                        Image = "/Common;component/Images/16x16/dashboard.png"}},
#endregion
#region ЖУРНАЛ
                {LinkType.EventLog, new PermissionDTO2{
                        Name  = "Журнал событий",
                        Image = "/Common;component/Images/32x32/event_log.png"}},
                {LinkType.Log, new PermissionDTO2{
                        Name = "Открыть журнал",
                        Uri = typeof (EventLogMainView).FullName,
                        Image = "/Common;component/Images/32x32/event_log.png"}},
#endregion
#region ДИСПЕТЧЕРСКИЕ_ЗАДАНИЯ
                {LinkType.DispTasks, new PermissionDTO2{
                        Name  = "Диспетчерские задания",
                        Image = "/Common;component/Images/32x32/tasks2.png"}},
                {LinkType.DispTasksAll, new PermissionDTO2{
                        Name = "Диспетчерские задания",
                        Uri = typeof (TasksMainView).FullName}},
                
#endregion
#region УЧЕТ_ГАЗА_(БАЛАНСЫ)
                {LinkType.Gasaux, new PermissionDTO2{
                        Name  = "Учет газа (балансы)",
                        Image = "/Common;component/Images/32x32/gasaux.png"}},
                {LinkType.Plan, new PermissionDTO2{
                        Name = "План транспорта газа",
                        Uri = typeof (PlanView).FullName}},
                {LinkType.DayBalance, new PermissionDTO2{
                        Name = "Суточный баланс",
                        Uri = typeof (DayBalanceView).FullName}},
                {LinkType.MonthBalance, new PermissionDTO2{
                        Name = "Месячный баланс",
                        Uri = typeof (MainView).FullName,
                        Image = "/Common;component/Images/16x16/circle2.png"}},

                {LinkType.GasCosts, new PermissionDTO2{
                        Name = "СТН",
                        Uri = typeof (GasCostsMainView).FullName}},

                {LinkType.GasCosts2, new PermissionDTO2{
                        Name = "СТН2",
                        Uri = typeof (GasCostsMainView2).FullName}},

                {LinkType.GasOwners, new PermissionDTO2{
                        Name = "Поставщики",
                        Uri = typeof (GasOwnersView).FullName}},
                {LinkType.Routes, new PermissionDTO2{
                        Name = "Маршруты",
                        Uri = typeof (RoutesView).FullName,
                        Image = "/Common;component/Images/16x16/route.png"}},
                {LinkType.BalanceGroups, new PermissionDTO2{
                        Name = "Группы",
                        Uri = typeof (BalanceGroupsView).FullName}},
                {LinkType.DistrNetworks, new PermissionDTO2{
                        Name = "ГРО",
                        Uri = typeof (DistrNetworksView).FullName}},
#endregion
#region РЕМОНТЫ
                {LinkType.Repairs, new PermissionDTO2{
                        Name  = "Ремонты",
                        Image = "/Common;component/Images/32x32/maintenance.png"}},
                //{LinkType.RepairPlan, new PermissionDTO2{
                //        Name = "(старое!) План ремонтных работ",
                //        Uri = typeof (RepairMainView).FullName}},
                {LinkType.RepairDva, new PermissionDTO2{
                        Name = "Планирование ремонтных работ",
                        Uri = typeof (Repair.Plan.PlanView).FullName}},
                {LinkType.RepairRequest, new PermissionDTO2{
                        Name = "Заявки на проведение ремонтных работ",
                        Uri = typeof (Repair.RepWorks.
                        RequestView).FullName}},
                {LinkType.RepairInProgress, new PermissionDTO2{
                        Name = "Текущие ремонтные работы",
                        Uri = typeof (Repair.RepWorks.CurrentWorksView).FullName}},
                {LinkType.RepairComplited, new PermissionDTO2{
                        Name = "Завершенные ремонтные работы",
                        Uri = typeof (Repair.RepWorks.ComplitedWorksView).FullName}},
                 {LinkType.RepairAgreements, new PermissionDTO2{
                        Name = "Работы на согласовании",
                        Uri = typeof (Repair.Agreement.UserAgreementsListView).FullName}},
#endregion
#region ВВОД_ДАННЫХ  
                {LinkType.Input, new PermissionDTO2{
                        Name  = "Ввод данных",
                        Image = "/Common;component/Images/48x48/bindings_2.png"}},
                {LinkType.Dashboard, new PermissionDTO2{
                        Name = "Контроль сеанса",
                        Uri = typeof (DashboardView).FullName,
                        Image = "/Common;component/Images/16x16/trace.png"}},
                {LinkType.Hourly, new PermissionDTO2{
                        Name = "Сеансовые данные",
                        Uri = typeof (HourlyView).FullName,
                        Image = "/Common;component/Images/16x16/ptq.png"}},
                {LinkType.Daily, new PermissionDTO2{
                        Name = "Данные за сутки",
                        Uri = typeof (DailyView).FullName}},
                {LinkType.SiteInput, new PermissionDTO2{
                        Name = "По поставщикам",
                        Uri = typeof (SiteInputView).FullName,
                        Image = "/Common;component/Images/16x16/circle2.png"}},
                {LinkType.CompUnit, new PermissionDTO2{
                        Name = "Состояния ГПА",
                        Uri = typeof (CompUnitView).FullName}},
                {LinkType.Valve, new PermissionDTO2{
                        Name = "Переключения ЗРА",
                        Uri = typeof (ValveView).FullName,
                        Image = "/Common;component/Images/16x16/EntityTypes/valve.png"} },
                {LinkType.ChemicalTests, new PermissionDTO2{
                        Name = "Результаты хим. анализа",
                        Uri = typeof (ChemicalTestsView).FullName,
                        Image ="/Common;component/Images/16x16/EntityTypes/meas_point.png" } },
                {LinkType.CompUnitTests, new PermissionDTO2{
                        Name = "Результаты испытаний ГПА",
                        Uri = typeof (CompUnitTestsView).FullName}},
                {LinkType.PipelineLimits, new PermissionDTO2{
                        Name = "Ограничения по давлению",
                        Uri = typeof (PipelineLimitsView).FullName,
                        Image ="/Common;component/Images/16x16/EntityTypes/pressure_limits.png" } },
                {LinkType.ContractPressures, new PermissionDTO2{
                        Name = "Договорные давления на выходах ГРС",
                        Uri = typeof (ContractPressuresView).FullName} },
#endregion
#region НАСТРОЙКИ
                {LinkType.Settings, new PermissionDTO2{
                        Name = "Настройки",
                        Image = "/Common;component/Images/32x32/settings.png"}},
                {LinkType.Permissions, new PermissionDTO2{
                        Name = "Управление доступом",
                        Uri = typeof (ActionsRolesUsersView).FullName,
                        Image = "/Common;component/Images/16x16/access.png"}},
                {LinkType.ObjectModel, new PermissionDTO2{
                        Name = "Ведение объектной модели",
                        Uri = typeof (ObjectModelEditorMainView).FullName,
                        Image = "/Common;component/Images/16x16/object2.png"}},
                {LinkType.DeviceConfig, new PermissionDTO2{
                        Name  = "Локальные справочники",
                        Uri = typeof (DeviceConfigView).FullName}},
                {LinkType.DataCoollect, new PermissionDTO2{
                        Name = "Параметры сбора данных",
                        Uri = $"{typeof (SettingsView).FullName}",
                        Image = "/Common;component/Images/16x16/ptq.png"}},
                {LinkType.ConstructCalc, new PermissionDTO2{
                        Name = "Конструктор расчетов",
                        Uri = typeof (MainCalcView).FullName,
                        Image = "/Common;component/Images/16x16/calculations.png"}},
                {LinkType.Asutp, new PermissionDTO2{
                        Name = "АСУ ТП",
                        Uri = typeof (AsutpView).FullName}},
                {LinkType.Astra, new PermissionDTO2{
                        Name = "ПВК АСТРА-ГАЗ",
                        Uri = typeof (AstraView).FullName}},
                {LinkType.CustomSource, new PermissionDTO2{
                        Name = "Задания обмена",
                        Uri = typeof (CustomSourceView).FullName}},
                {LinkType.TypicalExch, new PermissionDTO2{
                        Name = "GR Xchange",
                        Uri = typeof (TypicalExchangeView).FullName}},
                {LinkType.RestServices, new PermissionDTO2{
                        Name = "ЕИТП",
                        Uri = typeof (RestServicesView).FullName}},
                {LinkType.ExchangeLog, new PermissionDTO2{
                        Name = "Журнал",
                        Uri = typeof (ExchangeLogView).FullName,
                        Image = "/Common;component/Images/16x16/log.png"}},
                {LinkType.AsduMapping, new PermissionDTO2{
                        Name = "АСДУ ЕСГ",
                        Uri = typeof (SummaryCatalog).FullName+LinkType.AsduMapping.ToString()}},
                {LinkType.AsspootiMapping, new PermissionDTO2{
                        Name = "АССПООТИ",
                        Uri = typeof (SummaryCatalog).FullName+LinkType.AsspootiMapping.ToString()}},
                {LinkType.AsduNsiDataImport, new PermissionDTO2{
                        Name = "Загрузка НСИ АСДУ ЕСГ",
                        Uri = typeof (AsduImportData).FullName}},
                /*{LinkType.AsduMatching, new PermissionDTO2{
                        Name = "Связывание НСИ АСДУ ЕСГ",
                        Uri = typeof (AsduMatchingView).FullName}},*/
                 {LinkType.AsduNsiMetadata, new PermissionDTO2{
                        Name = "Метаданные НСИ АСДУ ЕСГ",
                        Uri = typeof (AsduMetadataView).FullName}},
                {LinkType.AsduNsiData, new PermissionDTO2{
                        Name = "Данные НСИ АСДУ ЕСГ",
                        Uri = typeof (AsduDataView).FullName}},
#endregion
#region ОТЧЕТЫ
                {LinkType.Report, new PermissionDTO2{
                        Name  = "Отчеты",
                        Image = "/Common;component/Images/32x32/reports.png"}},
                {LinkType.SapBo, new PermissionDTO2{
                        Name = "SAP BO",
                        Uri = UriBuilder.GetSapBoUri2}},
#endregion
#region ПОЛЬЗОВАТЕЛЬ
                {LinkType.User, new PermissionDTO2{
                        Name = "Пользователь",
                        Image = "/Common;component/Images/32x32/user4.png"}},
#endregion
            };
            GuidValidation(Links);
        }

        private static readonly Dictionary<LinkType, PermissionDTO2> Links;
        public static PermissionDTO2 GetLinkInfo(LinkType linkType)
        {
            return Links.ContainsKey(linkType) ? Links[linkType] : null;
        }
        private static void GuidValidation(Dictionary<LinkType, PermissionDTO2> dic)
        {
//            todo:    
// var t1 = ;
//            if (1 > 1)
//            {
//                throw new Exception("Идентификаторы линков совпадают!");
//            }
        }
    }
    public class MainMenuViewModel : AsyncViewModelBase
    {
#region constructor
        public MainMenuViewModel()
        {
            Media = new MediaElement
            {
                AutoPlay = false,
                Source = new Uri("/Client;component/Resources/notification.wav", UriKind.Relative)
            };
            CreateMenu();
        }
#endregion
#region variables
        private NonAckEventCountDTO _eventCount = new NonAckEventCountDTO();
        public MediaElement Media { get; }
        public DropMenuViewModel MonitoringDropViewModel { get; set; }
        public DropMenuViewModel EventLogDropViewModel { get; set; }
        public DropMenuViewModel TasksListDropViewModel { get; set; }
        public DropMenuViewModel BalanceDropViewModel { get; set; }
        public DropMenuViewModel RepairsDropViewModel { get; set; }
        public DropMenuViewModel InputDropViewModel { get; set; }
        public DropMenuViewModel SetupDropViewModel { get; set; }
        public DropMenuViewModel UserDropViewModel { get; set; }
        public DropMenuViewModel ReportDropViewModel { get; set; }
#endregion
        private void CreateMenu()
        {
#region МОНИТОРИНГ
            MonitoringDropViewModel = new DropMenuViewModel(LinkType.Monitoring);
            var monDrop = new LinkListDropViewModel();
            MonitoringDropViewModel.AddDropContent(monDrop);
            monDrop.AddLink(LinkType.Schema);
            monDrop.AddSeporator();
            monDrop.AddSection("СВОДКИ");
            monDrop.AddLink(LinkType.Kc);
            monDrop.AddLink(LinkType.Gpa);
            monDrop.AddLink(LinkType.Grs);
            monDrop.AddLink(LinkType.Gis);
            monDrop.AddLink(LinkType.Urg);
            monDrop.AddLink(LinkType.PhysChymParams);            
            monDrop.AddLink(LinkType.ZraLog);
            monDrop.AddLink(LinkType.GpaStop);                                                       
            monDrop.AddLink(LinkType.GasReserve);
            monDrop.AddSeporator();
            monDrop.AddLink(LinkType.Infopanel);
#endregion
#region ЖУРНАЛ_СОБЫТИЙ          
            EventLogDropViewModel = new DropMenuViewModel(LinkType.EventLog);
            var evntDrop = new EventsDropViewModel();
            EventLogDropViewModel.AddDropContent(evntDrop);
            evntDrop.AddLink(LinkType.Log);
            var evntAggr = ServiceLocator.Current.GetInstance<IEventAggregator>();
            evntAggr.GetEvent<NotAckEventCountChangedEvent>().Subscribe(UpdateEventCount, ThreadOption.UIThread, true);
            evntAggr.GetEvent<EventListUpdatedEvent>().Subscribe(UpdateEventList, ThreadOption.UIThread, true);
#endregion 
#region ДИСПЕТЧЕРСКИЕ_ЗАДАНИЯ
            TasksListDropViewModel = new DropMenuViewModel(LinkType.DispTasks);
            var taskDrop = new TasksDropViewModel();
            TasksListDropViewModel.AddDropContent(taskDrop);
            taskDrop.AddLink(LinkType.DispTasksAll);
            evntAggr.GetEvent<NotAckDispatherTaskListUpdatedEvent>()
                    .Subscribe(UpdateDispatherTaskList, ThreadOption.UIThread, true);
#endregion
#region УЧЕТ_ГАЗА_(БАЛАНСЫ)
            BalanceDropViewModel = new DropMenuViewModel(LinkType.Gasaux);
            var balDrop = new LinkListDropViewModel();
            BalanceDropViewModel.AddDropContent(balDrop);
            balDrop.AddLink(LinkType.Plan);
            balDrop.AddLink(LinkType.DayBalance);
            balDrop.AddLink(LinkType.MonthBalance);
            balDrop.AddLink(LinkType.GasCosts);
            balDrop.AddLink(LinkType.GasCosts2);
            balDrop.AddSeporator();
            balDrop.AddSection("БАЛАНСОВАЯ МОДЕЛЬ");
            balDrop.AddLink(LinkType.GasOwners);
            balDrop.AddLink(LinkType.Routes);
            balDrop.AddLink(LinkType.BalanceGroups);
            balDrop.AddLink(LinkType.DistrNetworks);
            #endregion
#region РЕМОНТЫ
                RepairsDropViewModel = new DropMenuViewModel(LinkType.Repairs);
                var repDrop = new LinkListDropViewModel();
                RepairsDropViewModel.AddDropContent(repDrop);
                //repDrop.AddLink(LinkType.RepairPlan);
                repDrop.AddLink(LinkType.RepairDva);
            repDrop.AddLink(LinkType.RepairRequest);
            repDrop.AddLink(LinkType.RepairInProgress);
            repDrop.AddLink(LinkType.RepairComplited);
            repDrop.AddLink(LinkType.RepairAgreements);
            #endregion
            #region ВВОД_ДАННЫХ                        
            InputDropViewModel = new DropMenuViewModel(LinkType.Input);
                var inpDrop = new LinkListDropViewModel();
                InputDropViewModel.AddDropContent(inpDrop);
                inpDrop.AddLink(LinkType.Dashboard);
                inpDrop.AddLink(LinkType.Hourly);
                inpDrop.AddLink(LinkType.Daily);
                inpDrop.AddLink(LinkType.SiteInput);
                inpDrop.AddSeporator();
                inpDrop.AddSection("ПО СОБЫТИЮ");
                inpDrop.AddLink(LinkType.CompUnit);
                inpDrop.AddLink(LinkType.Valve);
                inpDrop.AddLink(LinkType.ChemicalTests);
                inpDrop.AddLink(LinkType.CompUnitTests);
                inpDrop.AddLink(LinkType.PipelineLimits);
                inpDrop.AddLink(LinkType.ContractPressures);
            #endregion
            #region НАСТРОЙКИ
            SetupDropViewModel = new DropMenuViewModel(LinkType.Settings);
            var setupDrop = new LinkListDropViewModel();
            SetupDropViewModel.AddDropContent(setupDrop);
            setupDrop.AddLink(LinkType.Permissions);
            setupDrop.AddLink(LinkType.ObjectModel);
            setupDrop.AddLink(LinkType.DeviceConfig);
            setupDrop.AddLink(LinkType.DataCoollect);
            setupDrop.AddLink(LinkType.ConstructCalc);
            setupDrop.AddSeporator();
            setupDrop.AddSection("ИНФОРМАЦИОННОЕ ВЗАИМОДЕЙСТВИЕ");
            setupDrop.AddLink(LinkType.Asutp);
            setupDrop.AddLink(LinkType.Astra);
            setupDrop.AddLink(LinkType.CustomSource);
            setupDrop.AddLink(LinkType.TypicalExch);
            setupDrop.AddLink(LinkType.RestServices);
            setupDrop.AddLink(LinkType.ExchangeLog);
            setupDrop.AddLink(LinkType.AsduMapping);
            setupDrop.AddLink(LinkType.AsspootiMapping);
            setupDrop.AddLink(LinkType.AsduNsiDataImport);
            //setupDrop.AddLink(LinkType.AsduMatching);
            setupDrop.AddLink(LinkType.AsduNsiMetadata);
            setupDrop.AddLink(LinkType.AsduNsiData);
            #endregion
            #region ОТЧЕТЫ
            ReportDropViewModel = new DropMenuViewModel(LinkType.Report);
                        var reportDrop = new LinkListDropViewModel();
                        ReportDropViewModel.AddDropContent(reportDrop);
                        reportDrop.AddSapLink(LinkType.SapBo, true);
#endregion
#region ПОЛЬЗОВАТЕЛЬ
                        UserDropViewModel = new DropMenuViewModel(LinkType.User, false);
                        var usrDrop = new UserDropViewModel();
                        UserDropViewModel.AddDropContent(usrDrop);
#endregion
        }
        
        private void UpdateEventCount(NonAckEventCountDTO count)
        {
            if (_eventCount.LastEventDate < count.LastEventDate)
            {
                PlaySound();
            }
            EventLogDropViewModel.NotificationCount = count.Count;
            _eventCount = count;

            UpdateEventList(null);
        }
        private void UpdateEventList(object param)
        {
            var drop = EventLogDropViewModel.DropContent as EventsDropViewModel;
            drop?.RefreshEventList();
        }
        private void UpdateDispatherTaskList(object param)
        {
            PlaySound();

            var drop = TasksListDropViewModel.DropContent as TasksDrop.TasksDropViewModel;
            TasksListDropViewModel.NotificationCount = ((List<TaskRecordPdsDTO>)param).Count;
            drop?.RefreshEventList((List<TaskRecordPdsDTO>)param);
        }
        private void PlaySound()
        {
            try
            {
                Media.Stop();
                Media.Position = TimeSpan.FromSeconds(0);
                Media.Play();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}