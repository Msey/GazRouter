using GazRouter.Common.ViewModel;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Dialogs
{
    public class PolyLineStyleEditDialogViewModel : DialogViewModel
    {
        private double _thickness;
        private Color _stroke;
        private string _name;
        private string _description;
        private int _type;
        public DelegateCommand SaveCommand { get; private set; }
        private readonly IPolyLineWidget _model;


        public double StrokeThickness
        {
            get { return _thickness; }
            set
            {
                _thickness = value;
                OnPropertyChanged(() => StrokeThickness);
            }
        }

        public Color Stroke
        {
            get { return _stroke; }
            set
            {
                _stroke = value;
                OnPropertyChanged(() => Stroke);
            }
        }
                
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(() => Description);
            }
        }
        
        public int Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(() => Type);
            }
        }

        public PolyLineStyleEditDialogViewModel(IPolyLineWidget model)
            : base(() => { })
        {
            _model = model;

            _stroke = model.Stroke;
            _thickness = model.StrokeThickness;
            _description = model.Description;
            _type = (int)model.Type;
            _name = model.Name;
            SaveCommand = new DelegateCommand(OnSave);
        }
        private void OnSave()
        {
            _model.Stroke = _stroke;
            _model.StrokeThickness = _thickness;
            _model.Description = _description;
            _model.Type = (LineType)_type;
            _model.Name = _name;

            DialogResult = true;
        }


    }
}
