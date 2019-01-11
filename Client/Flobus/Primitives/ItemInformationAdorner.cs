using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using JetBrains.Annotations;

namespace GazRouter.Flobus.Primitives
{
    public class ItemInformationAdorner : Control, INotifyPropertyChanged
    {

        public static readonly DependencyProperty InformationTipProperty = DependencyProperty.Register(
         nameof(InformationTip), typeof(object), typeof(ItemInformationAdorner), new PropertyMetadata(null, OnInformationTipChanged));


        public ItemInformationAdorner()
        {
            DefaultStyleKey = typeof (ItemInformationAdorner);
        }

        private bool _isAdditionalContentVisible;
        public event EventHandler IsAdditionalContentVisibleChanged;
        public Schema Schema { get; set; }

        public bool IsAdditionalContentVisible
        {
            get { return _isAdditionalContentVisible; }
            set
            {
                if (value == _isAdditionalContentVisible) return;
                _isAdditionalContentVisible = value;
                OnPropertyChanged();
                OnIsAdditionalContentVisibleChanged();
            }
        }

     

        private ContentPresenter _informationTipPresenter;
        private Panel _informationTipPanel;
        private bool _isHiding;
        private Storyboard _hidingStoryBoard;

        private static void OnInformationTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((ItemInformationAdorner) d).UpdateInformationTip();
        }

        private void UpdateInformationTip()
        {
            if (_informationTipPresenter != null)
            {
                var informationTipValue = InformationTip;
                _informationTipPresenter.Content = informationTipValue;
            }
        }

        public object InformationTip
        {
            get { return GetValue(InformationTipProperty); }
            set { SetValue(InformationTipProperty, value); }
        }

        public bool IsInformationTipVisible
        {
            get { return _informationTipPanel != null && _informationTipPanel.Visibility == Visibility.Visible; }
            internal set
            {
                if (_informationTipPanel == null) return;

                if (value)
                {
                    ShowInformationTip();
                }
                else
                {
                    HideInformationTip();
                }
            }
        }

  

        private void ShowInformationTip()
        {
            if (_isHiding)
            {
                _hidingStoryBoard.Stop();
                _isHiding = false;
            }

            if (_informationTipPanel != null)
            {
                Storyboard storyboard = new Storyboard {Duration = new Duration(TimeSpan.FromSeconds(0))};

                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    To = 1,
                    From = 1,
                    BeginTime = TimeSpan.FromSeconds(0)
                };

                Storyboard.SetTarget(opacityAnimation, _informationTipPanel);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(nameof(_informationTipPanel.Opacity)));
                storyboard.Children.Add(opacityAnimation);

                ObjectAnimationUsingKeyFrames visibilityAnimation = new ObjectAnimationUsingKeyFrames();
                visibilityAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame {Value = Visibility.Visible, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))});
                Storyboard.SetTarget(visibilityAnimation, _informationTipPanel);
                Storyboard.SetTargetProperty(visibilityAnimation, new PropertyPath(nameof(_informationTipPanel.Visibility)));
                storyboard.Children.Add(visibilityAnimation);

                storyboard.Begin();
            }
        }

        private void HideInformationTip()
        {
            if (_informationTipPanel == null) return;

            if (_hidingStoryBoard == null)
            {
                _hidingStoryBoard = new Storyboard {Duration = new Duration(TimeSpan.FromSeconds(2))};
                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    To = 0,
                    From = 1,
                    BeginTime = TimeSpan.FromSeconds(1.5),
                    Duration = new Duration(TimeSpan.FromSeconds(0.5))
                };
                Storyboard.SetTarget(opacityAnimation, _informationTipPanel);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(nameof(_informationTipPanel.Opacity)));
                _hidingStoryBoard.Children.Add(opacityAnimation);

                ObjectAnimationUsingKeyFrames visibilityAnimation = new ObjectAnimationUsingKeyFrames();
                visibilityAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame() {Value = Visibility.Collapsed, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2))});
                Storyboard.SetTarget(visibilityAnimation,_informationTipPanel);
                Storyboard.SetTargetProperty(visibilityAnimation, new PropertyPath(nameof(_informationTipPanel.Visibility)));
                _hidingStoryBoard.Children.Add(visibilityAnimation);
                _hidingStoryBoard.Completed += (sender, args) =>
                {
                    if (_isHiding)
                    {
                        _isHiding = false;
                    }
                };
            }

            _isHiding = true;
            _hidingStoryBoard.Begin();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnIsAdditionalContentVisibleChanged()
        {
            IsAdditionalContentVisibleChanged?.Invoke(this, System.EventArgs.Empty);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _informationTipPresenter = GetTemplateChild("InformationTipPresenter") as ContentPresenter;
            _informationTipPanel = GetTemplateChild("InformationTipPanel") as Panel;
            
            UpdateInformationTip();

        }
    }
}