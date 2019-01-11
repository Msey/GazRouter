using GazRouter.Common.ViewModel;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DataExchange.Integro.ASSPOOTI
{
    public class MappingDescriptorDto : PropertyChangedBase
    {
        public int Id { get; set; }
        public string Key { get; set; }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged(() => Description);
            }
        }
        public string FullLine { get; set; }
    }
}
