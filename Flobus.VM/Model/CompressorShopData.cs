using GazRouter.DTO.Repairs.Plan;
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
using GazRouter.Controls.Measurings;

namespace GazRouter.Flobus.VM.Model
{
    public class CompressorShopData 
    {
        public CompressorShopData(CompressorShopMeasuring cs_measuring, RepairPlanBaseDTO repair = null)
        {
            RepairDto = repair;
            Measuring = cs_measuring;
        }

        public object Data
        {
            get
            {
                if (HasRepair && RepairDto != null)
                    return RepairDto;
                else
                    return Measuring;
            }
        }
        public CompressorShopMeasuring Measuring{ get; set; }
        public RepairPlanBaseDTO RepairDto { get; set; }
        /// <summary>
        /// Флаг ремонта
        /// </summary>
        public bool HasRepair { get; set; }
        
    }
}
