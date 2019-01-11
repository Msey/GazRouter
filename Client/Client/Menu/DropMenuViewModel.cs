using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
namespace GazRouter.Client.Menu
{
    public class DropMenuViewModel : ViewModelBase
    {
        [Obsolete]
        public DropMenuViewModel(string name, bool include = true)
        {
            Name = name;       
            if (include) AddModule?.Invoke(Name);
        }

        public DropMenuViewModel(LinkType linkType, bool include = true)
        {
            _include    = include;
            var link    = LinkRegister.GetLinkInfo(linkType);
            _linkType   = linkType;
            Name        = link.Name;
            ImageSource = link.Image;        
        }

        private readonly bool _include;

        public static Action<string> AddModule { get; set; }
        private readonly LinkType _linkType;
        private string _imgSrc;
        private int _notificationCount;
        public string ImageSource
        {
            get { return _imgSrc; }
            set
            {
                _imgSrc = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Image = new BitmapImage(new Uri(value, UriKind.Relative));
                    OnPropertyChanged(() => Image);
                }
            }
        }
        public string Name { get; set; }
        public ImageSource Image { get; set; }
        public DropBase DropContent { get; private set; }
        public bool IsOpen { get; set; }
        /// <summary>
        /// Счетчик уведомлений
        /// </summary>
        public int NotificationCount
        {
            get { return _notificationCount; }
            set
            {
                if (SetProperty(ref _notificationCount, value))
                {
                    OnPropertyChanged(() => HasNotification);
                }
            }
        }
        public bool HasNotification => NotificationCount > 0;

        public void AddDropContent(DropBase dropContent)
        {
            DropContent = dropContent;
            DropContent.AddCloseAction(() =>
            {
                IsOpen = false;
                OnPropertyChanged(() => IsOpen);
            });
            //
            if(_include) dropContent.RegisterDropMenu(_linkType);
        }
    }
}