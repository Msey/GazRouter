using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Flobus.Services;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GazRouter.Flobus.VM.Model
{
    public class RepairWorks : IRepairWorks
    {
        public RepairWorks(RepairPlanBaseDTO dto)
        {
            Dto = dto;
        }


        public RepairPlanBaseDTO Dto { get; internal set; }

        /// <summary>
        ///     Идентификатор ремонтной работы
        /// </summary>
        public int Id => Dto.Id;


        /// <summary>
        ///     Наименование объекта
        /// </summary>
        public string ObjectName
        {
            get
            {
                switch (Dto.EntityType)
                {
                    case EntityType.Pipeline:
                        return
                            $"Участок \r\n{(Dto.Works.Count > 0 ? Dto.Works.Min(w => w.KilometerStart)?.ToString("0.#") : "0")} - {(Dto.Works.Count > 0 ? Dto.Works.Max(w => w.KilometerEnd)?.ToString("0.#") : "0")} км.";
                    case EntityType.CompShop:
                        return
                            $"{((RepairPlanCompShopDTO)Dto).Kilometer} км,  \r\n{Dto.EntityName.Replace("/", ", \r\n")}";

                    default:
                        return Dto.EntityName;
                }
            }
        }

        public string PipelineName
        {
            get
            {
                switch (Dto.EntityType)
                {
                    case EntityType.CompShop:
                        return ((RepairPlanCompShopDTO)Dto).PipelineName;

                    case EntityType.DistrStation:
                        return "ГРС";

                    default:
                        return Dto.EntityName;
                }
            }
        }
        
        public bool IsPipeline => Dto.EntityType == EntityType.Pipeline;

        /// <summary>
        ///     Продолжительность (плановая), часов
        /// </summary>
        public TimeSpan DurationPlan => Dto.EndDate - Dto.StartDate;

        /// <summary>
        /// Название месяца даты начала работ
        /// </summary>
        public string StartDateMonthName => Dto.StartDate.ToString("MMMM");
        
        public string WorkflowState
        {
            get
            {
                return Dto.WFWState.WFState == DTO.Repairs.Workflow.WorkStateDTO.WorkflowStates.Undefined ? "" : DTO.Repairs.Workflow.WorkStateDTO.GetState(Dto.WFWState.WFState);
            }
        }

        public string RepairState
        {
            get
            {
                return Dto.WFWState.WState == DTO.Repairs.Workflow.WorkStateDTO.WorkStates.Undefined ? "" : DTO.Repairs.Workflow.WorkStateDTO.GetState(Dto.WFWState.WState);
            }
        }

    }
}
