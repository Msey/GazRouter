using System;
using GazRouter.DTO.Repairs.Plan;

namespace GazRouter.Repair.UpdateHistory
{
    public class RepairUpdate
    {
        private readonly RepairUpdateDTO _dto;


        public RepairUpdate(RepairUpdateDTO dto)
        {
            _dto = dto;
        }


        public string ActionName
        {
            get
            {
                return _dto.Action != null && _dto.Action.ToUpper() == "U" ? "Изменено" : "Добавлено"; }
        }

        public string UserName
        {
            get { return _dto.UserName; }
        }

        public string SiteName
        {
            get { return _dto.SiteName; }
        }

        public DateTime UpdateTime
        {
            get { return _dto.UpdateDate; }
        }
    }
}