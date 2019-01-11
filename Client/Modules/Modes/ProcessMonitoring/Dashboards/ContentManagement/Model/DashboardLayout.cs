using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{
    /// <summary>
    /// Этот клас определяет расположение элементов на панели, их внешний вид и прочие статические свойства
    /// </summary>
    [XmlInclude(typeof(EntityElementModel))]
    [XmlInclude(typeof(TextElementModel))]
    [XmlInclude(typeof(LineElementModel))]
    [XmlInclude(typeof(PropertyElementModel))]
    [XmlInclude(typeof(ShapeElementModel))]
    public class DashboardLayout
    {
        public DashboardLayout()
        {
            ElementList = new List<ElementModelBase>();
            FontSize = 11;
        }

        /// <summary>
        /// Размер шрифта для всех элементов дашборда
        /// </summary>
        public int FontSize { get; set; }


        /// <summary>
        /// Список элементов дашборда
        /// </summary>
        public List<ElementModelBase> ElementList { get; set; }
        
        
        public List<Guid> GetRelatedEntityList()
        {
            var list = new List<Guid>();
            foreach (var e in ElementList.Where(e => e.GetRelatedEntityList() != null))
            {
                list.AddRange(e.GetRelatedEntityList());
            }
            return list;
            
        }
        

        public static string Serialize(DashboardLayout cnt)
        {
            string dataString = "";
            using (TextWriter tw = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(typeof(DashboardLayout));
                xmlSerializer.Serialize(tw, cnt);
                dataString = tw.ToString();
                tw.Close();
            }
            return dataString;
        }

        public static DashboardLayout Deserialize(string data)
        {
            var xmlSerializer = new XmlSerializer(typeof(DashboardLayout));
            TextReader tr = new StringReader(data);
            try
            {
                return (DashboardLayout)xmlSerializer.Deserialize(tr);
                //var dl = (DashboardLayout)xmlSerializer.Deserialize(tr);
                //dl.ElementList.Where(e => e.FontSize == 0).ForEach(e => e.FontSize = 11);
                //return dl;
            }
            catch
            {
                return new DashboardLayout();
            }
            
        }
    }
    
}