using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.Entities
{
    public class GetEntityListParameterSet
    {
        public GetEntityListParameterSet()
        {
            EntityIdList = new List<Guid>();
        }

        /// <summary>
        /// Показывать скрытые объекты  
        /// </summary>
        public bool ShowHidden { get; set; }

        /// <summary>
        /// Показывать удаленные
        /// </summary>
        public bool ShowDeleted { get; set; }

        /// <summary>
        /// Показывать виртуальные
        /// </summary>
        public bool ShowVirtual { get; set; }

        public bool? IsInputOff { get; set; }


        public EntityType? EntityType { get; set; }

        public List<Guid> EntityIdList { get; set; }

        /// <summary>
        /// Идентификатор предприятия
        /// </summary>
        public Guid? EnterpriseId { get; set; }

        /// <summary>
        /// Идентификатор ГТС
        /// </summary>
        public int? SystemId { get; set; }

        /// <summary>
        /// Идентификатор предприятия
        /// </summary>
        public Guid? SiteId { get; set; }

        /// <summary>
        /// Для поиска по вхождению строки в наименование объекта
        /// </summary>
        public string NamePart { get; set; }

        /// <summary>
        /// Для поиска по вхождению строки в короткий путь установить в TRUE, 
        /// FALSE - будет искать только в наименование объекта
        /// </summary>
        public bool SearchPath { get; set; }
    }
    
}