using System;
using System.Windows.Media;
using GazRouter.Common.GoodStyles;
using GazRouter.Common.ViewModel;

namespace GazRouter.Modes.ProcessMonitoring.ObjectStory
{
    public class StoryItemBase : ViewModelBase
    {
        private bool _isSelected;



        public DateTime Timestamp { get; set; }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    OnPropertyChanged(() => Background);
                }

            }
        }

        public Brush Background => IsSelected ? Brushes.Orange : Brushes.Transparent;
    }
}