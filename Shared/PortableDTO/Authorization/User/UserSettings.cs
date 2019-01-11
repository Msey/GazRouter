using System.Runtime.Serialization;
using Utils.Units;

namespace GazRouter.DTO.Authorization.User
{
    public class UserSettings
    {
        public UserSettings()
        {
            PressureUnit = PressureUnit.Kgh;
            TemperatureUnit = TemperatureUnit.Celsius;
            CombHeatUnit = CombustionHeatUnit.kcal;

            EventLogArchivingDelay = 7;
           
            DTAutoRefreshPeriod = 30;
            DTAutoRefreshUse = true;

            DeltaThresholds = new DeltaThresholdList();
        }

        public TemperatureUnit TemperatureUnit { get; set; }

        public PressureUnit PressureUnit { get; set; }

        public CombustionHeatUnit CombHeatUnit { get; set; }


        /// <summary>
        /// Период в днях, за который события отображаются в журнале, 
        /// после этого периода отображаются уже на вкладке архив
        /// </summary>
        public int EventLogArchivingDelay { get; set; }
        

        public DeltaThresholdList DeltaThresholds { get; set; }

        
        #region Параметры обновления ДЗ

        /// <summary>
        /// Использовать автомотическое обновление списка диспечерских задач
        /// </summary>
        [DataMember(IsRequired = false)]
        public bool DTAutoRefreshUse { get; set; }
        /// <summary>
        /// Интервал автоматического обновления списка диспечерских задач
        /// </summary>
        [DataMember(IsRequired = false)]
        public int DTAutoRefreshPeriod { get; set; }

        #endregion

    }
}