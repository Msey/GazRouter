using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;


namespace GazRouter.ManualInput.Hourly.ObjectForms
{
    public abstract class EntityForm : FormBase
    {
        protected Guid NodeId;
        protected ManualInputEntityNode Node;

        protected EntityForm(ManualInputEntityNode node, DateTime date, int serieId)
            : base (date, serieId)
        {
            Node = node;
            NodeId = node.Entity.Id;
        }
        

        protected static void ExtractPropertyValue(
            ManualInputPropertyValue result,
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> valueDict,
            Guid entityId,
            PropertyType propType,
            DateTime date)
        {
            // получение значения
            if (valueDict.ContainsKey(entityId) && valueDict[entityId].ContainsKey(propType))
            {
                var value = valueDict[entityId][propType].SingleOrDefault(v => v.Date == date);
                if (value != null)
                {
                    result.MessageList = value.MessageList;
                    var dblVal = value as PropertyValueDoubleDTO;
                    if (dblVal != null)
                        result.Value = dblVal.Value;
                }

            }
        }
    }
}