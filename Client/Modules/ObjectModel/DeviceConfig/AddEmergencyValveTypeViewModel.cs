using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EmergencyValveTypes;
using GazRouter.DTO.Dictionaries.RegulatorTypes;
using GazRouter.DTO.ObjectModel.EmergencyValves;
using GazRouter.DTO.ObjectModel.Regulators;
using GazRouter.ObjectModel.Model.Dialogs;
using System;
using System.Threading.Tasks;

namespace GazRouter.ObjectModel.DeviceConfig
{
    public class AddEmergencyValveTypeViewModel : AddEditViewModelBase<EmergencyValveTypeDTO, int>
    {
        private EmergencyValveTypeDTO _Dto;

        private string _DtoName;
        private double _InnerDiameter;
        private string _Description;

        public AddEmergencyValveTypeViewModel(Action<int> actionBeforeClosing) : base(actionBeforeClosing)
        {
        }

        public AddEmergencyValveTypeViewModel(Action<int> actionBeforeClosing, EmergencyValveTypeDTO dto)
            : base(actionBeforeClosing, dto)
        {
            this._Dto = dto;

            this._Description = dto.Description;
            this._DtoName = dto.Name;
            this._InnerDiameter = dto.InnerDiameter;
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

        public double InnerDiameter
        {
            get { return _InnerDiameter; }
            set
            {
                _InnerDiameter = value;
                OnPropertyChanged(() => InnerDiameter);
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
                var paramSet = new EditEmergencyValveTypeParameterSet
                {
                    Id = Model.Id,
                    Name = _DtoName,
                    Description = _Description,
                    InnerDiameter = _InnerDiameter,
                };
                return new ObjectModelServiceProxy().EditEmergencyValveTypeAsync(paramSet);
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var paramSet = new AddEmergencyValveTypeParameterSet
                {
                    Name = _DtoName,
                    Description = _Description,
                    InnerDiameter = _InnerDiameter,
                };
                return new ObjectModelServiceProxy().AddEmergencyValveTypeAsync(paramSet);
            }
        }

        protected override string CaptionEntityTypeName => "типа предохранительных клапанов";

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }
    }
}
