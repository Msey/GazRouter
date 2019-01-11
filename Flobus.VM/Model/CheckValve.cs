using GazRouter.Common.ViewModel;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.VM.Serialization;
using System;
using System.Windows;

namespace GazRouter.Flobus.VM.Model
{
    public class CheckValve : PropertyChangedBase, ISearchable, ICheckValve
    {
        private Point _position;
        private string _tooltip;
        private int _angle;
        private bool _isFound;

        public CheckValve()
        {
        }

        public CheckValve(CheckValveJson tb)
        {
            Position = tb.Position;
            Tooltip = tb.Tooltip;
            Angle = tb.Angle;
        }

        public Point Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public string Tooltip
        {
            get { return _tooltip; }
            set { SetProperty(ref _tooltip, value); }
        }

        public int Angle
        {
            get { return _angle; }
            set { SetProperty(ref _angle, value); }
        }

        public Guid Id => new Guid();

        public string Name => Tooltip;

        public string ShortPath => Name != null ? Name : "Без наименования";

        public string TypeName => "Обратный клапан";

        public bool IsFound
        {
            get { return _isFound; }
            set { SetProperty(ref _isFound, value); }
        }
    }
}
