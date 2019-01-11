using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Expression = System.Linq.Expressions.Expression;

namespace GazRouter.Common.Ui
{
    public class Attach : FrameworkElement
    {
        private string _eventName;
        public string EventName
        {
            get
            {
                return _eventName;
            }
            set
            {
                RemoveEventHandler();
                _eventName = value;
                UpdateEventInfo();
                AddEventHandler();
            }
        }

        private string _behaviorPropertyName;
        public string BehaviorPropertyName
        {
            get
            {
                return _behaviorPropertyName;
            }
            set
            {
                _behaviorPropertyName = value;
                UpdatePropertyInfo();
                SetBehaviorProperty();
            }
        }

        private FrameworkElement _target;
        public FrameworkElement Target
        {
            get
            {
                return _target;
            }
            set
            {
                RemoveEventHandler();
                _target = value;
                UpdateEventInfo();
                UpdatePropertyInfo();
                AddEventHandler();
                SetBehaviorProperty();
            }
        }

        void TargetLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Target.DataContext;
        }

        public DelegateCommand<object[]> Command
        {
            get
            {
                return (DelegateCommand<object[]>)GetValue(CommandProperty);
            }
            set { SetValue(CommandProperty, value); }
        }

        private Delegate _currentHandler;
        private EventInfo _currentEventInfo;
        private PropertyInfo _currentBehaviorProperty;

        private void UpdatePropertyInfo()
        {
            _currentBehaviorProperty = null;
            if (Target == null)
                return;
            if (string.IsNullOrEmpty(_behaviorPropertyName))
                return;

            _currentBehaviorProperty = Target.GetType().GetProperty(_behaviorPropertyName);
        }

        private void UpdateEventInfo()
        {
            _currentEventInfo = null;
            if (Target == null)
                return;
            if (string.IsNullOrEmpty(EventName))
                return;

            _currentEventInfo = Target.GetType().GetEvent(EventName);
        }

        private void RemoveEventHandler()
        {
            if (Target == null)
                return;

            Target.Loaded -= TargetLoaded;

            if (_currentHandler == null)
                return;

            _currentEventInfo.RemoveEventHandler(Target, _currentHandler);
            _currentHandler = null;
        }

        private void AddEventHandler()
        {
            if (Target == null)
                return;

            Target.Loaded += TargetLoaded;

            if (_currentEventInfo == null)
                return;
            if (_currentHandler != null)
            {
                RemoveEventHandler();
            }


            var handlerType = _currentEventInfo.EventHandlerType;
            var eventParams = handlerType.GetMethod("Invoke").GetParameters();
            ParameterExpression[] parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
            var body = Expression.Call(Expression.Constant(this),
                                                   GetType().GetMethod("EventRaised"),
                                                   Expression.NewArrayInit(typeof(object), parameters));
            Delegate handler = Expression.Lambda(body, parameters.ToArray()).Compile();
            _currentEventInfo.AddEventHandler(Target,
                                              Delegate.CreateDelegate(_currentEventInfo.EventHandlerType, handler, "Invoke", false));
            _currentHandler = handler;
        }



        public void Update()
        {
            SetBehaviorProperty();
        }

        public void EventRaised(object[] eventArgs)
        {
            bool canExecute = SetBehaviorProperty();
            if (canExecute)
                Command.Execute(eventArgs);
        }

        private bool SetBehaviorProperty()
        {
            if (Command == null)
                return false;

            bool canExecute = Command.CanExecute(null);

            if (_currentBehaviorProperty == null)
                return canExecute;

            _currentBehaviorProperty.SetValue(Target, canExecute, null);
            return canExecute;
        }

        public void CanExecuteChanged(object sender, EventArgs e)
        {
            SetBehaviorProperty();
        }

        public static AttachCollection GetAttachCollection(DependencyObject obj)
        {
            return (AttachCollection)obj.GetValue(AttachCollectionProperty);
        }

        public static void SetAttachCollection(DependencyObject obj, AttachCollection value)
        {
            foreach (var item in value)
            {
                item.Target = (FrameworkElement)obj;
                item.DataContext = value.DataContext;
            }
            obj.SetValue(AttachCollectionProperty, value);
        }

        public static readonly DependencyProperty AttachCollectionProperty =
            DependencyProperty.RegisterAttached("AttachCollection", typeof(AttachCollection), typeof(Attach), null);

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(DelegateCommand<object[]>), typeof(Attach), new PropertyMetadata(CommandChangedHandler));

        public static void CommandChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var attach = (Attach) d ;

            var newCommand = e.NewValue as DelegateCommand<object[]>;
            var oldCommand = e.OldValue as DelegateCommand<object[]>;

            if (oldCommand != null)
                oldCommand.CanExecuteChanged -= attach.CanExecuteChanged;
            if (newCommand != null)
                newCommand.CanExecuteChanged += attach.CanExecuteChanged;

            attach.Update();
        }
    }
}