using System.Windows;
using System.Windows.Input;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{
    public abstract class AnyViewBase : IContextMenuGenerator
    {
        protected AnyViewBase(DashboardElementContainer dashboard)
        {
            Dashboard = dashboard;
        }

        public delegate void ElementMoveEventHandler(object sender, Point offset);

        /// <summary>
        ///     ELEMENT MOVE
        /// </summary>
        public event ElementMoveEventHandler ElementMove;

        /// <summary>
        ///     Mouse LEFT button DOWN
        /// </summary>
        public event MouseButtonEventHandler MouseLeftButtonDown;

        /// <summary>
        ///     Mouse LEFT button UP
        /// </summary>
        public event MouseButtonEventHandler MouseLeftButtonUp;

        /// <summary>
        ///     Mouse RIGHT button UP
        /// </summary>
        public event MouseButtonEventHandler MouseRightButtonUp;

        /// <summary>
        ///     Ссылка на контейнер
        /// </summary>
        public DashboardElementContainer Dashboard { get; private set; }

        public virtual Visibility Visibility { get; set; }
        protected bool IsDraging { get; private set; }

        public abstract void Destroy();

        public abstract void UpdatePosition();

        public virtual void UpdateContents()
        {
            UpdatePosition();
        }

        public virtual bool Select()
        {
            return true;
        }

        public virtual void Deselect()
        {
        }

        public virtual void StartDrag()
        {
            IsDraging = true;
        }

        public virtual void EndDrag()
        {
            IsDraging = false;
        }

        public void NotifyMouseRightButtonUp(object sender, MouseButtonEventArgs args)
        {
            MouseRightButtonUp?.Invoke(sender, args);
        }

        public void NotifyElementMove(object sender, Point offset)
        {
            ElementMove?.Invoke(sender, offset);
        }

        public void NotifyMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            MouseLeftButtonUp?.Invoke(sender, args);
        }

        public void NotifyMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            MouseLeftButtonDown?.Invoke(sender, args);
        }

        public virtual void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            menu.Items.Clear();
        }
    }
}