using System.Windows;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Columns
{
    public abstract class DataElementBase : AnyViewBase
    {

        private double _height;
        private double _width;
        private int _z;
        private Point _pos;


        protected DataElementBase(DashboardElementContainer dashboard) : base(dashboard)
        {
        }

        /// <summary>
        /// положение элемента
        /// </summary>
        public Point Position
        {
            get { return _pos; }
            set
            {
                _pos = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Z - индекс
        /// </summary>
        public int Z
        {
            get { return _z; }
            set
            {
                _z = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Высота элемента
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value <= 0 ? 0 : value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Ширина элемента
        /// </summary>
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value <= 0 ? 0 : value;
                UpdatePosition();
            }
        }

    }
}
