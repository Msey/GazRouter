using System;
using GazRouter.Controls.Measurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using JetBrains.Annotations;
using GazRouter.DTO.Repairs.Plan;
using System.Windows.Media;

namespace GazRouter.Flobus.VM.Model
{
    public class CompressorShopMeasuring
    {
        public CompressorShopMeasuring(CompShopDTO dto, [NotNull] CompressorShop compressorShop)
        {
            if (compressorShop == null)
            {
                throw new ArgumentNullException(nameof(compressorShop));
            }
            CompressorShop = compressorShop;
            PressureInlet = new DoubleMeasuring(dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, false, true);
            PressureOutlet = new DoubleMeasuring(dto.Id, PropertyType.PressureOutlet, PeriodType.Twohours, false, true);
            CompressionRatio = new DoubleMeasuring(dto.Id, PropertyType.CompressionRatio, PeriodType.Twohours, false, true);

            TemperatureInlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, false, true);
            TemperatureOutlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureOutlet, PeriodType.Twohours, false, true);
            TemperatureCooling = new DoubleMeasuring(dto.Id, PropertyType.TemperatureCooling, PeriodType.Twohours, false, true);

            FuelGasConsumption = new DoubleMeasuring(dto.Id, PropertyType.FuelGasConsumption, PeriodType.Twohours, false, true);
            Pumping = new DoubleMeasuring(dto.Id, PropertyType.Pumping, PeriodType.Twohours, false, true);

            Pattern = new StringMeasuring(dto.Id, PropertyType.CompressorShopPattern, PeriodType.Twohours);
        }

        public CompressorShopMeasuring(CompressorShopMeasuring meas, RepairPlanBaseDTO repair = null)
        {
            CompressorShop = meas.CompressorShop;
            PressureInlet = meas.PressureInlet;
            PressureOutlet = meas.PressureOutlet;
            CompressionRatio = meas.CompressionRatio;
            TemperatureInlet = meas.TemperatureInlet;
            TemperatureOutlet = meas.TemperatureOutlet;
            TemperatureCooling = meas.TemperatureCooling;
            FuelGasConsumption = meas.FuelGasConsumption;
            Pumping = meas.Pumping;
            Pattern = meas.Pattern;
            if(repair!=null) RepairDto = repair;
        }

        public CompressorShop CompressorShop { get; }

        /// <summary>
        /// Давление газа на входе цеха
        /// </summary>
        public DoubleMeasuring PressureInlet { get; }

        /// <summary>
        /// Давление газа на выходе цеха
        /// </summary>
        public DoubleMeasuring PressureOutlet { get; }

        /// <summary>
        /// Степень сжатия
        /// </summary>
        public DoubleMeasuring CompressionRatio { get; }

        /// <summary>
        /// Температура газа на входе цеха
        /// </summary>
        public DoubleMeasuring TemperatureInlet { get; }

        /// <summary>
        /// Температура газа на выходе цеха
        /// </summary>
        public DoubleMeasuring TemperatureOutlet { get; }

        /// <summary>
        /// Температура газа после АВО
        /// </summary>
        public DoubleMeasuring TemperatureCooling { get; }

        /// <summary>
        /// Расход топливного газа
        /// </summary>
        public DoubleMeasuring FuelGasConsumption { get; }

        /// <summary>
        /// Объем газа, перекачиваемый КЦ
        /// </summary>
        public DoubleMeasuring Pumping { get; }

        public StringMeasuring Pattern { get; }

        public string RepairPeriod => HasRepair ? string.Format("{0} - {1}", RepairDto.StartDate.Date.ToString("d"), RepairDto.EndDate.Date.ToString("d")) : string.Empty;

        public bool IsFact => HasRepair ? RepairDto.StartDate <= DateTime.Now && RepairDto.EndDate >= DateTime.Now : false;

        public Brush FactColor => IsFact ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Transparent);

        public RepairPlanBaseDTO RepairDto { get; set; }
        /// <summary>
        /// Флаг ремонта
        /// </summary>
        public bool HasRepair => RepairDto != null;

    }
}