using System.Windows;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements
{

    public abstract class ElementViewBase : AnyViewBase
    {
        #region PROPERTIES

        private readonly ElementModelBase _elementModel;

        public ElementModelBase ElementModel
        {
            get { return _elementModel; }
        }

        /// <summary>
        /// Z - индекс
        /// </summary>
        public int Z
        {
            get { return _elementModel.Z; }
            set
            {
                _elementModel.Z = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Ширина элемента
        /// </summary>
        public double Width
        {
            get { return _elementModel.Width; }
            set
            {
                _elementModel.Width = value <= 0 ? 0 : value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Высота элемента
        /// </summary>
        public double Height
        {
            get { return _elementModel.Height; }
            set
            {
                _elementModel.Height = value <= 0 ? 0 : value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// положение элемента
        /// </summary>
        public Point Position
        {
            get { return _elementModel.Position; }
            set
            {
                _elementModel.Position = value;
                UpdatePosition();
            }
        }


        #endregion

        protected ElementViewBase(DashboardElementContainer dashboard, ElementModelBase elementModel)
            : base(dashboard)
        {
            _elementModel = elementModel;
        }

        public virtual void Move(double xOffset, double yOffset)
        {
            Position = new Point(Position.X + xOffset, Position.Y + yOffset);
            UpdatePosition();
        }

        public virtual void UpdateData()
        {
        }


    }


}