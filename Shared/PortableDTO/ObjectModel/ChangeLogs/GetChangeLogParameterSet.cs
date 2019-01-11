using System;

namespace GazRouter.DTO.ObjectModel.ChangeLogs
{
    public class GetChangeLogParameterSet
    {
        public string TableName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsExec { get; set; }
        public string TableNameToTable()
        {
            switch (TableName)
            {
                case "Типы электроагрегатов":           return "POWER_UNIT_TYPES";
                case "Типы котлов":                     return "BOILER_TYPES";
                case "Типы кранов-регуляторов":         return "REGULATOR_TYPES";
                case "Типы предохранительных клапанов": return "EMERGENCY_VALVE_TYPES";
                case "Типы подогревателей газа":        return "HEATER_TYPES";
                default:                                return null;
            }
        }

    }
}
