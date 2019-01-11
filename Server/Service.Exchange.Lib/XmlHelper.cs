using GazRouter.Service.Exchange.Lib.Import;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace GazRouter.Service.Exchange.Lib
{
    public static class XmlHelper
    {
        public static void Save<T>(T obj, string fullPath, XmlAttributeOverrides overs = null, bool isXmlnsEmpty = false)
        {
            var serializer = XmlSerializer.FromTypes(new[] { typeof(T) })[0];
            var xmlnsEmpty = new XmlSerializerNamespaces();
            if (isXmlnsEmpty)
            {
                xmlnsEmpty.Add("", "");
            }


            var folder = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) Directory.CreateDirectory(folder);
            using (var fs = FileTools.OpenOrCreate(fullPath))
            {
                serializer.Serialize(fs, obj, xmlnsEmpty);
            }
        }
        public static void SaveToStream<T>(T obj, Stream stream, XmlAttributeOverrides overs = null, bool isXmlnsEmpty = false)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var serializer = new XmlSerializer(typeof(T));

            //в целом небезопасный код - кто и где почистит память от этого ридера и мемори стрима внизу?
            var writer = new StringWriter();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                CloseOutput = false,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false,
                Indent = true
            };
            using (var xw = XmlWriter.Create(writer, xmlWriterSettings))
            {
                xw.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                serializer.Serialize(xw, obj, ns);
                writer.Flush();
            }
            new MemoryStream(Encoding.UTF8.GetBytes(writer.ToString())).CopyTo(stream);
        }

        public static Stream GetStream<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            MemoryStream memoryStream = new MemoryStream();

            serializer.Serialize(memoryStream, obj);

            return memoryStream;
        }

        public static string GetString<T>(T obj, XmlWriterSettings settings = null, XmlAttributeOverrides overs = null)
        {
            var serializer = new XmlSerializer(typeof(T), overs);
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            using (var fs = new StringWriter(sb))
            {
                serializer.Serialize(writer, obj);
                return fs.ToString();
            }
        }

        public static T Get<T>(byte[] buffer, XmlAttributeOverrides overs = null)
        {
            using (var ms = new MemoryStream(buffer))
            using (var sr = new StreamReader(ms))
            {
                return Get<T>(sr);
            }
        }
        
        public static byte[] GetBytes<T>(T obj, XmlAttributeOverrides overs = null)
        {
            var serializer = new XmlSerializer(typeof(T), overs);

            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private static string RemoveInvalidXmlChars(string content)
        {
            return new string(content.Where(System.Xml.XmlConvert.IsXmlChar).ToArray());
        }

        public static T Get<T>(StreamReader stream, XmlAttributeOverrides overs = null)
        {
            var input = RemoveInvalidXmlChars(stream.ReadToEnd());
            using (var sr = new StringReader(input))
            {
                var serializer = new XmlSerializer(typeof(T), overs);
                return (T)serializer.Deserialize(sr);
            }
        }

        public static Stream Transform(Stream xsl, Stream sourceXml)
        {
            var myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(XmlReader.Create(xsl));
            var result = new MemoryStream();
            var sourceXPathDoc = new XPathDocument(sourceXml);
            //var settings = new XmlWriterSettings
            //{
            //    OmitXmlDeclaration = true,
            //    ConformanceLevel = ConformanceLevel.Auto,
            //    CloseOutput = false
            //};
            //var strm = new MemoryStream();
            //var writer = XmlWriter.Create(strm, settings);
            //myXslTrans.Transform(sourceXPathDoc, null, writer);
            myXslTrans.Transform(sourceXPathDoc, null, result);
            result.Flush();
            return result;
        }
    }
}
