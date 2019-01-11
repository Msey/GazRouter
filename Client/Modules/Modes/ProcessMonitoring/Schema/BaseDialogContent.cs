using System.Windows;
using GazRouter.Common.ViewModel;

namespace GazRouter.Modes.ProcessMonitoring.Schema
{
    public abstract class BaseDialogContent : LockableViewModel
    {
        private double _height;
        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        private double _left;
        public double Left
        {
            get { return _left; }
            set { SetProperty(ref _left, value); }
        }

        private double _top;
        public double Top
        {
            get { return _top; }
            set { SetProperty(ref _top, value); }
        }

        private bool _dialogResult;
        public bool DialogResult
        {
            get { return _dialogResult; }
            set { SetProperty(ref _dialogResult, value); }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }

        private Visibility _dialogVisibility;
        public Visibility DialogVisibility
        {
            get { return _dialogVisibility; }
            set { SetProperty(ref _dialogVisibility, value); }
        }
    }
}