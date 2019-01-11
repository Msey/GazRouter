using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.VM.Model;

namespace GazRouter.Flobus.VM.Dialogs
{
    public class FindObjectViewModel : PropertyChangedBase
    {
        private ISearchable _selectedItem;
        private string _findCriteria = string.Empty;

        public FindObjectViewModel(SchemeViewModel model)
        {
            Model = model;
        }

        public SchemeViewModel Model { get; set; }

        public ISearchable SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != null)
                {
                    _selectedItem.IsFound = false;
                }
                if (SetProperty(ref _selectedItem, value))
                {
                    if (value != null)
                    {
                        SelectedItem.IsFound = true;
                    }
                }

//                Opacity = 20;
            }
        }

//        private double _opacity = 100;
/*
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (value != _opacity) return;
                _opacity = value;
                OnPropertyChanged(() => Opacity);
            }
        }
*/

        public IEnumerable<ISearchable> FoundObjects
        {
            get
            {
                if (Model == null)
                {
                    return null;
                }
                var foundObjects = !string.IsNullOrWhiteSpace(_findCriteria)
                    ? AllObjects.Where(o => o.ShortPath.ToLower().Contains(_findCriteria.ToLower()))
                    : AllObjects;
                return foundObjects;
            }
        }

        public string HighlightText => FindCriteria;

        /// <summary>
        ///     Критерий поиска объектов
        /// </summary>
        public string FindCriteria
        {
            get { return _findCriteria; }
            set
            {
                _findCriteria = value;
                OnPropertyChanged(() => FindCriteria);
                OnPropertyChanged(() => FoundObjects);
                OnPropertyChanged(() => HighlightText);
            }
        }

        private IEnumerable<ISearchable> AllObjects => Model.CompressorShops.Cast<ISearchable>()
            .Union(Model.DistributingStations)
            .Union(Model.MeasuringLines)
            .Union(Model.Valves)
            .Union(Model.PolyLines.Cast<ISearchable>())
            .Union(Model.CheckValves.Cast<ISearchable>())
            .Union(Model.ReducingStations)
            .Union(Model.Pipelines.Cast<ISearchable>());
    }
}