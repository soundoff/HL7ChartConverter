using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Xml;

namespace ChartConverter
{
    class XmlBuilder
    {
        private XmlTextWriter xmlTextWrite;

        public XmlBuilder(string Filename, string RootElementText)
        {
            xmlTextWrite = new XmlTextWriter(Filename, null);
            xmlTextWrite.WriteStartDocument();
            xmlTextWrite.WriteStartElement(RootElementText);
        }

        public void AddFullElement(string TagText, string ElementText)
        {
            xmlTextWrite.WriteElementString(TagText, ElementText);
        }

        public void AddOpenElement(string TagText)
        {
            xmlTextWrite.WriteStartElement(TagText);
        }

        public void AddCloseElement()
        {
            xmlTextWrite.WriteEndElement();
        }

        public void CloseXMLFile()
        {
            xmlTextWrite.WriteEndDocument();
            xmlTextWrite.Close();
        }
    }
}
