using System;
using System.Collections.Generic;
using System.Windows.Browser;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GazRouter.Common.GoodStyles;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
namespace GazRouter.Client.Menu
{
    public class MenuItemBase : PropertyChangedBase
    {
        private bool _isSelected;
        public MenuItemBase()
        {
        }
        public MenuItemBase(string name)
        {
            Items = new List<MenuItemBase>();

            Name = name;
            OnPropertyChanged(() => Name);
        }
        public MenuItemBase(string name, string imgSrc)
        {
            Items = new List<MenuItemBase>();

            Name = name;
            OnPropertyChanged(() => Name);

            ImageSource = new BitmapImage(new Uri(imgSrc, UriKind.Relative));
            OnPropertyChanged(() => ImageSource);
        }
        public List<MenuItemBase> Items { get; set; }
        public string Name { get; set; }
        public ImageSource ImageSource { get; set; }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }
    }
    public class LinkMenuItem : MenuItemBase
    {
        private string _uri;
        private Action _action;
        private bool _isExternal = false;
        // Пока придумал только так. 
        // Приходиться прокидывать Action, 
        // который будет закрытвать меню, при открытии модуля.
        private Action _closeMenu; 
        public LinkMenuItem(string name, string uri, string imgSrc = "", bool isExternal = false) 
            :base (name, imgSrc){
            _isExternal = isExternal;
            _uri = uri;
            OpenLinkCommand = new DelegateCommand(OpenLink);
        }
        public LinkMenuItem(string name, Action action, string imgSrc = "")
            : base(name, imgSrc)
        {
            _action = action;
            OpenLinkCommand = new DelegateCommand(OpenLink);
        }
        public void AddCloseAction(Action closeMenu)
        {
            _closeMenu = closeMenu;
        }
        public DelegateCommand OpenLinkCommand { get; set; }
        public void OpenLink()
        {
            if (_isExternal)
            {
                HtmlPage.Window.Navigate(new Uri(_uri), "_blank");
                HtmlPage.Document.SetProperty("title", Name ?? "");
                return;
            }
            Back = Brushes.Transparent;
            OnPropertyChanged(() => Back);
            _closeMenu?.Invoke();
            _action?.Invoke();
            if (!string.IsNullOrEmpty(_uri))
                ServiceLocator.Current.GetInstance<IRegionManager>().RequestNavigate("MainRegion", _uri);
            //            ServiceLocator.Current.GetInstance<IRegionManager>()
            //                .RequestNavigate("MainRegion", _uri, NavigationCallback);
            HtmlPage.Document.SetProperty("title", Name ?? "");
        }
        private void NavigationCallback(NavigationResult navigationResult)
        {
            _closeMenu?.Invoke();
            _action?.Invoke();
            var s = navigationResult;
        }
        public Brush Back { get; set; }
    }
    public class SeparatorMenuItem : MenuItemBase { }
    /// <summary> Класс определяющий группу </summary>
    public class SectionMenuItem : MenuItemBase
    {
        public SectionMenuItem(string name) : base(name)
        {

        }
    }
}