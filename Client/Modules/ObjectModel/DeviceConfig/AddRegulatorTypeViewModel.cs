using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.RegulatorTypes;
using GazRouter.DTO.ObjectModel.Regulators;
using GazRouter.ObjectModel.Model.Dialogs;
using System;
using System.Threading.Tasks;

namespace GazRouter.ObjectModel.DeviceConfig
{
    public class AddRegulatorTypeViewModel : AddEditViewModelBase<RegulatorTypeDTO, int>
    {
        private RegulatorTypeDTO _Dto;

        private string _DtoName;
        private double _GasConsumptionRate;
        private string _Description;

        public AddRegulatorTypeViewModel(Action<int> actionBeforeClosing) : base(actionBeforeClosing)
        {
        }

        public AddRegulatorTypeViewModel(Action<int> actionBeforeClosing, RegulatorTypeDTO dto)
            : base(actionBeforeClosing, dto)
        {
            this._Dto = dto;

            this._Description = dto.Description;
            this._DtoName = dto.Name;
            this._GasConsumptionRate = dto.GasConsumptionRate;
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

        public double GasConsumptionRate
        {
            get { return _GasConsumptionRate; }
            set
            {
                _GasConsumptionRate = value;
                OnPropertyChanged(() => GasConsumptionRate);
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
                var paramSet = new EditRegulatorTypeParameterSet
                {
                    Id = Model.Id,
                    Name = _DtoName,
                    Description = _Description,
                    GasConsumptionRate = _GasConsumptionRate,
                };
                return new ObjectModelServiceProxy().EditRegulatorTypeAsync(paramSet);
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var paramSet = new AddRegulatorTypeParameterSet
                {
                    Name = _DtoName,
                    Description = _Description,
                    GasConsumptionRate = _GasConsumptionRate,
                };
                return new ObjectModelServiceProxy().AddRegulatorTypeAsync(paramSet);
            }
        }

        protected override string CaptionEntityTypeName => "типа кранов-регуляторов";

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
    }
}
