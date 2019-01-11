using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.Dictionaries.Integro
{
    public enum SessionDataType
    {
        //[Description("Режимно технологические")]
        RT,
        //[Description("Учетно-балансовая информация")]
        UB,
        //[Description("Плановые значения по ГРС")]
        PL,
        //[Description("НСИ")]
        NSI,
        //[Description("Месячный. Производственно-отчтная информация. Отчет о добычи.")]
        PROD,
        //[Description("Месячный. Производственно-отчтная информация. Отчет о работе предприятия.")]
        PRO,
        //[Description("Месячный. Производственно-отчтная информация. Отчет 1П.")]
        F1P,
        //[Description("---. АСППОТИ, ")]
        РТ2,
        //[Description("---. АСППОТИ, ")]
        РТ24,

    }
}
