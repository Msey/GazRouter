using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.ObjectModel.Model.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GazRouter.ObjectModel.DeviceConfig
{
    public class AddBoilerTypeViewModel : AddEditViewModelBase<BoilerTypeDTO, int>
    {
        private BoilerTypeDTO _BoilerType;

        private string _DtoName;
        private double _RatedHeatingEfficiency;
        private double _RatedEfficiencyFactor;
        private string _GroupName;
        private bool _IsSmall;
        private double _HeatingArea;
        private string _Description;

        public AddBoilerTypeViewModel(Action<int> actionBeforeClosing) : base(actionBeforeClosing)
        {
        }

        public AddBoilerTypeViewModel(Action<int> actionBeforeClosing, BoilerTypeDTO dto)
            : base(actionBeforeClosing, dto)
        {
            this._BoilerType = dto;

            this._Description = dto.Description;
            this._DtoName = dto.Name;
            this._RatedHeatingEfficiency = dto.RatedHeatingEfficiency;
            this._RatedEfficiencyFactor = dto.RatedEfficiencyFactor;
            this._GroupName = dto.GroupName;
            this._IsSmall = dto.IsSmall;
            this._HeatingArea = dto.HeatingArea;
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

        public double RatedHeatingEfficiency
        {
            get { return _RatedHeatingEfficiency; }
            set
            {
                _RatedHeatingEfficiency = value;
                OnPropertyChanged(() => RatedHeatingEfficiency);
            }
        }

        public double RatedEfficiencyFactor
        {
            get { return _RatedEfficiencyFactor; }
            set
            {
                _RatedEfficiencyFactor = value;
                OnPropertyChanged(() => RatedEfficiencyFactor);
            }
        }

        public double HeatingArea
        {
            get { return _HeatingArea; }
            set
            {
                _HeatingArea = value;
                OnPropertyChanged(() => HeatingArea);
            }
        }


        /// <summary>
        /// Список групп котлов
        /// </summary>
        public List<string> BoilerGroupList => new List<string>(){"Бытовые подогреватели", "Прочие"};

        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                _GroupName = value;
                OnPropertyChanged(() => GroupName);
            }
        }
        public bool IsSmall
        {
            get { return _IsSmall; }
            set
            {
                _IsSmall = value;
                OnPropertyChanged(() => IsSmall);
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
                var paramSet = new EditBoilerTypeParameterSet
                {
                    Id = Model.Id,
                    Name = _DtoName,
                    Description = _Description,
                    GroupName = _GroupName,
                    IsSmall = _IsSmall,
                    HeatingArea = _HeatingArea,
                    RatedEfficiencyFactor = _RatedEfficiencyFactor,
                    RatedHeatingEfficiency = _RatedHeatingEfficiency,
                };
                return new ObjectModelServiceProxy().EditBoilerTypeAsync(
                    paramSet);
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var paramSet = new AddBoilerTypeParameterSet
                {
                    Name = _DtoName,
                    Description = _Description,
                    GroupName = _GroupName,
                    IsSmall = _IsSmall,
                    HeatingArea = _HeatingArea,
                    RatedEfficiencyFactor = _RatedEfficiencyFactor,
                    RatedHeatingEfficiency = _RatedHeatingEfficiency,
                };
                return new ObjectModelServiceProxy().AddBoilerTypeAsync(
                    paramSet);
            }
        }

        protected override string CaptionEntityTypeName => "типа котла";

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
        }

    }
}
