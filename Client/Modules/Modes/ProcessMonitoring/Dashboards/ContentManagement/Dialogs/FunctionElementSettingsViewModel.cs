using GazRouter.Common.ViewModel;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using GazRouter.Controls.Dialogs;
using System.Linq;
using System;
using System.Collections.Generic;
using GazRouter.Application.Helpers;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs
{
    public class FunctionElementSettingsViewModel : DialogViewModel
    {
        public DelegateCommand SaveCommand { get; private set; }        
        public DelegateCommand SetParameter { get; private set; }
        private readonly FunctionElementModel _model;
        private SelectEntityPropertyViewModel _psvm;

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged(() => Comment);
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }
        private int _fontSize;
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                OnPropertyChanged(() => FontSize);
            }
        }

        private Period _selectedPeriod;
        public Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                //if (SetProperty(ref _selectedPeriod, value))
                //    Refresh();
                _selectedPeriod = value;
                OnPropertyChanged(() => IsBoxVisible);
            }
        }
        private List<Function> _funcs;
        public List<Function> FuncList
        {
            get { return _funcs; }
            set
            {
                _funcs = value;
                OnPropertyChanged(() => FuncList);
            }
        }

        private Function _func;
        public Function Func
        {
            get { return _func; }
            set
            {
                _func = value;
                PeriodPickerVisible = value == Function.AVG ? false : true;
                OnPropertyChanged(() => Func);
            }
        }

        private Color _fontColor;
        public Color FontColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                OnPropertyChanged(() => FontColor);
            }
        }

        private bool _isBoxVisible;
        public bool IsBoxVisible
        {
            get { return _isBoxVisible; }
            set
            {
                _isBoxVisible = value;
                OnPropertyChanged(() => IsBoxVisible);
            }
        }

        private bool _periodPickerVisible;
        public bool PeriodPickerVisible
        {
            get { return _periodPickerVisible; }
            set
            {
                _periodPickerVisible = value;
                OnPropertyChanged(() => PeriodPickerVisible);
            }
        }

        private bool _showTitle;
        public bool ShowTitle
        {
            get { return _showTitle; }
            set
            {
                _showTitle = value;
                OnPropertyChanged(() => ShowTitle);
            }
        }



        public FunctionElementSettingsViewModel(FunctionElementModel model,bool IsAdd)
            : base(() => { })
        {
            FuncList = new List<Function>()
            {
                Function.SUMM,
                Function.AVG,
                Function.MAX,
                Function.MIN
            };
            SelectedPeriod = new Period(DateTime.Now.AddDays(-1), DateTime.Now);
            PeriodPickerVisible = false;
            _model = model;

            _comment = model.Comment;
            _fontColor = model.FontColor;
            _fontSize = model.FontSize;
            _isBoxVisible = model.IsBoxVisible;
            _showTitle = model.ShowTitle;

            SetParameter = new DelegateCommand(OnAddPropertyElement);
            SaveCommand = new DelegateCommand(OnSave);
        }


        private void OnSave()
        {
            _model.Comment = _comment;
            _model.FontSize = _fontSize;
            _model.FontColor = _fontColor;
            _model.IsBoxVisible = _isBoxVisible;
            _model.ShowTitle = _showTitle;
            _model.PropertyType = Element.PropertyType;
            _model.EntityName = Element.EntityName;
            _model.EntityType = Element.EntityType;
            _model.EntityId = Element.EntityId;
            DialogResult = true;
        }

        public PropertyElementModel Element = new PropertyElementModel();
        
        private void OnAddPropertyElement()
        {
            _psvm = new SelectEntityPropertyViewModel(() =>
            {
                Element = new PropertyElementModel
                {
                    EntityId = _psvm.SelectedItem.Id,
                    EntityName = _psvm.SelectedItem.ShortPath,
                    EntityType = _psvm.SelectedItem.EntityType,
                    PropertyType = _psvm.SelectedEntityProperty.PropertyType,
                    Comment = _psvm.SelectedItem.ShortPath
                    + Environment.NewLine
                    + ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == _psvm.SelectedEntityProperty.PropertyType).Name

                };
                Title = Element.EntityName;
                Comment = Element.Comment;

            }, null, /*для примера - Только Давление new List<PhysicalType>{PhysicalType.Pressure}*/null);

            var ps = new SelectEntityProperty { DataContext = _psvm };
            ps.Show();
        }
    }

    /*
     <telerik:RadComboBoxItem Content="Сумма" FontSize="8"/>
                <telerik:RadComboBoxItem Content="Среднее значение" FontSize="9"/>
                <telerik:RadComboBoxItem Content="Максимум" FontSize="10"/>
                <telerik:RadComboBoxItem Content="Минимум" FontSize="11"/>
    */
    public enum Function
    {
        SUMM = 1,
        AVG = 2,
        MAX = 3,
        MIN = 4
    }
}
