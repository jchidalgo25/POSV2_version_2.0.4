using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;

namespace POS.Control.Common
{
    public static class XmlHelper
    {
        public static bool ValidateXml(string xml, string xsd)
        {
            try
            {
                XmlSchemaSet schema = new XmlSchemaSet();
                XmlSchema xmlSchema = new XmlSchema();
                xmlSchema = XmlSchema.Read(new System.IO.StringReader(xsd), null);
                schema.Add(xmlSchema);
                XmlReader rd = XmlReader.Create(new System.IO.StringReader(xml));
                System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(rd);
                doc.Validate(schema, ValidationEventHandler);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (System.Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            }
        }

        public static bool IsMinimallyValidXml(string text)
        {
            if (string.IsNullOrEmpty(text)) text = string.Empty;
            System.IO.Stream stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(text));
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = true,
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                ValidationFlags = XmlSchemaValidationFlags.None,
                ValidationType = ValidationType.None,
            };
            bool isValid;

            using (XmlReader xmlReader = XmlReader.Create(stream, settings))
            {
                try
                {
                    while (xmlReader.Read())
                    {
                        ; // Este espacio se deja intencionalmente en blanco
                    }
                    isValid = true;
                }
                catch (XmlException)
                {
                    isValid = false;
                }
            }
            return isValid;
        }

        public static string XmlToText(string strXml)
        {
            try
            {
                int maxStringLineLength = 50;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strXml);

                StringBuilder sb = new StringBuilder();
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.ChildNodes.Count > 1)
                    {
                        foreach (XmlNode nodeJr in node)
                        {
                            sb.AppendLine(Common.StringHelper.ToLimitedLength(nodeJr.InnerText, maxStringLineLength));
                        }
                        sb.AppendLine("");
                    }
                    else
                    {
                        //sb.Append(char.ToUpper(node.Name[0]));
                        //sb.Append(node.Name.Substring(1));
                        //sb.Append(' ');
                        if (node.InnerText.Trim().Length > maxStringLineLength) sb.AppendLine("");
                        sb.AppendLine(Common.StringHelper.ToLimitedLength(node.InnerText, maxStringLineLength));
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogMessage(Common.Enum.LogTypes.Error, "XmlHelper", "XmlToText", ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                return strXml;
            }
        }
    }
}
