using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.Entities
{
    public class GetEntityListParameterSetBase
    {
        public GetEntityListParameterSetBase()
        {
            HideVirtual = false;
        }
        
        /// <summary>
        /// Показывать виртуальные
        /// </summary>
        public bool HideVirtual { get; set; }
        
    }
    
}