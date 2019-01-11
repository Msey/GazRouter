using System;
using System.Windows;
using Utils.Extensions;
using PropertyMetadata = Telerik.Windows.PropertyMetadata;

namespace GazRouter.Controls.UserStamp
{
    public partial class UserStamp
    {
        public UserStamp()
        {
            InitializeComponent();

            TimestampTxt.Visibility = Visibility.Collapsed;
        }



        #region Timestamp
        public DateTime? Timestamp
        {
            get { return GetValue(TimestampProperty) as DateTime?; }
            set { SetValue(TimestampProperty, value); }
        }


        public static readonly DependencyProperty TimestampProperty =
            DependencyProperty.Register(
                "Timestamp",
                typeof(DateTime?),
                typeof(UserStamp),
                new PropertyMetadata(OnTimestampPropertyChanged));


        private static void OnTimestampPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as UserStamp;
            if (ctrl?.Timestamp != null)
            {
                ctrl.TimestampTxt.Text = ctrl.Timestamp.Value.ToDailyDateTimeString();
                ctrl.TimestampTxt.Visibility = Visibility.Visible;
            }
            else
                ctrl.TimestampTxt.Visibility = Visibility.Collapsed;
        }

        #endregion


        #region UserName
        public string UserName
        {
            get { return GetValue(UserNameProperty) as string; }
            set { SetValue(UserNameProperty, value); }
        }


        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register(
                "UserName",
                typeof(string),
                typeof(UserStamp),
                new PropertyMetadata(OnUserNamePropertyChanged));


        private static void OnUserNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as UserStamp;
            if (ctrl != null)
                ctrl.UserNameTxt.Text = ctrl.UserName;
        }

        #endregion



        #region SiteName
        public string SiteName
        {
            get { return GetValue(SiteNameProperty) as string; }
            set { SetValue(SiteNameProperty, value); }
        }


        public static readonly DependencyProperty SiteNameProperty =
            DependencyProperty.Register(
                "SiteName",
                typeof(string),
                typeof(UserStamp),
                new PropertyMetadata(OnSiteNamePropertyChanged));


        private static void OnSiteNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as UserStamp;
            if (ctrl != null)
                ctrl.SiteNameTxt.Text = ctrl.SiteName;
        }

        #endregion
    }

}
