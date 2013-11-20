using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ChartConverter
{
    class EMRImportFile
    {
        public EMRImportFile(string FileName)
        {
            StartOutputFile(FileName);
        }

        private XmlBuilder _xmlSummary;

        public XmlBuilder XmlSummary
        {
            get { return _xmlSummary; }
            set { _xmlSummary = value; }
        }

        public void StartOutputFile(string pFileToCreate)
        {
            if (File.Exists(pFileToCreate)) { File.Delete(pFileToCreate); }
            XmlSummary = new XmlBuilder(pFileToCreate, "Charts");
        }

        private void AddChartNodeToXML(string pChartNumber, string pFirstName,
                                       string pLastName, string pVisitDate,
                                       string pFilePath, string pProviderCode)
        {
            XmlSummary.AddOpenElement("Chart");
            XmlSummary.AddFullElement("ChartNumber", pChartNumber);
            XmlSummary.AddOpenElement("Demographics");
            XmlSummary.AddFullElement("LastName", pLastName);
            XmlSummary.AddFullElement("FirstName", pFirstName);
            XmlSummary.AddFullElement("MiddleInitial", "");
            XmlSummary.AddFullElement("Sex", "");
            XmlSummary.AddFullElement("DOB", "");
            XmlSummary.AddFullElement("SSN", "");
            XmlSummary.AddCloseElement();
            XmlSummary.AddOpenElement("Provider");
            XmlSummary.AddFullElement("ID", pProviderCode);
            XmlSummary.AddFullElement("LastName", "");
            XmlSummary.AddFullElement("FirstName", "");
            XmlSummary.AddFullElement("MiddleInitial", "");
            XmlSummary.AddFullElement("Title", "");
            XmlSummary.AddFullElement("Suffix", "");
            XmlSummary.AddFullElement("UPIN", "");
            XmlSummary.AddCloseElement();
            XmlSummary.AddOpenElement("Location");
            XmlSummary.AddFullElement("Name", "");
            XmlSummary.AddFullElement("ID", "");
            XmlSummary.AddCloseElement();
            XmlSummary.AddOpenElement("Notes");
            XmlSummary.AddFullElement("DateTime", pVisitDate);
            XmlSummary.AddFullElement("Format", "SINGLE_PAGE_IMAGE");
            XmlSummary.AddFullElement("Subject", "");
            XmlSummary.AddFullElement("Comment", "");
            XmlSummary.AddOpenElement("Pages");
            XmlSummary.AddOpenElement("Page");
            XmlSummary.AddFullElement("Path", pFilePath);
            XmlSummary.AddFullElement("Number", "1");
            XmlSummary.AddCloseElement();
            XmlSummary.AddCloseElement();
            XmlSummary.AddCloseElement();
            XmlSummary.AddCloseElement();
        }
    }
}
