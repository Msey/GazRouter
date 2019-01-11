using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.DispatcherTasks.TaskStatuses;
using Microsoft.Practices.ServiceLocation;


namespace GazRouter.Modes.DispatcherTasks.Enterprise
{
    public class TaskItem : PropertyChangedBase
    {
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public TaskItem(TaskDTO dto)
        {
            Dto = dto;
        }


        public TaskDTO Dto { get; }

        /// <summary>
        /// Список допустимых статусов для данного
        /// </summary>
        public List<StatusType> AllowedStatusList
            =>
                ClientCache.DictionaryRepository.TaskStatusTypes.Single(s => s.StatusType == Dto.StatusType)
                    .AllowedStatusList;
        
        
        public bool IsChangeable => Dto.StatusType != StatusType.Annuled && Dto.StatusType != StatusType.Performed && Dto.StatusType != StatusType.ApprovedForSite;

        public bool IsDeletable => string.IsNullOrEmpty(Dto.GlobalTaskId) && Dto.StatusType == StatusType.Created;

        public bool IsApprovedForSite => Dto.StatusType == StatusType.ApprovedForSite;

        
    }
}