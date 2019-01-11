using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;
using Utils.Extensions;

namespace GazRouter.Common.ViewModel
{
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        [NotifyPropertyChangedInvocator]
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                OnUiThread(() => handler(this, new PropertyChangedEventArgs(propertyName)));
            }
        }

        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            OnPropertyChanged(property.GetMemberInfo().Name);
        }

        private void OnUiThread(Action onUiThreadAction)
        {
#if (SILVERLIGHT)
            if (Deployment.Current.Dispatcher.CheckAccess())
            {
                onUiThreadAction.Invoke();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(onUiThreadAction);
            }
#else
            if (Application.Current.Dispatcher.CheckAccess())
            {
                onUiThreadAction.Invoke();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(onUiThreadAction);
            }
#endif
        }
    }
}