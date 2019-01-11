using System.Windows;
using System.Windows.Data;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.GasCosts;
using Telerik.Windows.Controls;
namespace GazRouter.Modes.GasCosts
{
    public class GasCostColumnDefinition : PropertyChangedBase
    {
        public GasCostColumnDefinition()
        {
            Width = GridViewLength.Auto;
        }
        public string UniqueName { get; set; }
        private int _displayIndex = -1;
        public int DisplayIndex
        {
            get
            {
                return _displayIndex;
            }
            set
            {
                if (_displayIndex != value)
                {
                    _displayIndex = value;
                    OnPropertyChanged(() => DisplayIndex);
                }
            }
        }
        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(() => IsVisible);
                }
            }
        }
        private string _header;
        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged(() => Header);
                }
            }
        }
        public string HeaderToolTip
        {
            get
            {
                return _headerToolTip;
            }
            set
            {
                if (value == _headerToolTip)
                {
                    return;
                }
                _headerToolTip = value;
                OnPropertyChanged(() => HeaderToolTip);
            }
        }
        private Style _headerCellStyle;
        public Style HeaderCellStyle
        {
            get
            {
                return _headerCellStyle;
            }
            set
            {
                if (_headerCellStyle != value)
                {
                    _headerCellStyle = value;
                    OnPropertyChanged(() => HeaderCellStyle);
                }
            }
        }
        private Binding _dataMemberBinding;
        public Binding DataMemberBinding
        {
            get
            {
                return _dataMemberBinding;
            }
            set
            {
                if (_dataMemberBinding != value)
                {
                    _dataMemberBinding = value;
                    OnPropertyChanged(() => DataMemberBinding);
                }
            }
        }
        private bool _isResizable;
        public bool IsResizable
        {
            get
            {
                return _isResizable;
            }
            set
            {
                if (_isResizable != value)
                {
                    _isResizable = value;
                    OnPropertyChanged(() => IsResizable);
                }
            }
        }
        private bool _isSorteable;
        public bool IsSorteable
        {
            get
            {
                return _isSorteable;
            }
            set
            {
                if (_isSorteable != value)
                {
                    _isSorteable = value;
                    OnPropertyChanged(() => IsSorteable);
                }
            }
        }
        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged(() => IsReadOnly);
                }
            }
        }
        private bool _isGroupable;
        public bool IsGroupable
        {
            get
            {
                return _isGroupable;
            }
            set
            {
                if (_isGroupable != value)
                {
                    _isGroupable = value;
                    OnPropertyChanged(() => IsGroupable);
                }
            }
        }
        private bool _isFilterable;
        public bool IsFilterable
        {
            get
            {
                return _isFilterable;
            }
            set
            {
                if (_isFilterable != value)
                {
                    _isFilterable = value;
                    OnPropertyChanged(() => IsFilterable);
                }
            }
        }
        private TextAlignment _headerTextAlignment;
        public TextAlignment HeaderTextAlignment
        {
            get
            {
                return _headerTextAlignment;
            }
            set
            {
                if (_headerTextAlignment != value)
                {
                    _headerTextAlignment = value;
                    OnPropertyChanged(() => HeaderTextAlignment);
                }
            }
        }
        private StyleSelector _cellStyleSelector;
        public StyleSelector CellStyleSelector
        {
            get
            {
                return _cellStyleSelector;
            }
            set
            {
                if (_cellStyleSelector != value)
                {
                    _cellStyleSelector = value;
                    OnPropertyChanged(() => CellStyleSelector);
                }
            }
        }
        private DataTemplateSelector _cellTemplateSelector;
        public DataTemplateSelector CellTemplateSelector
        {
            get
            {
                return _cellTemplateSelector;
            }
            set
            {
                if (_cellTemplateSelector != value)
                {
                    _cellTemplateSelector = value;
                    OnPropertyChanged(() => CellTemplateSelector);
                }
            }
        }
        private double _maxWidth = 1000;
        private string _headerToolTip;
        private TextAlignment _textAligment;
        private DataTemplate _cellTemplate;
        public double MaxWidth
        {
            get
            {
                return _maxWidth;
            }
            set
            {
                if (_maxWidth != value)
                {
                    _maxWidth = value;
                    OnPropertyChanged(() => MaxWidth);
                }
            }
        }
        public object Tag { get; set; }
        public GridViewLength Width { get; set; }
        public TextAlignment TextAligment
        {
            get
            {
                return _textAligment;
            }
            set
            {
                _textAligment = value;
                OnPropertyChanged(()=>TextAligment);
            }
        }
        public DataTemplate CellTemplate
        {
            get { return _cellTemplate; }
            set
            {
                SetProperty(ref _cellTemplate, value);
            }
        }
        public CostType CostType     { get; set; }
    }
}