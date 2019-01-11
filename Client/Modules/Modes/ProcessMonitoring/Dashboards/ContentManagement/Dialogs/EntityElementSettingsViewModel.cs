using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs
{
    public class EntityElementSettingsViewModel : DialogViewModel
    {
        

        public DelegateCommand SaveCommand { get; private set; }

        public List<PropertyTypeDisplaySettings> PropertyTypeVisibilityList { get; private set; }
        

        private string _tableHead;
        public string TableHead
        {
            get { return _tableHead; }
            set
            {
                _tableHead = value;
                OnPropertyChanged(() => TableHead);
            }
        }

        private int _serieCount;
        public int SerieCount
        {
            get { return _serieCount; }
            set
            {
                _serieCount = value;
                OnPropertyChanged(() => SerieCount);
                
            }
        }

        private bool _isTimestampVisible;
        public bool IsTimestampVisible
        {
            get { return _isTimestampVisible; }
            set
            {
                _isTimestampVisible = value;
                OnPropertyChanged(() => IsTimestampVisible);
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

        private readonly EntityElementModel _model;

        
        
        public EntityElementSettingsViewModel(EntityElementModel model)
            : base(() => { })
        {
            _model = model;


            _serieCount = model.SerieCount;
            _fontSize = model.FontSize;
            _isTimestampVisible = model.IsTimestampVisible;
            _tableHead = model.EntityName;

            SaveCommand = new DelegateCommand(OnSave);

            PropertyTypeVisibilityList = ClientCache.DictionaryRepository.EntityTypes
                .Single(et => et.EntityType == model.EntityType).EntityProperties
                .Select(
                    pt =>
                        model.VisiblePropertyTypeList.Any(p => p.PropertyTypeId == pt.Id)
                            ? model.VisiblePropertyTypeList.Single(p => p.PropertyTypeId == pt.Id)
                            : new PropertyTypeDisplaySettings {PropertyTypeId = pt.Id}).ToList();
                
        }



        private void OnSave()
        {
            _model.SerieCount = _serieCount;
            _model.FontSize = _fontSize;
            _model.IsTimestampVisible = _isTimestampVisible;
            _model.EntityName = _tableHead;
            _model.VisiblePropertyTypeList = 
                PropertyTypeVisibilityList.Where(p => p.IsVisible).ToList();
            
            DialogResult = true;
        }
        
        
    }
}
