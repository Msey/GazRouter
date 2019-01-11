using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
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

namespace GazRouter.Controls.RepairWorks
{
    public class RepairBase : PropertyChangedBase, IComparable
    {
        public virtual int CompareTo(object obj)
        {
            return -1;
        }
    }


    public class ExternalCondition : RepairBase
    {

    }


    public class RepairWork : RepairBase
    {

        public static RepairWork Create(RepairPlanBaseDTO dto)
        {
            if (dto.EntityType == EntityType.Pipeline)
                return new PipelineRepair { Dto = dto };
            return new RepairWork { Dto = dto };
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

        /// <summary>
        ///     Порядок сортировки
        /// </summary>
        public double SortOrder
        {
            get
            {
                try
                {
                    switch (Dto.EntityType)
                    {
                        case EntityType.Pipeline:
                            return Dto.Works.Count > 0 ? Dto.Works.Where(w => w.KilometerStart.HasValue).Min(w => w.KilometerStart.Value) : 0;

                        case EntityType.CompShop:
                            return ((RepairPlanCompShopDTO)Dto).Kilometer;

                        default:
                            return Dto.StartDate.DayOfYear;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }


        /// <summary>
        ///     Вид ремонтных работ
        /// </summary>
        public string RepairTypeName
            => ClientCache.DictionaryRepository.RepairTypes.Single(rt => rt.Id == Dto.RepairTypeId).Name;


        /// <summary>
        ///     Способ ведения работ
        /// </summary>
        public string ExecutionMeansName
            =>
                ClientCache.DictionaryRepository.RepairExecutionMeans.Single(
                    em => em.ExecutionMeans == Dto.ExecutionMeans).Name;

        public bool IsPipeline => Dto.EntityType == EntityType.Pipeline;

        /// <summary>
        ///     Дата начала работ (плановая)
        /// </summary>
        //public DateTime StartDatePlan
        //{
        //    get { return Dto.StartDate; }
        //    set
        //    {
        //        Dto.StartDate = value;
        //        OnPropertyChanged(()=>StartDatePlan);
        //    }
        //}

        /// <summary>
        ///     Дата окончания работ (плановая)
        /// </summary>
        //public DateTime EndDatePlan
        //{
        //    get { return Dto.EndDate; }
        //    set
        //    {
        //        Dto.EndDate = value;
        //        OnPropertyChanged(() => EndDatePlan);
        //    }
        //}

        /// <summary>
        ///     Продолжительность (плановая), часов
        /// </summary>
        public TimeSpan DurationPlan => Dto.EndDate - Dto.StartDate;

        /// <summary>
        /// Название месяца даты начала работ
        /// </summary>
        public string StartDateMonthName => Dto.StartDate.ToString("MMMM");



        /// <summary>
        /// Ремонт включен в комплекс
        /// </summary>
        public bool HasComplex => Dto.Complex != null;


        /// <summary>
        ///     Признак ошибок, связанных с включением работы в комплекс
        /// </summary>
        public bool HasComplexError => !string.IsNullOrEmpty(ComplexErrorString);

        /// <summary>
        ///     Текстовое описание ошибки, связанной с включением работы в комплекс
        /// </summary>
        public string ComplexErrorString
        {
            get
            {
                if (Dto.Complex == null)
                    return string.Empty;

                if (Dto.StartDate < Dto.Complex.StartDate)
                    return "Дата начала ремонта меньше даты начала комплекса";

                if (Dto.EndDate > Dto.Complex.EndDate)
                    return "Дата окончания ремонта больше даты окончания комплекса";

                return string.Empty;
            }
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public override int CompareTo(object obj)
        {
            var other = obj as RepairWork;
            if (other == null)
            {
                return 1;
            }


            if (SortOrder > other.SortOrder)
            {
                return 1;
            }
            if (SortOrder == other.SortOrder && Dto.EntityType == EntityType.CompShop)
            {
                return 1;
            }
            return -1;
        }

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


    public class PipelineRepair : RepairWork { }


    public class Complex
    {
        public Complex(ComplexDTO dto, Action<Complex> addTo)
        {
            Dto = dto;
            HasErrors = true;
            AddToThisComplexCommand = new DelegateCommand(() => addTo(this));
        }

        public ComplexDTO Dto { get; }

        public TimeSpan Duration => Dto.EndDate - Dto.StartDate;

        public bool HasErrors { get; set; }

        public DelegateCommand AddToThisComplexCommand { get; set; }
    }
}
