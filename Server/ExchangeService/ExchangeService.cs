using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.ServiceProcess;
using System.Threading;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DTO.Exchange.ExchangeSettings;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Run;
using GazRouter.Service.Exchange.Lib.Transport;

namespace ExchangeService
{
    public class TimerContainer
    {
        public Timer Timer { get; set; }
        public TimerCallback TimerCallback { get; set; }
        public TimerSettingId SettingId { get; set; }
        public Action Action { get; set; }
        public TimeSpan Interval { get; set; }
    }

    internal partial class ExchangeService : ServiceBase
    {
        private const int default2HOffset = 10;
        private readonly JobHost _jobHost;
        private ExchangeImportAgent _importAgent;
        private readonly object _lock = new object();
        private Dictionary<int, Timer> _timers;

        public ExchangeService()
        {
            InitializeComponent();
            
            _jobHost = new JobHost();

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
        }

        private void Init()
        {
            var timerSettings = new TimerSettings();
            InitTimer(TimerSettingId.Export, ExchangeExportAgent.Run, timerSettings.Interval(TimerSettingId.Export));
            InitTimer(TimerSettingId.Transport, TransportAgent.Run, timerSettings.Interval(TimerSettingId.Transport));
            InitTimer(TimerSettingId.ExcelGenerator, ExcelGeneratorAgent.Run, timerSettings.Interval(TimerSettingId.ExcelGenerator));
            InitTimer(TimerSettingId.EventExchange, IncidentSituationsDispatcherTasksAgent.Run, timerSettings.Interval(TimerSettingId.EventExchange));

            if (AppSettingsManager.AsduEsgExportRun)
                InitAsduTimer();
            if (AppSettingsManager.AsspootiExportRun)
                InitAsduTimer();

            _importAgent = ExchangeImportAgent.Instance;
        }

        /// <summary>
        /// Таймеры создаются таким образом, чтобы делался dispose после каждого срабатывания. Если так не делать, 
        /// то сборщик мусоры не может убрать объекты, на которые подписан таймер.  
        /// </summary>
        /// <param name="settingId"></param>
        /// <param name="action"></param>
        /// <param name="interval"></param>
        private void InitTimer(TimerSettingId settingId, Action action, TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero) return ;

            lock (_lock)
            {
                var container = new TimerContainer();
                TimerCallback onTimerElapsed = state =>
                {
                    var cont = (TimerContainer) state;
                    var myLogger = new MyLogger("exchangeLogger");
                    try
                    {
                        myLogger.Info($@"Таймер {cont.SettingId} сработал");
                        _jobHost.DoWork(action);
                    }
                    catch (Exception ex)
                    {
                        myLogger.WriteFullException(ex, ex.Message);
                    }
                    finally
                    {
                        try
                        {
                            cont.Timer.Dispose();
                            cont.Timer = null;
                            InitTimer(cont.SettingId, cont.Action, cont.Interval);
                        }
                        catch (Exception e)
                        {
                            myLogger.WriteFullException(e, e.Message);
                        }
                    }
                };
                container.SettingId = settingId;
                container.Action = action;
                container.Interval = interval;
                container.Timer = new Timer(onTimerElapsed, container, interval, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// Таймеры создаются таким образом, чтобы делался dispose после каждого срабатывания. Если так не делать, 
        /// то сборщик мусоры не может убрать объекты, на которые подписан таймер.  
        /// </summary>
        /// <param name="settingId"></param>
        /// <param name="action"></param>
        /// <param name="interval"></param>
        /// <param name="startInterval"> интервал первого запуска</param>
        private void InitTimerIntegro(TimerSettingId settingId, Action action, TimeSpan interval, TimeSpan? startInterval = null)
        {
            if (interval <= TimeSpan.Zero) return;

            lock (_lock)
            {
                var container = new TimerContainer();
                TimerCallback onTimerElapsed = state =>
                {
                    var cont = (TimerContainer)state;
                    var myLogger = new MyLogger("exchangeLogger");
                    try
                    {
                        InitTimerIntegro(cont.SettingId, cont.Action, cont.Interval);
                        myLogger.Info($@"Таймер {cont.SettingId} сработал");
                        _jobHost.DoWork(action);
                    }
                    catch (Exception ex)
                    {
                        myLogger.WriteFullException(ex, ex.Message);
                    }
                    finally
                    {
                        try
                        {
                            cont.Timer.Dispose();
                            cont.Timer = null;                            
                        }
                        catch (Exception e)
                        {
                            myLogger.WriteFullException(e, e.Message);
                        }
                    }
                };
                container.SettingId = settingId;
                container.Action = action;
                container.Interval = interval;
                container.Timer = new Timer(onTimerElapsed, container, startInterval ?? interval, Timeout.InfiniteTimeSpan);
            }
        }

        private void InitAsduTimer()
        {
            MyLogger logger = new MyLogger("exchangeLogger");
            DateTime dateNow = DateTime.Now;
            var dateStart = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, 0, 0);
            // регламент 10 мин. (на отправку файла)
            // TODO:вынести минуты регламента в конфиг
            var offset = AppSettingsManager.AsduEsg2HOffset > 120 ? default2HOffset : AppSettingsManager.AsduEsg2HOffset;
            if ((dateNow.Hour % 2) != 0 || (dateNow.Minute > offset)) // если время старта сервиса в нечетный час или минуты вышли из регламента
            { 
                dateStart = dateStart.AddHours((dateNow.Hour % 2) == 0 ? 2 : 1);
            }
            dateStart = dateStart.AddMinutes(offset);
            var startTick = dateStart.Ticks - DateTime.Now.Ticks;
            var interval = new TimeSpan(2, 0, 0);
            logger.Info($@"Экспорт АСДУ: время старта {dateStart} с интервалом в {interval}");
            InitTimerIntegro(TimerSettingId.Asdu, AsduEsgExchangeAgent.Run, interval, new TimeSpan(startTick < 1 ? 1 : startTick));
        }

        private void InitAsspootiTimer()
        {
            MyLogger logger = new MyLogger("exchangeLogger");
            DateTime dateNow = DateTime.Now;
            var dateStart = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, 0, 0);
            // регламент 10 мин. (на отправку файла)
            // TODO:вынести минуты регламента в конфиг
            var offset = AppSettingsManager.Asspooti2HOffset > 120 ? default2HOffset : AppSettingsManager.Asspooti2HOffset;
            if ((dateNow.Hour % 2) != 0 || (dateNow.Minute > offset)) // если время старта сервиса в нечетный час или минуты вышли из регламента
            {
                dateStart = dateStart.AddHours((dateNow.Hour % 2) == 0 ? 2 : 1);
            }
            dateStart = dateStart.AddMinutes(offset);
            var startTick = dateStart.Ticks - DateTime.Now.Ticks;
            var interval = new TimeSpan(2, 0, 0);
            logger.Info($@"Экспорт АСДУ: время старта {dateStart} с интервалом в {interval}");
            InitTimerIntegro(TimerSettingId.Asspooti, AsspootiExchangeAgent.Run, interval, new TimeSpan(startTick < 1 ? 1 : startTick));
        }

        public static void RestartService(string strServiceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(strServiceName);
            var myLogger = new MyLogger("exchangeLogger");

            try
            {
                var timeoutMillisec1 = Environment.TickCount;
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                if (service.CanStop)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                var timeoutMillisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (timeoutMillisec2 - timeoutMillisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch(Exception e)
            {
                myLogger.WriteFullException(e, e.Message);
            }
        }


        protected override void OnStart(string[] args)
        {

            // TODO: Add code here to start your service.
            Init();
            _jobHost.Start();
            new MyLogger("exchangeLogger").Info("IUSPTP Exchange service is started", "test");
            new MyLogger("exchangeLogger").Info($@"Connection string is {AppSettingsManager.ConnectionString}", "test");

        }



        protected override void OnStop()
        {

            // TODO: Add code here to perform any tear-down necessary to stop your service.

            _jobHost.Stop();
            _importAgent = null;

            new MyLogger("exchangeLogger").Info("IUSPTP Exchange service is stopped", "test");

        }
    }
}
