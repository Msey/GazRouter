using System;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.GasLeaks;

namespace GazRouter.GasLeaks.ViewModels
{
    public class ExtLeakDTO : LeakDTO
    {
        public ExtLeakDTO(LeakDTO leakDTO)
        {
	        ContactName = leakDTO.ContactName;
			CreateDate = leakDTO.CreateDate;
			Description = leakDTO.Description;
			DiscoverDate = leakDTO.DiscoverDate;
			EntityId = leakDTO.EntityId;
			EntityName = leakDTO.EntityName;
			EntityShortPath = leakDTO.EntityShortPath;
			EntityType = leakDTO.EntityType;
			Id = leakDTO.Id;
			Kilometer= leakDTO.Kilometer;
			Place= leakDTO.Place;
			Reason= leakDTO.Reason;
			RepairActivity= leakDTO.RepairActivity;
			RepairPlanDate= leakDTO.RepairPlanDate;
			RepairPlanFact= leakDTO.RepairPlanFact;
			SiteName= leakDTO.SiteName;
			UserName= leakDTO.UserName;
			VolumeDay= leakDTO.VolumeDay;
        }

        /// <summary>
        /// Если тип объекта = газопровод, то отображается значение Kilometer, в противном случает текст Place
        /// </summary>
        public string ThePlace
        {
            get
            {
                return
                    EntityType == EntityType.Pipeline
                        ? string.Format("{0} км.", Kilometer)
                        : Place;
            }
        }

        /// <summary>
        /// Суммарный объем газа с момента обнаружения
        /// </summary>
        public double VolumeTotal
        {
            get
            {
                if (RepairPlanFact.HasValue)
                {
                    return Math.Round((RepairPlanFact.Value - DiscoverDate).TotalHours/24*VolumeDay, 3);
                }
                return Math.Round((DateTime.Now - DiscoverDate).TotalHours / 24 * VolumeDay, 3);
            }
        }


        public bool IsRectified
        {
            get
            {
                return RepairPlanFact.HasValue;
            }
        }

    }
}