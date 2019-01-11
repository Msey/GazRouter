using System;
using System.Collections.Generic;
using GazRouter.Controls.Trends;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataLoadMonitoring;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ObjectModel;
using GazRouter.ObjectModel.Model.Dialogs.Series;
using Microsoft.Practices.Prism.Commands;
using SeriesView = GazRouter.ObjectModel.Model.Dialogs.Series.SeriesView;

namespace GazRouter.Modes.ProcessMonitoring.Module
{
    public class CompressorShopValues : CompressorShopValuesChangeDTO
    {
       
        public CompressorShopValues()
        {}

        public CompressorShopValues(CompressorShopValuesChangeDTO values, GasModeChangeConditions cond)
        {
            SiteId = values.SiteId;
            SiteName = values.SiteName;
            CompStationId = values.CompStationId;
            CompStationName = values.CompStationName;

            Valve20 = new DTO.DataLoadMonitoring.Valve
            {
                ValveId = values.Valve20.ValveId,
                ValveName = values.Valve20.ValveName,
                ValveState = values.Valve20.ValveState,
                StateByPass1 = values.Valve20.StateByPass1,
                StateByPass2 = values.Valve20.StateByPass2,
                StateByPass3 = values.Valve20.StateByPass3,
                Valve20PressureInlet = getValueAndColor(values.Valve20.Valve20PressureInlet, cond.PressureChangeAlaram),
                Valve20PressureOutlet = getValueAndColor(values.Valve20.Valve20PressureOutlet, cond.PressureChangeAlaram),
                Valve20TemperatureInlet = getValueAndColor(values.Valve20.Valve20TemperatureInlet, cond.TemperatureChangeLimit),
                Valve20TemperatureOutlet = getValueAndColor(values.Valve20.Valve20TemperatureOutlet, cond.TemperatureChangeLimit),

            };
            Valve20.SetValveColor();
            CompShopId = values.CompShopId;
            CompShopName = values.CompShopName;

            CompShopPressureInlet = getValueAndColor(values.CompShopPressureInlet, cond.PressureChangeAlaram);
            CompShopPressureOutlet = getValueAndColor(values.CompShopPressureOutlet, cond.PressureChangeAlaram);
            CompShopScheme = values.CompShopScheme;
            CompShopTemperatureCooling = getValueAndColor(values.CompShopTemperatureCooling, cond.TemperatureChangeLimit);
            CompShopTemperatureInlet = getValueAndColor(values.CompShopTemperatureInlet, cond.TemperatureChangeLimit);
            CompShopTemperatureOutlet = getValueAndColor(values.CompShopTemperatureOutlet, cond.TemperatureChangeLimit);

			AddToTrendCommand = new DelegateCommand<object>(AddToTrendBoard, par => true);
			ViewDataCommand = new DelegateCommand<object>(ViewData, par => true);
            GetShopInfo();
        }

        private async void GetShopInfo()
        {
            _dto = await new ObjectModelServiceProxy().GetEntityByIdAsync(CompShopId);
        }


        ChangeModeValueDouble getValueAndColor( ChangeModeValueDouble inputValue, double lim)
        {
            var value = inputValue;
            if (inputValue.PreviousValue.HasValue == false || inputValue.Value.HasValue == false)
            {
                return value;
            }
            var absVal = Math.Abs(inputValue.PreviousValue.Value - inputValue.Value.Value);
           
            
            if (absVal >= lim)
            {
                value.Color = "#FFF00808";
            }
            value.CalcDiff();
            return value;
        }

      
		private void ViewData(object parameter)
		{
			if (parameter == null || _dto == null) return;
			var prop = (PropertyType)Enum.Parse(typeof(PropertyType), parameter.ToString(), true);
			var vm = new SeriesViewModel(() => { }, _dto, prop);
			var dialog = new SeriesView { DataContext = vm };
			dialog.ShowDialog();
		}

        

		public DelegateCommand<object> AddToTrendCommand{get; private set; }
		public DelegateCommand<object> ViewDataCommand { get; private set; }
	    private CommonEntityDTO _dto;

		private void AddToTrendBoard(object parameter)
		{
			if (parameter == null || _dto==null) return;
			var prop = (PropertyType)Enum.Parse(typeof(PropertyType),parameter.ToString(),true);
			TrendsHelper.ShowTrends(_dto, prop);
		}
    }

    public class CompressorShopChangeTree 
    {
        #region name - наименование в дереве
        public string Name { get; set; }
        public bool IsShopElem { get; set; }
        #endregion
        #region isExpanded
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
              
            }
        }
        #endregion
        #region значение расхода газа дереве
        private CompressorShopValues _value;
        public CompressorShopValues Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion
        public List<CompressorShopChangeTree> Items { get; set; }
        public CompressorShopChangeTree(string name, CompressorShopValues val, bool isExpanded = true, bool isShopElem = false)
        {
            Name = name;
            Value = val;
            IsExpanded = isExpanded;
            IsShopElem = isShopElem;
            Items = new List<CompressorShopChangeTree>();
            
        }

        public override string ToString()
        {
            return Name;
        }

       
    }

    public class Valve
    {
        public Guid ValveId { get; set; }
        public string ValveName { get; set; }
        public ValveState ValveState { get; set; }
        public ChangeModeValueDouble Valve20PressureInlet { get; set; }
        public ChangeModeValueDouble Valve20PressureOutlet { get; set; }
        public ChangeModeValueDouble Valve20TemperatureInlet { get; set; }
        public ChangeModeValueDouble Valve20TemperatureOutlet { get; set; }
    }

}
