using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ExchangeService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += ProjectInstaller_AfterInstall;

            var assembly = Assembly.GetAssembly(typeof(ExchangeService));
            var config = ConfigurationManager.OpenExeConfiguration(assembly.Location);
            serviceProcessInstaller1.Account = ServiceAccount.User;
            serviceProcessInstaller1.Username = config.AppSettings.Settings["InstallLogin"]?.Value;
            serviceProcessInstaller1.Password = config.AppSettings.Settings["InstallPassword"]?.Value;
            if (string.IsNullOrEmpty(serviceProcessInstaller1.Username) && !string.IsNullOrEmpty(config.AppSettings.Settings["InstallLocalLogin"]?.Value))
            {
                serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
                serviceProcessInstaller1.Username = config.AppSettings.Settings["InstallLocalLogin"]?.Value;
                serviceProcessInstaller1.Password = config.AppSettings.Settings["InstallLocalPassword"]?.Value;
            }
        }

        private void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            //autostart service 
            using (var sc = new ServiceController(serviceInstaller1.ServiceName)) 
            {
                sc.Start();
            }
        }
    }
}
