using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro.Enum
{
    /// <summary>
    /// статус сводки
    /// </summary>
	public enum SummaryStatusTypes
    {
        /// <summary>
        /// Используется в обмене
        /// </summary>
		Used = 1,

        /// <summary>
        /// Не используется в обмене
        /// </summary>
		NotUsed = 2,

        /// <summary>
        /// Удаленная
        /// </summary>
		Deleted = 3
    }
}
