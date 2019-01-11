using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GazRouter.DataServices.Infrastructure;

namespace GazRouter.DataServices.ExchangeServices
{
    public static class XmlMessageFileHelper
    {
        public static string FullPath(string fileName)
        {
            return Path.Combine(AppSettingsManager.AsduExchangeDirectory, fileName);
        }
        
        public static IEnumerable<string> Files
        {
            get { return Directory.GetFiles(AppSettingsManager.AsduExchangeDirectory, "*.xml").ToList(); }
        }

        public static void Save(string fileName, XmlMessage message)
        {
            var serializer = new XmlSerializer(typeof(XmlMessage));
            using (var textWriter = new StreamWriter(FullPath(fileName)))
            {
                serializer.Serialize(textWriter, message);
            }
        }

        public static XmlMessage Load(string fileName)
        {
            var serializer = new XmlSerializer(typeof(XmlMessage));
            using (var reader = new StreamReader(FullPath(fileName)))
            {
                return (XmlMessage) serializer.Deserialize(reader);
            }
        }

        public static string ToXml<T>(T message)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, message);
                return writer.ToString();
            }
        }
    }
}