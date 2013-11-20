using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ChartConverter
{
    public class Configuration
    {
        public Configuration(string pConfigFile)
        {
            ConfigFileLocation = pConfigFile;
        }

        private string _configFileLocation;
        public string ConfigFileLocation
        {
            get { return _configFileLocation; }
            set { _configFileLocation = value; }
        }

        private bool _splitFile;
        public bool SplitFile
        {
            get { return _splitFile; }
            set { _splitFile = value; }
        }

        private int _typeCount;
        public int TypeCount
        {
            get { return _typeCount; }
            set { _typeCount = value; }
        }

        private List<DocumentType> _documentTypes;
        public List<DocumentType> DocumentTypes
        {
            get
            {
                if (_documentTypes == null)
                    _documentTypes = new List<DocumentType>();
                return _documentTypes; 
            }
            set { _documentTypes = value; }
        }

        private Dictionary<string, List<Item>> _documentFileDict;
        public Dictionary<string, List<Item>> DocumentFileDict
        {
            get
            {
                if (_documentFileDict == null)
                    _documentFileDict = new Dictionary<string, List<Item>>();
                return _documentFileDict;
            }
            set { _documentFileDict = value; }
        }

        private Dictionary<string, string> _HL7defaults;
        public Dictionary<string, string> HL7defaults
        {
            get
            {
                if (_HL7defaults == null)
                {
                    _HL7defaults = new Dictionary<string,string>();
                }
                return _HL7defaults; 
            }
            set { _HL7defaults = value; }
        }

        private List<Item> _items;
        public List<Item> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<Item>();
                return _items;
            }
            set { _items = value; }
        }

        private string _HL7PathExtension;
        public string HL7PathExtension
        {
            get { return _HL7PathExtension; }
            set { _HL7PathExtension = value; }
        }

        public void LoadConfiguration()
        {
            Controller.Instance.RaiseShowMessage(this,
                new MessagingEventArgs("Loading configuration from " + ConfigFileLocation));

            // Create the XDocument object by loading the config file
            XDocument config = XDocument.Load(ConfigFileLocation);

            // Next get the types of documents to process this session
            GetDocumentTypes(config);

            // This is the rules for finding items in each document type
            GetItemRulesPerDocument(config);

            // Next get the default values for creating the HL7 xml file
            GetHL7XmlRules(config);

            Controller.Instance.RaiseShowMessage(this,
                new MessagingEventArgs("Configuration load complete"));
        }

        private void GetHL7XmlRules(XDocument config)
        {
            Controller.Instance.RaiseShowMessage(this,
                new MessagingEventArgs("\tLoading HL7 Xml defaults"));

            var hl7Rules = from setting in config.Root.Element("HL7XML").DescendantsAndSelf()
                           where setting.HasElements == false
                           where setting.Value != ""
                           select setting;


            foreach (var rule in hl7Rules)
            {
                this.HL7defaults.Add(rule.Parent.Name.LocalName.ToString() + "." + rule.Name.LocalName.ToString(),
                                     rule.Value);
                
            }

            HL7PathExtension = HL7defaults["HL7XML.Extension"];
            HL7defaults.Remove("HL7XML.Extension");
        }

        private void GetItemRulesPerDocument(XDocument config)
        {
            Controller.Instance.RaiseShowMessage(this,
                new MessagingEventArgs("\tLoading document item rules"));

            var documents = config.Root
                                  .Elements("Documents")
                                  .Descendants("Document")
                                  .Select(rule => new
                                  {
                                      Type = rule.Element("Type").Value,
                                      Items = rule.Element("Items")
                                  });

            foreach (var doc in documents)
            {
                List<Item> itemsList = new List<Item>();

                var items = doc.Items
                               .Descendants("Item")
                               .Select(rule => new
                               {
                                   Description = rule.Element("Description").Value,
                                   PartOf = rule.Element("HowToFind").Element("PartOf").Value,
                                   Start = rule.Element("HowToFind").Element("Start").Value,
                                   End = rule.Element("HowToFind").Element("End").Value,
                                   FrontTrim = rule.Element("HowToFind").Element("FrontTrim").Value,
                                   EndTrim = rule.Element("HowToFind").Element("EndTrim").Value,
                                   IsCritical = rule.Element("HowToFind").Element("Critical").Value,
                                   RemoveEnds = rule.Element("HowToFind").Element("RemoveEnds").Value,
                                   RemoveInternal = rule.Element("HowToFind").Element("RemoveInternal").Value
                               });

                // Loop thru the Items collection pulled for this document
                foreach (var item in items)
                {
                    StartEnd howToFind = new StartEnd();
                    howToFind.FindStart = item.Start;
                    howToFind.FindEnd = item.End;
                    if (item.FrontTrim.Length > 0)
                        howToFind.FrontTrim = Convert.ToInt32(item.FrontTrim);
                    if (item.EndTrim.Length > 0)
                        howToFind.EndTrim = Convert.ToInt32(item.EndTrim);

                    Item docItem = new Item();
                    docItem.Name = item.Description;
                    docItem.PartOf = item.PartOf;
                    docItem.HowToFind = howToFind;
                    docItem.Critical = Convert.ToBoolean(item.IsCritical);
                    docItem.RemoveEnds = item.RemoveEnds.ToCharArray();
                    docItem.RemoveInternal = item.RemoveInternal;
                    itemsList.Add(docItem);
                }

                DocumentFileDict.Add(doc.Type, itemsList);
            }
        }

        private void GetDocumentTypes(XDocument config)
        {
            Controller.Instance.RaiseShowMessage(this,
                new MessagingEventArgs("\tLoading document types"));

            var documentTypes = config.Root
                                      .Elements("TypesOfDocuments")
                                      .Descendants("Type")
                                      .Select(rule => new
                                      {
                                          Description = rule.Element("Description").Value,
                                          Start = rule.Element("HowToDetermine").Element("Start").Value,
                                          End = rule.Element("HowToDetermine").Element("End").Value,
                                          Split = rule.Element("SplitMe").Value,
                                          Rename = rule.Element("FileNaming").Element("Rename").Value,
                                          NameParts = rule.Element("FileNaming").Element("NameParts").Value,
                                          Extension = rule.Element("FileNaming").Element("Extension").Value
                                      });

            foreach (var doctype in documentTypes)
            {
                StartEnd howToFind = new StartEnd();
                howToFind.FindStart = doctype.Start;
                howToFind.FindEnd = doctype.End;

                DocumentType docType = new DocumentType();
                docType.Description = doctype.Description;
                docType.HowToEvalType = howToFind;
                docType.SplitMe = Convert.ToBoolean(doctype.Split);
                docType.Rename = Convert.ToBoolean(doctype.Rename);
                docType.NewFileNameParts = doctype.NameParts;
                docType.NewFileExtension = doctype.Extension;
                DocumentTypes.Add(docType);
            }
        }
    }

    public class DocumentType
    {
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private bool _splitMe;
        public bool SplitMe
        {
            get { return _splitMe; }
            set { _splitMe = value; }
        }

        private bool _rename;
        public bool Rename
        {
            get { return _rename; }
            set { _rename = value; }
        }

        private string _newFileNameParts;
        public string NewFileNameParts
        {
            get { return _newFileNameParts; }
            set { _newFileNameParts = value; }
        }

        private string _newFileExtension;
        public string NewFileExtension
        {
            get { return _newFileExtension; }
            set { _newFileExtension = value; }
        }

        private StartEnd _howToEvalType;
        public StartEnd HowToEvalType
        {
            get
            {
                if (_howToEvalType == null)
                    _howToEvalType = new StartEnd();
                return _howToEvalType; 
            }
            set { _howToEvalType = value; }
        }
    }

    public class Item
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _partOf;
        public string PartOf
        {
            get { return _partOf; }
            set { _partOf = value; }
        }

        private StartEnd _howToFind;
        public StartEnd HowToFind
        {
            get
            {
                if (_howToFind == null)
                    _howToFind = new StartEnd();
                return _howToFind;
            }
            set { _howToFind = value; }
        }

        private string _foundValue;
        public string FoundValue
        {
            get { return _foundValue; }
            set { _foundValue = value; }
        }

        private bool _critical;
        public bool Critical
        {
            get { return _critical; }
            set { _critical = value; }
        }

        private char[] _removeEnds;
        public char[] RemoveEnds
        {
            get { return _removeEnds; }
            set { _removeEnds = value; }
        }

        private string _removeInternal;
        public string RemoveInternal
        {
            get { return _removeInternal; }
            set { _removeInternal = value; }
        }
    }

    public class StartEnd
    {
        private string _findStart;
        public string FindStart
        {
            get { return _findStart; }
            set { _findStart = value; }
        }

        private string _findEnd;
        public string FindEnd
        {
            get { return _findEnd; }
            set { _findEnd = value; }
        }

        private int _frontTrim;
        public int FrontTrim
        {
            get { return _frontTrim; }
            set { _frontTrim = value; }
        }

        private int _endTrim;
        public int EndTrim
        {
            get { return _endTrim; }
            set { _endTrim = value; }
        }
    }
}
