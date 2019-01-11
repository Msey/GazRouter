using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.Repairs.Workflow;

namespace GazRouter.DTO.Repairs.Plan
{
    [DataContract]
    public class RepairPlanBaseDTO : BaseDto<int>
    {
        public RepairPlanBaseDTO()
        {
            Works = new List<RepairWorkDTO>();
            //Complex = new ComplexDTO();
            WFWState = new WorkStateDTO();
        }

        /// <summary>
        /// Идентификатор объекта ремонта
        /// </summary>
        [DataMember]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Тип объекта ремонта
        /// </summary>
        [DataMember]
        public EntityType EntityType { get; set; }

        [DataMember]
        public int PlanType { get; set; }
        /// <summary>
        /// Наименование объекта ремонта
        /// </summary>
        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string SiteName { get; set; }

        /// <summary>
        /// Группа газопроводов (технологический корридор)
        /// </summary>
        [DataMember]
        public string PipelineGroupName { get; set; }

        /// <summary>
        /// Перечень работ
        /// </summary>
        [DataMember]
        public List<RepairWorkDTO> Works { get; set; }

        /// <summary>
        /// Вид ремонтных работ
        /// </summary>
        [DataMember]
        public int RepairTypeId { get; set; }

        /// <summary>
        /// Объем стравливаемого газа, млн.м3
        /// </summary>
        [DataMember]
        public double BleedAmount { get; set; }

        /// <summary>
        /// Объем выработанного газа, млн.м3
        /// </summary>
        [DataMember]
        public double SavingAmount { get; set; }

        /// <summary>
        /// Плановая дата начала работ
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Плановая дата окончания работ
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Примечания от ГТП
        /// </summary>
        [DataMember]
        public string DescriptionGtp { get; set; }

        /// <summary>
        /// Комментарий от ЦПДД
        /// </summary>
        [DataMember]
        public string DescriptionCpdd { get; set; }

        /// <summary>
        /// Достигнутый объем транспорта газа на участке - зима, млн.м3/сут
        /// </summary>
        [DataMember]
        public double MaxTransferWinter { get; set; }

        /// <summary>
        /// Достигнутый объем транспорта газа на участке - лето, млн.м3/сут
        /// </summary>
        [DataMember]
        public double MaxTransferSummer { get; set; }

        /// <summary>
        /// Достигнутый объем транспорта газа на участке - межсезонье, млн.м3/сут
        /// </summary>
        [DataMember]
        public double MaxTransferTransition { get; set; }

        /// <summary>
        /// Расчетная пропускная способность участка - зима, млн.м3/сут
        /// </summary>
        [DataMember]
        public double CapacityWinter { get; set; }

        /// <summary>
        /// Расчетная пропускная способность участка - лето, млн.м3/сут
        /// </summary>
        [DataMember]
        public double CapacitySummer { get; set; }

        /// <summary>
        /// Расчетная пропускная способность участка - межсезонье, млн.м3/сут
        /// </summary>
        [DataMember]
        public double CapacityTransition { get; set; }

        /// <summary>
        /// Расчетный объем транспорта газа на период проведения работ, млн.м3/сут
        /// </summary>
        [DataMember]
        public double CalculatedTransfer { get; set; }

        /// <summary>
        /// Признак влияния на транспорт газа
        /// </summary>
        [DataMember]
        public bool IsCritical { get; set; }

        /// <summary>
        /// Признак является ли работа внешним условием
        /// </summary>
        [DataMember]
        public bool IsExternalCondition { get; set; }

        /// <summary>
        /// Способ ведения работ
        /// </summary>
        [DataMember]
        public ExecutionMeans ExecutionMeans { get; set; }

        /// <summary>
        /// Дата поставки МТР
        /// </summary>
        [DataMember]
        public DateTime PartsDeliveryDate { get; set; }

        /// <summary>
        /// Комплекс
        /// </summary>
        [DataMember]
        public ComplexDTO Complex { get; set; }

        /// <summary>
        /// Пользователь последний раз изменивший запись
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Дата последнего изменения
        /// </summary>
        [DataMember]
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Подразделение пользователя последний раз изменившего запись
        /// </summary>
        [DataMember]
        public string UserSiteName { get; set; }

        [DataMember]
        public WorkStateDTO WFWState {get;set;}

        [DataMember]
        public FireworksDTO.FireTypes Firework { get; set; }
        [DataMember]
        public DateTime? DateStartFact { get; set; }
        [DataMember]
        public DateTime? DateEndFact { get; set; }
        [DataMember]
        public DateTime? DateStartSched { get; set; }
        [DataMember]
        public DateTime? DateEndSched { get; set; }
        [DataMember]
        public DateTime? ResolutionDate { get; set; }
        [DataMember]
        public DateTime? ResolutionDateCpdd { get; set; }
        [DataMember]
        public string ResolutionNum { get; set; }
        [DataMember]
        public string ResolutionNumCpdd { get; set; }
        [DataMember]
        public Guid SiteId { get; set; }

        [DataMember]
        public string AgreedUserName { get; set; }
        [DataMember]
        public int AgreedRecordID { get; set; }
        [DataMember]
        public DateTime? AgreedCreationDate { get; set; }
        [DataMember]
        public int AgreedUserID { get; set; }
        [DataMember]
        public int? Duration { get; set; }
        [DataMember]
        public string GazpromPlanID { get; set; }
        [DataMember]
        public DateTime? GazpromPlanDate { get; set; }
        [DataMember]
        public string ConsumersState { get; set; }
    }
}