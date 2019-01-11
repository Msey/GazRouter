using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using DataProviders;
using GazRouter.Application;
using GazRouter.Application.Localization;
using GazRouter.Controls.Dialogs;
using GazRouter.DTO.Infrastructure.Faults;
using NLog;
using NLog.Config;
using NLog.Targets;
using Telerik.Windows.Automation.Peers;
using Telerik.Windows.Controls;
using Telerik.Windows.Input.Touch;

namespace GazRouter.Client
{
    public partial class App
    {
        private readonly Bootstrapper _bootstrapper = new Bootstrapper();

        public App()
        {
            AutomationManager.AutomationMode = AutomationMode.Disabled;
            StyleManager.IsEnabled = false;
            TouchManager.IsTouchEnabled = false;

            Startup += OnStartup;
            Exit += OnExit;
            UnhandledException += OnUnhandledException;

            var cl = new CultureInfo("ru-RU") {NumberFormat = {CurrencyGroupSeparator = " "}};
            Thread.CurrentThread.CurrentCulture = cl;
            Thread.CurrentThread.CurrentUICulture = cl;

            InitializeComponent();
        }

        private void OnExit(object sender, EventArgs e)
        {
            EventCountWatcher.Stop();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            Settings.DispatherDayStartHour = Convert.ToInt32(e.InitParams["DispatherDayStartHour"]);
            Settings.ServerTimeUtcOffset = Convert.ToInt32(e.InitParams["ServerTimeUtcOffset"]);
            Settings.ServerAssemblyVersion = e.InitParams["ServerAssemblyVersion"];
            Settings.ServerAssemblyDate = new DateTime(Convert.ToInt64(e.InitParams["ServerAssemblyDateInTicks"]));
            Settings.EnterpriseId = Guid.Parse(e.InitParams["EnterpriseId"]);

            LocalizationManager.Manager = new RusLocalizationManager
            {
                ResourceManager =
                    TelerikResources.
                        ResourceManager
            };

            _bootstrapper.Run();
            InitializeNLog();
            LogManager.GetCurrentClassLogger().Debug("App started!");

            //Отключение системного контекстного меню
            HtmlPage.Document.AttachEvent("oncontextmenu", (o, args) => args.PreventDefault());

            Initialize();
        }

        private void InitializeNLog()
        {
            var config = new LoggingConfiguration();

/*#if (DEBUG)
	        var networkTarget = new NLogViewerTarget() {Address = "tcp4://10.240.5.105:4505"};
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, networkTarget));
#endif*/
            var traceTarget = new DebuggerTarget();
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, traceTarget));

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, new MethodCallTarget
            {
                ClassName = $"{GetType().AssemblyQualifiedName}",
                MethodName = "LogMessage",
                Parameters =
                {
                    new MethodCallParameter("${level}"), // {Layout = "$(level)",Name = "level", Type = typeof(string)},
                    new MethodCallParameter("${message}") // { Name = "message", Type = typeof(string) } 
                }
            }));

            //	        config.AddTarget(logReceiverWebServiceTarget);

            LogManager.Configuration = config;
        }

        private async void Initialize()
        {
            EventCountWatcher.Run();
            DispatcherTasksWatcher.Run();
        }
        private void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            var faultException = e.ExceptionObject.InnerException != null
                ? e.ExceptionObject.InnerException as FaultException<FaultDetail>
                : e.ExceptionObject as FaultException<FaultDetail>;
            if (faultException != null && faultException.Detail.FaultType == FaultType.IntegrityConstraint)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Header = "Ошибка",
                    Content = faultException.Detail.Message
                });
            }
            else
            {
                DialogHelper.ShowErrorWindow(e, DataProvideSettings.ServerUri);
            }
            e.Handled = true;
        }
    }
}