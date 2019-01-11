using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Import;
using Microsoft.JScript;
using Saxon.Api;

namespace GazRouter.Service.Exchange.Lib
{
    public class XsltProcessor
    {
        private Stream _outputStream;
        private string _xsl;
        private string _output;
        private List<KeyValuePair<QName, XdmValue>> _params = new List<KeyValuePair<QName, XdmValue>>();
        private DocumentBuilder _db;
        private Processor _proc;
        const string XmlStub = @"<?xml version=""1.0"" encoding=""utf-8""?><root></root>"; //for consistency


        public XsltProcessor()
        {
            _proc = new Processor()  ;
            _db = _proc.NewDocumentBuilder();
            _db.BaseUri = new Uri(@"http://exchange");
        }

        public XsltProcessor(string xsl) : this()
        {
            _xsl = xsl;
        }



        public void Run(string xml, Stream outStream)
        {
            using (var sxsl = XmlReader.Create(new StringReader(_xsl), null , "xsl"))
            using (var sxml = XmlReader.Create(new StringReader(xml), null, "xml"))
            {
                var input = _db.Build(sxml);
                var xsltCompiler = _proc.NewXsltCompiler();
                var transformer = xsltCompiler.Compile(sxsl).Load();
                transformer.InitialContextNode = input;
                var serializer = new Serializer();

                serializer.SetOutputStream(outStream);
                transformer.Run(serializer);
                outStream.Flush();
                outStream.Close();
            }
        }

        public void Run(Stream inStream, Stream outStream)
        {
            using (var sxsl = XmlReader.Create(new StringReader(_xsl), null, "xsl"))
            {
                inStream.Position = 0;
                var input = _db.Build(inStream);
                var xsltCompiler = _proc.NewXsltCompiler();
                var transformer = xsltCompiler.Compile(sxsl).Load();
                transformer.InitialContextNode = input;
                var serializer = new Serializer();

                serializer.SetOutputStream(outStream);
                transformer.Run(serializer);
                outStream.Flush();
                outStream.Close();
            }
        }


        public void Parse(Stream outputStream)
        {

            try
            {
                using (var sxsl = XmlReader.Create(new StringReader(_xsl), null , "xsl"))
                using (var sxml = XmlReader.Create(new StringReader(XmlStub), null , "xml"))
                {
                    var input = _db.Build(sxml);
                    var comp = _proc.NewXsltCompiler();
                    var transformer = comp.Compile(sxsl).Load();
                    transformer.InitialContextNode = input;
                    foreach (var p in _params)
                    {
                        transformer.SetParameter(p.Key, p.Value);
                    }    
                    var serializer = new Serializer();
                    serializer.SetOutputProperty(Serializer.METHOD, "xml");
                    serializer.SetOutputProperty(Serializer.INDENT, "yes");
                    serializer.SetOutputProperty(Serializer.ENCODING, "utf-8");
                    serializer.SetOutputStream(outputStream);
                    transformer.Run(serializer);
                    outputStream.Flush();
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException) throw;
                new MyLogger("exchangeLogger").WriteException(e, e.Message);
            }
        }

        public void AddParameter(string key, string value)
        {
            _params.Add(CreateParameter(key, value));
        }

        private KeyValuePair<QName, XdmValue> CreateParameter(string key, string value)
        {
            return new KeyValuePair<QName, XdmValue>(new QName("", "", key), new XdmAtomicValue(value));
        }


        public static bool CheckXsl(string xsl, out string errMessage)
        {
            errMessage = null;
            return true;
        }


    }



}
