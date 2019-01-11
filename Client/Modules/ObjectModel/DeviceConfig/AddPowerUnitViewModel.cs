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
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using System.Collections.Generic;
using GazRouter.ObjectModel.Model.Dialogs;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.PowerUnits;

namespace GazRouter.ObjectModel.DeviceConfig
{
    public class AddPowerUnitViewModel : AddEditViewModelBase<PowerUnitTypeDTO, int>
    {
        private PowerUnitTypeDTO _powerUnitType;

        private EngineGroup _engineGroup;
        private string _DtoName;
        private double _ratedPower;
        private double _FuelConsumptionRate;
        private string _EngineTypeName;
        private string _Description;

        public AddPowerUnitViewModel(Action<int> actionBeforeClosing) : base(actionBeforeClosing)
        {
        }

        public AddPowerUnitViewModel(Action<int> actionBeforeClosing, PowerUnitTypeDTO powerUnitDto)
            : base(actionBeforeClosing, powerUnitDto)
        {
            this._powerUnitType = powerUnitDto;

            this._Description = powerUnitDto.Description;
            this._DtoName = powerUnitDto.Name;
            this._engineGroup = powerUnitDto.EngineGroup;
            this._EngineTypeName = powerUnitDto.EngineTypeName;
            this._FuelConsumptionRate = powerUnitDto.FuelConsumptionRate;
            this._ratedPower = powerUnitDto.RatedPower;
        }

        /// <summary>
        /// Список типов двигателей
        /// </summary>
        public Dictionary<string, EngineGroup> EngineGroupList => new Dictionary<string, EngineGroup>
        {
            {"Газотурбинный", EngineGroup.Turbine},
            {"Газопоршневой", EngineGroup.Reciprocating}
        };

        /// <summary>
        /// Выбранный тип двигателя
        /// </summary>
        public EngineGroup EngineGroup
        {
            get { return _engineGroup; }
            set
            {
                _engineGroup = value;
                OnPropertyChanged(() => EngineGroup);
            }
        }

        public string DtoName
        {
            get { return _DtoName; }
            set
            {
                _DtoName = value;
                OnPropertyChanged(() => DtoName);
            }
        }

        public double RatedPower
        {
            get { return _ratedPower; }
            set
            {
                _ratedPower = value;
                OnPropertyChanged(() => RatedPower);
            }
        }

        public double FuelConsumptionRate
        {
            get { return _FuelConsumptionRate; }
            set
            {
                _FuelConsumptionRate = value;
                OnPropertyChanged(() => FuelConsumptionRate);
            }
        }

        public string EngineTypeName
        {
            get { return _EngineTypeName; }
            set
            {
                _EngineTypeName = value;
                OnPropertyChanged(() => EngineTypeName);
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged(() => Description);
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                var paramSet = new EditPowerUnitTypeParameterSet
                {
                    Id = Model.Id,
                    Name = _DtoName,
                    Description = _Description,
                    EngineGroup = _engineGroup,
                    EngineTypeName = _EngineTypeName,
                    FuelConsumptionRate = _FuelConsumptionRate,
                    RatedPower = _ratedPower,
                };
                return new ObjectModelServiceProxy().EditPowerUnitTypeAsync(
                    paramSet);
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var paramSet = new AddPowerUnitTypeParameterSet
                {
                    Name = _DtoName,
                    Description = _Description,
                    EngineGroup = _engineGroup,
                    EngineTypeName = _EngineTypeName,
                    FuelConsumptionRate = _FuelConsumptionRate,
                    RatedPower = _ratedPower,
                };
                return new ObjectModelServiceProxy().AddPowerUnitTypeAsync(
                    paramSet);
            }
        }

        protected override string CaptionEntityTypeName => "типа электроагрегата";

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
    }
}
