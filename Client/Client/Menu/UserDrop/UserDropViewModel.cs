using System;
using System.Windows;
using System.Windows.Browser;
using GazRouter.Application;
using GazRouter.Client.LogViewer;
using GazRouter.Client.UserSettings;
using GazRouter.Common;
using UserSettingsView = GazRouter.Client.UserSettings.UserSettingsView;

namespace GazRouter.Client.Menu.UserDrop
{
    public class UserDropViewModel : DropBase
    {
        public UserDropViewModel()
        {
            LastChangesLink = new LinkMenuItem("Последние изменения", ViewLastChanges, "/Common;component/Images/16x16/changelog.png");
            GuideLink = new LinkMenuItem("Инструкция",DownloadGuide, "/Common;component/Images/16x16/log.png");
            SettingsLink = new LinkMenuItem("Настройки...", ChangeSettings, "/Common;component/Images/32x32/settings.png");
            LogLink = new LinkMenuItem("Лог...", () => new LogViewer.LogViewer().Show(), "/Common;component/Images/16x16/history.png");
            LogViewerViewModel.Init();
            //
            ClearStorageLink = new LinkMenuItem("Очистить хранилище", 
                () => {
                    IsolatedStorageManager.Clear();
                    MessageBox.Show("Хранилище очищено");
                }, 
                "/Common;component/Images/16x16/trash.png"); 
        }

        public string UserName => UserProfile.Current.UserName;

        public string SiteName => UserProfile.Current.Site.Name;


        /// <summary>
        /// Версия сборки
        /// </summary>
        public string Version => Settings.ServerAssemblyVersion;

        /// <summary>
        /// Дата сборки
        /// </summary>
        public DateTime VersionDate => Settings.ServerAssemblyDate;

        /// <summary>
        /// Последние изменения
        /// </summary>
        public LinkMenuItem LastChangesLink { get; set; }

        /// <summary>
        /// Скачать инструкцию
        /// </summary>
        public LinkMenuItem GuideLink { get; set; }

        public LinkMenuItem SettingsLink { get; set; }

        /// <summary>
        /// Открыть окно лога
        /// </summary>
        public LinkMenuItem LogLink { get; set; }

        /// <summary>
        /// Очистка пользовательского хранилища
        /// </summary>
        public LinkMenuItem ClearStorageLink { get; set; }


        private void ChangeSettings()
        {
            new UserSettingsView { DataContext = new UserSettingsViewModel() }.ShowDialog();
        }

        private async void DownloadGuide()
        {
            string helpFile = await new DataProviders.SystemVariables.SysVarServiceProxy().GetHelpFileNameAsync();
            if(!String.IsNullOrWhiteSpace(helpFile))
                HtmlPage.Window.Navigate(DataProviders.UriBuilder.GetServiceUri(helpFile), "_blank");
        }

        private async void ViewLastChanges()
        {
            string file = await new DataProviders.SystemVariables.SysVarServiceProxy().GetLastChangesFileNameAsync();
            if (!String.IsNullOrWhiteSpace(file))
                HtmlPage.Window.Navigate(DataProviders.UriBuilder.GetServiceUri(file), "_blank");
        }
    }
}