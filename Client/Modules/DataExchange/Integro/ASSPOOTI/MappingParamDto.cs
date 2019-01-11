using GazRouter.Common.GoodStyles;
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
    public class MappingParamDto : PropertyChangedBase
    {
        public Guid SummaryParameterId { get; set; }
        private int? parameterDescriptorId;
        public int? ParameterDescriptorId
        {
            get { return parameterDescriptorId; }
            set
            {
                parameterDescriptorId = value;
                OnPropertyChanged(() => ParameterDescriptorId);
                OnPropertyChanged(() => FullLinkName);
            }
        }
        //
        private string parameterDescriptorNameNew;
        public string ParameterDescriptorNameNew
        {
            get { return parameterDescriptorNameNew; }
            set
            {
                parameterDescriptorNameNew = value;
                OnPropertyChanged(() => ParameterDescriptorNameNew);
                OnPropertyChanged(() => IsLinked);
                OnPropertyChanged(() => RowColor);
            }
        }        
        //
        private string parameterDescriptorName;
        public string ParameterDescriptorName
        {
            get { return parameterDescriptorName; }
            set
            {
                parameterDescriptorName = value;
                OnPropertyChanged(() => ParameterDescriptorName);
                OnPropertyChanged(() => FullLinkName);
                OnPropertyChanged(() => IsLinked);
                OnPropertyChanged(() => RowColor);
            }
        }
        //
        public bool IsLinked
        {
            get { return ParameterDescriptorName == ParameterDescriptorNameNew; }
        }

        public Brush RowColor
        {
            get { return !IsLinked ? Brushes.Red : Brushes.Transparent; }
        }
        //public string ParameterGid { get; set; }
        //
        private Guid entityId;
        public Guid EntityId
        {
            get { return entityId; }
            set
            {
                entityId = value;
                OnPropertyChanged(() => EntityId);
                OnPropertyChanged(() => FullLinkName);
            }
        }
        //
        private int? propertyTypeId;
        public int? PropertyTypeId
        {
            get { return propertyTypeId; }
            set
            {
                propertyTypeId = value;
                OnPropertyChanged(() => PropertyTypeId);
                OnPropertyChanged(() => FullLinkName);
            }
        }
        //
        public string propertyTypeName { get; set; }
        public string PropertyTypeName
        {
            get { return propertyTypeName; }
            set
            {
                propertyTypeName = value;
                OnPropertyChanged(() => PropertyTypeName);
                OnPropertyChanged(() => FullLinkName);
            }
        }
        public string Description { get; set; }
        public string FullLinkName { get { return string.Format("{0} / {1}", ParameterDescriptorName, PropertyTypeName); } }
    }
}
