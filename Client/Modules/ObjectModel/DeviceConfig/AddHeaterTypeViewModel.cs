using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.HeaterTypes;
using GazRouter.DTO.ObjectModel.Heaters;
using GazRouter.ObjectModel.Model.Dialogs;
using System;
using System.Threading.Tasks;

namespace GazRouter.ObjectModel.DeviceConfig
{
    public class AddHeaterTypeViewModel : AddEditViewModelBase<HeaterTypeDTO, int>
    {
        private HeaterTypeDTO _Dto;

        private string _DtoName;
        private double? _EfficiencyFactorRated;
        private double _GasConsumptionRate;
        private string _Description;

        public AddHeaterTypeViewModel(Action<int> actionBeforeClosing) : base(actionBeforeClosing)
        {
        }

        public AddHeaterTypeViewModel(Action<int> actionBeforeClosing, HeaterTypeDTO dto)
            : base(actionBeforeClosing, dto)
        {
            this._Dto = dto;

            this._Description = dto.Description;
            this._DtoName = dto.Name;
            this._EfficiencyFactorRated = dto.EffeciencyFactorRated;
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

        public double? EfficiencyFactorRated
        {
            get { return _EfficiencyFactorRated; }
            set
            {
                _EfficiencyFactorRated = value;
                OnPropertyChanged(() => EfficiencyFactorRated);
            }
        }

        public double GasConsumptionRate
        {
            get { return _GasConsumptionRate; }
            set
            {
                _GasConsumptionRate = value;
                OnPropertyChanged(() => _GasConsumptionRate);
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
                var paramSet = new EditHeaterTypeParameterSet
                {
                    Id = Model.Id,
                    Name = _DtoName,
                    Description = _Description,
                    EfficiencyFactorRated = _EfficiencyFactorRated,
                    GasConsumptionRate = _GasConsumptionRate,
                };
                return new ObjectModelServiceProxy().EditHeaterTypeAsync(paramSet);
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var paramSet = new AddHeaterTypeParameterSet
                {
                    Name = _DtoName,
                    Description = _Description,
                    EfficiencyFactorRated = _EfficiencyFactorRated,
                    GasConsumptionRate = _GasConsumptionRate,
                };
                return new ObjectModelServiceProxy().AddHeaterTypeAsync(paramSet);
            }
        }

        protected override string CaptionEntityTypeName => "типа подогревателя газа";

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
    }
}
