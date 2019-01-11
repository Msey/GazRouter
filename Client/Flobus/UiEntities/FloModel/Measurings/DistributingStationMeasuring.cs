using GazRouter.Common.ViewModel;
using GazRouter.Controls.Measurings;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.Repairs.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace GazRouter.Flobus.UiEntities.FloModel.Measurings
{
    public class DistributingStationMeasuring
    {
        public DistributingStationMeasuring(DistrStationDTO dto)
        {
            Dto = dto;
            PressureInlet = new DoubleMeasuring(dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, false, true);
            TemperatureInlet = new DoubleMeasuring(dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, false, true);
            Flow = new DoubleMeasuring(dto.Id, PropertyType.Flow, PeriodType.Twohours, false, true);
            foreach(var outlet in dto.Outlets)
                PressureOutlets.Add(new DistributingStationOutletMeasuring(outlet));

        }
        public DistributingStationMeasuring(DistributingStationMeasuring meas = null, RepairPlanBaseDTO repair = null)
        {
            if (meas != null)
            {
                Dto = meas.Dto;
                PressureInlet = meas.PressureInlet;
                TemperatureInlet = meas.TemperatureInlet;
                PressureOutlets = meas.PressureOutlets;
                Flow = meas.Flow;
            }
            if (repair != null)
                RepairDto = repair;
        }

        public DistrStationDTO Dto { get; set; }
        /// <summary>
        /// Давление газа на входе станции
        /// </summary>
        public DoubleMeasuring PressureInlet { get; set; }

        /// <summary>
        /// Температура газа на входе станции
        /// </summary>
        public DoubleMeasuring TemperatureInlet { get; set; }

        /// <summary>
        /// Расход газа
        /// </summary>
        public DoubleMeasuring Flow { get; set; }

        /// <summary>
        /// Давления газа на выходах станции
        /// </summary>
        public List<DistributingStationOutletMeasuring> PressureOutlets = new List<DistributingStationOutletMeasuring>();

        public List<DistributingStationOutletMeasuring> List => PressureOutlets.OrderBy(o => o.Name).ToList();

        public string RepairPeriod => HasRepair ? string.Format("{0} - {1}", RepairDto.StartDate.Date.ToString("d"), RepairDto.EndDate.Date.ToString("d")) : string.Empty;

        public bool IsFact => HasRepair ? RepairDto.StartDate <= DateTime.Now && RepairDto.EndDate >= DateTime.Now : false;

        public Brush FactColor => IsFact ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Transparent);

        public RepairPlanBaseDTO RepairDto { get; set; }
        /// <summary>
        /// Флаг ремонта
        /// </summary>
        public bool HasRepair => RepairDto != null;
    }


    public class DistributingStationOutletMeasuring
    {
        public DistributingStationOutletMeasuring(DistrStationOutletDTO dto)
        {
            Dto = dto;
            PressureOutlet = new DoubleMeasuring(dto.Id, PropertyType.PressureOutlet, PeriodType.Twohours, false, true);
        }
        public DistrStationOutletDTO Dto { get; set; }
        public string Name => Dto.Name;

        public DoubleMeasuring PressureOutlet { get; set; }
    }
}