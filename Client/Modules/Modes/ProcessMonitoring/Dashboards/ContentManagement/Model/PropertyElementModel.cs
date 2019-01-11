using System;
using System.Collections.Generic;
using System.Windows.Media;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{

    public class PropertyElementModel : BoxedElementModel
    {
        public PropertyElementModel()
        {
            FontSize = 11;
            ShowTitle = true;
            FontColor = Colors.Black;
        }

        /// <summary>
        /// Сущность
        /// </summary>
        public Guid EntityId { get; set; }


        /// <summary>
        /// Имя сущности
        /// </summary>
        public string EntityName { get; set; }


        /// <summary>
        /// Тип сущности
        /// </summary>
        public EntityType EntityType { get; set; }


        /// <summary>
        /// Тип свойства
        /// </summary>
        public PropertyType PropertyType { get; set; }


        /// <summary>
        /// Показывать подпись к значению (короткое наименование свойства)
        /// </summary>
        public bool ShowTitle { get; set; }


        /// <summary>
        /// Текстовый комментарий к свойству
        /// </summary>
        public string Comment { get; set; }


        /// <summary>
        /// Список индетификаторов сущностей, которые связаны с данным элементом. 
        /// Необходимо для формирования перечня сущностей для запроса получения данных.
        /// </summary>
        public override List<Guid> GetRelatedEntityList()
        {
            return new List<Guid> { EntityId };
        }
    }
}