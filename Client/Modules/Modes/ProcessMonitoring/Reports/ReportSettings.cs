
namespace GazRouter.Modes.ProcessMonitoring.Reports
{
    public class ReportSettings
    {
        /// <summary>
        /// Используется элемент выбора серии (2-х часовки)
        /// </summary>
        public bool SerieSelector { get; set; }

        /// <summary>
        /// Используется элемент выбора ЛПУ
        /// </summary>
        public bool SiteSelector { get; set; }

        /// <summary>
        /// Добавить возможность выбирать пустое ЛПУ (т.е. по всему предприятию разом)
        /// </summary>
        public bool EmptySiteAllowed { get; set; }

        /// <summary>
        /// Используется элемент выбора периода
        /// </summary>
        public bool PeriodSelector { get; set; }

        /// <summary>
        /// Используется элемент выбора ГТС
        /// </summary>
        public bool SystemSelector { get; set; }

        
        /// <summary>
        /// Скрывает или отображает детальную информацию
        /// </summary>
        public bool DetailView { get; set; }
    }
}
