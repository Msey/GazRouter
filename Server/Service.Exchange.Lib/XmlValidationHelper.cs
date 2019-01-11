using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace GazRouter.Service.Exchange.Lib
{
    public class XmlValidationHelper
    {
        public List<string> Errors { get; private set; }
        public List<string> Warnings { get; private set; }
        public XmlSchemaSet Schemas { get; set; }

        public XmlValidationHelper()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Errors.Add($"Error: {e.Message}");
                    break;
                case XmlSeverityType.Warning:
                    Warnings.Add($"Warning: {e.Message}");
                    break;
            }

        }

        public void AddSchema(string namesp, string filename)
        {
            var reader = XmlReader.Create(filename);
            if (Schemas == null)
                Schemas = new XmlSchemaSet();
            Schemas.Add(namesp, reader);
        }

        //public void AddSchema(string namesp, Stream stream)
        //{
        //    var reader = XmlReader.Create(stream);
        //    if (Schemas == null)
        //        Schemas = new XmlSchemaSet();
        //    Schemas.Add(namesp, reader);
        //}

        //public bool ValidateString(string sourceXml)
        //{
        //    var stream = StreamHelper.StringToStream(sourceXml);
        //    return ValidateStream(stream);
        //}


        //public bool ValidateFile(string filename)
        //{
        //    using (var stream = StreamHelper.ReadFile(filename))
        //    {
        //        try
        //        {
        //            return ValidateStream(stream);
        //        }
        //        finally
        //        {
        //            stream.Close();
        //        }
        //    }
        //}

        public bool ValidateStream(Stream inputStream)
        {
            if (Schemas == null || Schemas.Count == 0) return true;
            try
            {
                var document = XDocument.Load(inputStream);
                document.Validate(Schemas, ValidationEventHandler);
            }
            catch (Exception e)
            {
                Errors.Add($"Validation error: {e.Message}");
            }
            return !Errors.Any();
        }

    }
}
