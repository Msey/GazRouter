using System;

namespace GazRouter.DTO.Dictionaries.EventTypes
{    public enum EventType
    {
        //1. событие
        //2. заявка
        //3. нештатная ситуация
        Regular = 1,
        Request = 2,
        Emergency = 3,
    }

    public static class EventTypeHelper
    {
        public static string GetName(this EventType et)
        {
            switch (et)
            {
                case EventType.Regular:
                    return "Cобытие";
                case EventType.Request:
                    return "Заявка";
                case EventType.Emergency:
                    return "Нештатная ситуация";
                default:
                    throw new ArgumentOutOfRangeException("et", "Не выбран EventType");
            }
        }
    }
}
