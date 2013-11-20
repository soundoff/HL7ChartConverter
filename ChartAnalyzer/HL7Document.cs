using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace ChartConverter
{
    public class HL7Charts
    {
        private List<HL7Chart> _sectionChart;
        public List<HL7Chart> SectionChart
        {
            get 
            {
                if (_sectionChart == null)
                {
                    _sectionChart = new List<HL7Chart>();
                }
                return _sectionChart;
            }
            set { _sectionChart = value; }
        }

        public void AddNew(HL7Chart pChart)
        {
            SectionChart.Add(pChart);
        }

        public void CreateFinalXML(string pFileName)
        {
            XDocument xml = new XDocument();
            XElement root = new XElement("Charts");

            foreach (HL7Chart chart in SectionChart)
            {
                root.Add(chart.ToXElement());
            }

            xml.Add(root);
            xml.Save(pFileName);
        }
    }

    public class HL7Chart
    {
        private string _sectionchartNumber;
        public string SectionchartNumber
        {
            get { return _sectionchartNumber; }
            set { _sectionchartNumber = value; }
        }

        private Demographics _sectiondemographic;
        internal Demographics Sectiondemographic
        {
            get 
            {
                if (_sectiondemographic == null)
                {
                    _sectiondemographic = new Demographics();
                }
                return _sectiondemographic; 
            }
            set { _sectiondemographic = value; }
        }

        private Provider _sectionProvider;
        internal Provider SectionProvider
        {
            get 
            {
                if (_sectionProvider == null)
                {
                    _sectionProvider = new Provider();
                }
                return _sectionProvider; 
            }
            set { _sectionProvider = value; }
        }

        private Location _sectionLocation;
        internal Location SectionLocation
        {
            get 
            {
                if (_sectionLocation == null)
                {
                    _sectionLocation = new Location();
                }
                return _sectionLocation; }
            set { _sectionLocation = value; }
        }

        private Notes _sectionNotes;
        internal Notes SectionNotes
        {
            get 
            {
                if (_sectionNotes == null)
                {
                    _sectionNotes = new Notes();
                }
                return _sectionNotes; 
            }
            set { _sectionNotes = value; }
        }

        public XElement ToXElement()
        {
            XElement root = new XElement("Chart",
                                new XElement("ChartNumber", SectionchartNumber),
                                Sectiondemographic.ToXElement(),
                                SectionProvider.ToXElement(),
                                SectionLocation.ToXElement(),
                                SectionNotes.ToXElement());
            return root;
        }

        public void RouteValue(string pElement, string pValue)
        {
            switch (pElement)
            {
                case "Defaults.ChartNumber":
                    SectionchartNumber = pValue;
                    break;
                case "Demographics.LastName":
                    Sectiondemographic.LastName = pValue;
                    break;
                case "Demographics.FirstName":
                    Sectiondemographic.FirstName = pValue;
                    break;
                case "Demographics.MiddleInitial":
                    Sectiondemographic.MiddleInitial = pValue;
                    break;
                case "Demographics.Sex":
                    Sectiondemographic.Sex = pValue;
                    break;
                case "Demographics.DOB":
                    Sectiondemographic.Dob = pValue;
                    break;
                case "Demographics.SSN":
                    Sectiondemographic.Ssn = pValue;
                    break;
                case "Provider.ID":
                    SectionProvider.Id = pValue;
                    break;
                case "Provider.LastName":
                    SectionProvider.LastName = pValue;
                    break;
                case "Provider.FirstName":
                    SectionProvider.FirstName = pValue;
                    break;
                case "Provider.MiddleInitial":
                    SectionProvider.MiddleInitial = pValue;
                    break;
                case "Provider.Title":
                    SectionProvider.Title = pValue;
                    break;
                case "Provider.Suffix":
                    SectionProvider.Suffix = pValue;
                    break;
                case "Provider.UPIN":
                    SectionProvider.Upin = pValue;
                    break;
                case "Location.Name":
                    SectionLocation.Name = pValue;
                    break;
                case "Location.ID":
                    SectionLocation.Id = pValue;
                    break;
                case "Notes.DateTime":
                    SectionNotes.DateTime = pValue;
                    break;
                case "Notes.Format":
                    SectionNotes.Format = pValue;
                    break;
                case "Notes.Subject":
                    SectionNotes.Subject = pValue;
                    break;
                case "Notes.Comment":
                    SectionNotes.Comment = pValue;
                    break;
                case "Notes.Path":
                    SectionNotes.Page.Path = pValue;
                    break;
                case "Notes.Number":
                    SectionNotes.Page.Number = pValue;
                    break;
            }
        }
    }

    public class Demographics
    {
        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _middleInitial;
        public string MiddleInitial
        {
            get { return _middleInitial; }
            set { _middleInitial = value; }
        }

        private string _sex;
        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        private string _dob;
        public string Dob
        {
            get { return _dob; }
            set { _dob = value; }
        }

        private string _ssn;
        public string Ssn
        {
            get { return _ssn; }
            set { _ssn = value; }
        }

        public XElement ToXElement()
        {
            XElement root = new XElement("Demographics",
                                new XElement("LastName", LastName),
                                new XElement("FirstName", FirstName),
                                new XElement("MiddleInitial", MiddleInitial),
                                new XElement("Sex", Sex),
                                new XElement("DOB", Dob),
                                new XElement("SSN", Ssn));
            return root;
        }
    }

    public class Provider
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _middleInitial;
        public string MiddleInitial
        {
            get { return _middleInitial; }
            set { _middleInitial = value; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _suffix;
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        private string _upin;
        public string Upin
        {
            get { return _upin; }
            set { _upin = value; }
        }

        public XElement ToXElement()
        {
            XElement root = new XElement("Provider",
                                new XElement("ID", Id),
                                new XElement("LastName", LastName),
                                new XElement("FirstName", FirstName),
                                new XElement("MiddleInitial", MiddleInitial),
                                new XElement("Title", Title),
                                new XElement("Suffix", Suffix),
                                new XElement("UPIN", Upin));
            return root;
        }

    }

    public class Location
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public XElement ToXElement()
        {
            XElement root = new XElement("Location",
                                new XElement("Name", Name),
                                new XElement("ID", Id));
            return root;
        }
    }

    public class Notes
    {
        private string _dateTime;
        public string DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        private string _subject;
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        private List<Page> _pages;
        internal List<Page> Pages
        {
            get 
            {
                if (_pages == null)
                {
                    _pages = new List<Page>();
                }
                return _pages; 
            }
            set { _pages = value; }
        }

        private Page _page;
        internal Page Page
        {
            get
            {
                if (_page == null)
                {
                    _page = new Page();
                }
                return _page;
            }
            set { _page = value; }
        }

        public void AddNewPageToPages()
        {
            Pages.Add(Page);
        }

        public XElement ToXElement()
        {
            XElement root = new XElement("Notes",
                                new XElement("DateTime", DateTime),
                                new XElement("Format", Format),
                                new XElement("Subject", Subject),
                                new XElement("Comment", Comment),
                                GetPagesElement());
            return root;
        }

        private XElement GetPagesElement()
        {
            XElement pagesElement = new XElement("Pages");
            foreach (Page page in Pages)
            {
                pagesElement.Add(page.ToXElement());
            }

            return pagesElement;
        }
    }

    public class Page
    {
        private string _path;
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private string _number;
        public string Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public XElement ToXElement()
        {
            XElement root = new XElement("Page",
                                new XElement("Path",Path),
                                new XElement("Number",Number));
            return root;
        }
    }
}
