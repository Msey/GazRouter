using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Enums;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Repair
{
    public class RepairItem : PropertyChangedBase, IComparable
    {
        public RepairItem()
        {
            Dto = new RepairPlanBaseDTO();
        }

        public RepairItem(RepairPlanBaseDTO repairDto, RepairGroupingType grType)
        {
            Dto = repairDto;
            GroupingType = grType;
        }

        /// <summary>
        ///     Идентификатор ремонтной работы
        /// </summary>
        public int Id => Dto.Id;

        public RepairPlanBaseDTO Dto { get; }

        public RepairGroupingType GroupingType { get; set; }

        /// <summary>
        ///     Признак, является ли запись внешним условием для планирования
        /// </summary>
        public bool IsExternalCondition => Dto.IsExternalCondition;

        /// <summary>
        ///     Работы
        /// </summary>
        public string RepairWorks
        {
            get
            {
                var result = string.Empty;

                // Для газопроводов
                if (Dto.EntityType == EntityType.Pipeline)
                {
                    foreach (var rw in Dto.Works)
                    {
                        if (result != string.Empty)
                        {
                            result += "," + Environment.NewLine;
                        }
                        result += rw.KilometerStart?.ToString("0.#") + " - " + rw.KilometerEnd?.ToString("0.#") + " - " +
                                  rw.WorkTypeName;
                    }
                }
                else
                {
                    // Для остальных типов объектов (километр начала и конца не указывается)
                    foreach (var rw in Dto.Works)
                    {
                        if (result != string.Empty)
                        {
                            result += "," + Environment.NewLine;
                        }
                        result += rw.WorkTypeName;
                    }
                }

                return result;
            }
        }

        /// <summary>
        ///     Наименование объекта
        /// </summary>
        public string ObjectName
        {
            get
            {
                var pref = GroupingType == RepairGroupingType.ByPipeline
                    ? string.Empty
                    : DefaultGroupName + Environment.NewLine;

                switch (Dto.EntityType)
                {
                    case EntityType.Pipeline:
                        return
                            $"{pref}Участок \r\n{(Dto.Works.Count > 0 ? Dto.Works.Min(w => w.KilometerStart)?.ToString("0.#") : "0")} - {(Dto.Works.Count > 0 ? Dto.Works.Max(w => w.KilometerEnd)?.ToString("0.#") : "0")} км.";
                    case EntityType.CompShop:
                        return
                            $"{pref}{((RepairPlanCompShopDTO) Dto).Kilometer} км,  \r\n{Dto.EntityName.Replace("/", ", \r\n")}";

                    default:
                        return Dto.EntityName;
                }
            }
        }

        public string DefaultGroupName
        {
            get
            {
                if (IsExternalCondition)
                {
                    return "Внешние условия";
                }

                switch (Dto.EntityType)
                {
                    case EntityType.Pipeline:
                        return Dto.EntityName;

                    case EntityType.CompShop:
                        return ((RepairPlanCompShopDTO) Dto).PipelineName;

                    case EntityType.DistrStation:
                        return "ГРСы";

                    default:
                        return Dto.EntityName;
                }
            }
        }

        /// <summary>
        ///     Группирующий объект
        /// </summary>
        public string GroupObject
        {
            get
            {
                switch (GroupingType)
                {
                    case RepairGroupingType.ByPipeline:
                        return DefaultGroupName;

                    case RepairGroupingType.BySite:
                        return SiteName;

                    case RepairGroupingType.ByComplex:
                        return string.IsNullOrEmpty(ComplexName) ? "Без комплекса" : ComplexName;

                    case RepairGroupingType.ByRepairType:
                        return string.IsNullOrEmpty(RepairTypeName) ? "Прочее" : RepairTypeName;

                    case RepairGroupingType.ByExecutionMeans:
                        return ExecutionMeansName;

                    case RepairGroupingType.ByPipelineGroup:
                        return string.IsNullOrEmpty(PipelineGroupName) ? "Прочие" : PipelineGroupName;

                    case RepairGroupingType.ByMonth:
                        return StartDatePlan.ToString("MMMM");

                    default:
                        return DefaultGroupName;
                }
            }
        }

        /// <summary>
        ///     Технологический корридор
        /// </summary>
        public string PipelineGroupName => Dto.PipelineGroupName;

        /// <summary>
        ///     Идентификатор комлекса
        /// </summary>
        public int? ComplexId => Dto.Complex?.Id;

        /// <summary>
        ///     Наименование комлекса
        /// </summary>
        public string ComplexName => Dto.Complex?.ComplexName;

        /// <summary>
        ///     Порядок сортировки
        /// </summary>
        public double SortOrder
        {
            get
            {
                switch (Dto.EntityType)
                {
                    case EntityType.Pipeline:
                        return Dto.Works.Count > 0 && Dto.Works.Any(w => w.KilometerStart.HasValue) ? Dto.Works.Where(w => w.KilometerStart.HasValue).Min(w => w.KilometerStart.Value) : 0;

                    case EntityType.CompShop:
                        return ((RepairPlanCompShopDTO) Dto).Kilometer;

                    default:
                        return 9999;
                }
            }
        }

        /// <summary>
        ///     Наименование ЛПУ
        /// </summary>
        public string SiteName => Dto.SiteName;

        /// <summary>
        ///     Вид ремонтных работ
        /// </summary>
        public string RepairTypeName
        {
            get
            {
                return IsExternalCondition
                    ? string.Empty
                    : ClientCache.DictionaryRepository.RepairTypes.Single(rt => rt.Id == Dto.RepairTypeId).Name;
            }
        }

        /// <summary>
        ///     Способ ведения работ
        /// </summary>
        public string ExecutionMeansName
        {
            get
            {
                if (IsExternalCondition)
                {
                    return string.Empty;
                }
                return
                    ClientCache.DictionaryRepository.RepairExecutionMeans.Single(
                        em => em.ExecutionMeans == Dto.ExecutionMeans).Name;
            }
        }

        /// <summary>
        ///     Дата начала работ (плановая)
        /// </summary>
        public DateTime StartDatePlan
        {
            get { return Dto.StartDate; }
            set
            {
                Dto.StartDate = value;
                OnPropertyChanged(()=>StartDatePlan);
            }
        }

        /// <summary>
        ///     Дата окончания работ (плановая)
        /// </summary>
        public DateTime EndDatePlan
        {
            get { return Dto.EndDate; }
            set { Dto.EndDate = value; }
        }

        /// <summary>
        ///     Продолжительность (плановая), часов
        /// </summary>
        public int DurationPlan => (int) (EndDatePlan - StartDatePlan).TotalHours;

        /// <summary>
        ///     Дата поставки МТР (плановая)
        /// </summary>
        public string PartsDeliveryDateString
            => IsExternalCondition ? string.Empty : Dto.PartsDeliveryDate.ToShortDateString();

        /// <summary>
        ///     Описание
        /// </summary>
        public string Description => Dto.Description;

        /// <summary>
        ///     Комментарий ГТП
        /// </summary>
        public string CommentGto => Dto.DescriptionGtp;


        /// <summary>
        ///     Комментарий ЦПДД
        /// </summary>
        /// 
        public string CommentCpdd => Dto.DescriptionCpdd;

        /// <summary>
        ///     Объем стравливаемого газа, млн.м3
        /// </summary>
        public string BleedAmount
            => IsExternalCondition ? string.Empty : Dto.BleedAmount.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        ///     Объем выработанного газа, млн.м3
        /// </summary>
        public string SavingAmount
            => IsExternalCondition ? string.Empty : Dto.SavingAmount.ToString(CultureInfo.InvariantCulture)
            ;

        /// <summary>
        ///     Достигнутый объем транспорта газа на участке, млн.м3/сут (Зима)
        /// </summary>
        public string MaxTransferWinter
            => IsExternalCondition ? string.Empty : Dto.MaxTransferWinter.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        ///     Достигнутый объем транспорта газа на участке, млн.м3/сут (Лето)
        /// </summary>
        public string MaxTransferSummer => IsExternalCondition ? string.Empty : Dto.MaxTransferSummer.ToString();

        /// <summary>
        ///     Достигнутый объем транспорта газа на участке, млн.м3/сут (Межсезонье)
        /// </summary>
        public string MaxTransferTransition => IsExternalCondition ? string.Empty : Dto.MaxTransferTransition.ToString()
            ;

        /// <summary>
        ///     Расчетная пропускная способность участка, млн.м3/сут (Зима)
        /// </summary>
        public string CapacityWinter => IsExternalCondition ? string.Empty : Dto.CapacityWinter.ToString();

        /// <summary>
        ///     Расчетная пропускная способность участка, млн.м3/сут (Лето)
        /// </summary>
        public string CapacitySummer => IsExternalCondition ? string.Empty : Dto.CapacitySummer.ToString();

        /// <summary>
        ///     Расчетная пропускная способность участка, млн.м3/сут (Межсезонье)
        /// </summary>
        public string CapacityTransition => IsExternalCondition ? string.Empty : Dto.CapacityTransition.ToString();

        /// <summary>
        ///     Расчетный объем транспорта газа на период проведения работ, млн.м3/сут.
        /// </summary>
        public string CalculatedTransfer => IsExternalCondition ? string.Empty : Dto.CalculatedTransfer.ToString();

        /// <summary>
        ///     Работы влияют на транспорт газа
        /// </summary>
        public bool IsCritical => Dto.IsCritical;

        /// <summary>
        ///     Пользователь изменивший работу
        /// </summary>
        public string UserName => Dto.UserName;

        /// <summary>
        ///     Подразделение пользователя, изменившего работу
        /// </summary>
        public string UserSiteName => Dto.UserSiteName;

        /// <summary>
        ///     Время последнего изменения работы
        /// </summary>
        public DateTime LastUpdate => Dto.LastUpdateDate;

        public Brush TheColor => IsExternalCondition
            ? new SolidColorBrush(Color.FromArgb(0xff, 0xDC, 0x14, 0x3C))
            : new SolidColorBrush(IsCritical ? Colors.Black : Colors.Gray);

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
                if (!ComplexId.HasValue)
                {
                    return string.Empty;
                }

                StringBuilder result = new StringBuilder();

                if (StartDatePlan < Dto.Complex.StartDate)
                {
                    result.Append("Дата начала ремонта меньше даты начала комплекса");
                }

                if (EndDatePlan > Dto.Complex.EndDate)
                {

                    if (result.Length > 0)
                    {
                        result.AppendLine();
                    }
                    result.Append( "Дата окончания ремонта больше даты окончания комплекса");
                }
                return result.ToString();
            }
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public int CompareTo(object obj)
        {
            var other = obj as RepairItem;
            if (other == null)
            {
                return 1;
            }

            if (GroupingType == RepairGroupingType.ByPipeline)
            {
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

            if (StartDatePlan > other.StartDatePlan)
            {
                return 1;
            }
            return -1;
        }
    }
}