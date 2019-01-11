using System;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
namespace GazRouter.Modes.GasCosts.DefaultDataDialog
{
    public class DefaultParamValues
    {
        private readonly DefaultParamValuesDTO _dto;
        public DefaultParamValues(DefaultParamValuesDTO dto)
        {
            _dto = dto;
        }
        public DefaultParamValuesDTO Dto
        {
            get { return _dto; }
        }
        public Target Target
        {
            get { return _dto.Target; }
        }
        public double PressureAir
        {
            get { return _dto.PressureAir; }
            set
            {
                if (value < 700 || value > 800)
                    throw new Exception("Недопустимое значение. Должно быть в диапазоне (700 - 800)");
                _dto.PressureAir = value;
            }
        }
        public double Density
        {
            get { return _dto.Density; }
            set
            {
                if (value < 0.5 || value > 1)
                    throw new Exception("Недопустимое значение. Должно быть в диапазоне (0.5 - 1)");
                _dto.Density = value;
            }
        }
        public double CombHeat
        {
            get { return _dto.CombHeat; }
            set
            {
                if (value < 7000 || value > 9500)
                    throw new Exception("Недопустимое значение. Должно быть в диапазоне (7000 - 9500)");
                _dto.CombHeat = value;
            }
        }
        public double NitrogenContent
        {
            get { return _dto.NitrogenContent; }
            set
            {
                if (value < 0 || value > 10)
                    throw new Exception("Недопустимое значение. Должно быть в диапазоне (0 - 10)");
                _dto.NitrogenContent = value;
            }
        }
        public double CarbonDioxideContent
        {
            get { return _dto.CarbonDioxideContent; }
            set
            {
                if (value < 0 || value > 10)
                    throw new Exception("Недопустимое значение. Должно быть в диапазоне (0 - 10");
                _dto.CarbonDioxideContent = value;
            }
        }
	}
}