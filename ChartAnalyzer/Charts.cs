using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading;

namespace ChartConverter
{
    public class Charts : IModel
    {
        public Charts() { Controller.Instance.RegisterModel(this); }

        private MSWord ms = MSWord.Instance;
        private int _totalProcessCount;
        private int _currentProcessCount;

        public string ApplicationPath
        {
            get { return Path.GetDirectoryName(Application.ExecutablePath);}
        }

        private string _importFromParentPath;
        public string ImportFromParentPath
        {
            get { return _importFromParentPath; }
            private set
            {
                _importFromParentPath = value;
                Controller.Instance.StartErrorLogging(ImportFromParentPath);
            }
        }

        private string _importFromPath;
        public string ImportFromPath
        {
            get { return _importFromPath; }
            private set
            {
                _importFromPath = value;
                TempDocsPath = ImportFromPath + @"\WorkingFiles\";

                /* Get an array of all of the files in the current directory
                 * with a .DOC extension.  These are the files that will be 
                 * processed.  */

                //FilesToImport = Directory.GetFiles(_importFromPath, "*.doc");
            }
        }

        private string[] _filesToImport;
        public string[] FilesToImport
        {
            get { return _filesToImport; }
            set { _filesToImport = value; }
        }

        private string _tempDocsPath;
        public string TempDocsPath
        {
            get { return _tempDocsPath; }
            private set
            {
                _tempDocsPath = value;

                // Create the directory if it doesn't exist
                if (!Directory.Exists(_tempDocsPath))
                {
                    try { Directory.CreateDirectory(_tempDocsPath); }
                    catch (IOException ioEx)
                    { MessageBox.Show("An error occurred while creating the new document directory" +
                                        ioEx.Message); }
                }
            }
        }

        private int _tempDocumentNumber;
        public int TempDocumentNumber
        {
            get { return _tempDocumentNumber; }
            set { _tempDocumentNumber = value; }
        }

        private string _currentTextVersion;
        public string CurrentTextVersion
        {
            get { return _currentTextVersion; }
            set { _currentTextVersion = value; }
        }

        private Configuration _configuration;
        public Configuration Config
        {
            get { return _configuration; }
            set { _configuration = value; }
        }

        private List<SingleDocument> _singleDocuments;
        public List<SingleDocument> SingleDocuments
        {
            get 
            {
                if (_singleDocuments == null)
                {
                    _singleDocuments = new List<SingleDocument>();
                }
                return _singleDocuments; 
            }
            set { _singleDocuments = value; }
        }

        public void ProcessAllDocuments(object sender, Action action)
        {
            Controller.Instance.RaiseShowStartTime(DateTime.Now.ToLocalTime());

            switch (action)
            {
                case Action.ProcessCharts:
                    ProcessPatientCharts();
                    break;
                case Action.ConvertCharts:
                    ConvertPatientChartsToText();
                    break;
                case Action.ConvertDocToTif:
                    ConvertGroupDocsToTif();
                    break;
                default:
                    Controller.Instance.RaiseShowMessage(this,
                    new MessagingEventArgs("Process is ending because an unknown action was requested"));
                    break;
            }

            Controller.Instance.StopErrorLogging();
            Controller.Instance.RaiseShowFinishTime(DateTime.Now.ToLocalTime());
        }

        private void ProcessPatientCharts()
        {
            // Get the entire configuration settings
            string configFileLocation = ApplicationPath + @"\configuration.xml";
            Config = new Configuration(configFileLocation);
            Config.LoadConfiguration();

            if (ChooseFolderToImport())
            {
                ResetDirectoriesForProcess(ImportFromParentPath);
                var AllFiles = from file in Directory.GetFiles(ImportFromParentPath, "*", SearchOption.AllDirectories)
                               where Path.GetExtension(file).ToUpper() == ".DOC"
                               where ((File.GetAttributes(file) & FileAttributes.Hidden) != FileAttributes.Hidden)
                               orderby file
                               select file;

                _totalProcessCount = AllFiles.Count();

                
                foreach (var fullPath in AllFiles)
                {
                    Controller.Instance.RaiseShowCurrentFile(Path.GetFileName(fullPath));
                    ImportFromPath = fullPath.ExtractPath();

                    // Open the word document that will be processed in this pass
                    try
                    {
                        ms.SetOriginalDocument(fullPath);
                        CurrentTextVersion = ms.ConvertDocToText(fullPath);

                        DocumentType thisType = DetermineDocumentType(fullPath);
                        if (thisType != null)
                        {
                            if (thisType.SplitMe)
                                SetupSplitDocument(fullPath, thisType);
                            else
                                SetupNonSplitDocument(fullPath, thisType);
                        }
                        // Close the word document that was just processed
                        ms.CloseOriginalDocumentWithoutSaving();
                    }
                    catch (Exception ex)
                    {
                        // Get the new processed file path
                        string saveFileName = GetSetupErrorDocPath(fullPath);

                        // Move the untyped file to the error directory
                        if (!File.Exists(saveFileName))
                            File.Copy(fullPath, saveFileName);
                        Controller.Instance.RaiseLogError(new LogErrorEventArgs(ex, "", fullPath, true));
                    }

                    // Update the forms status count
                    Controller.Instance.RaiseShowStatus(++_currentProcessCount, _totalProcessCount);
                }
                // Clear the current file on the form now that it's done
                Controller.Instance.RaiseShowCurrentFile("");
                
                BuildHL7();
            }
            else
            {
                Controller.Instance.RaiseShowMessage(this,
                    new MessagingEventArgs("Process is ending because you did not choose a folder to process"));
            }
        }

        private DocumentType DetermineDocumentType(string pCurrentFile)
        {
            string saveFileName;
            //CurrentTextVersion = ms.ConvertDocToText(pCurrentFile);

            DocumentType currentType = Config.DocumentTypes.Find(
                delegate(DocumentType t)
                {
                    if (t.HowToEvalType.FindEnd == "")
                    {
                        return CurrentTextVersion.ValidateDocumentType(t.HowToEvalType.FindStart);
                    }
                    else
                    {
                        return CurrentTextVersion.ValidateDocumentType(t.HowToEvalType.FindStart,
                                                                 t.HowToEvalType.FindEnd);
                    }
                });

            if (currentType == null)
            {
                // Get the new processed file path
                saveFileName = GetSetupErrorDocPath(pCurrentFile);

                // Move the untyped file to the error directory
                if (!File.Exists(saveFileName))
                    File.Copy(pCurrentFile, saveFileName);

                // Log the error
                Controller.Instance.RaiseLogError(new LogErrorEventArgs(null,
                "The application could not determine the document type during setup process for: " +
                "\r\n\tFile name: " + pCurrentFile +
                "\r\n\tThe file has been moved to: " + saveFileName,
                pCurrentFile, true));
            }

            return currentType;
        }

        public void SetupSplitDocument(string pCurrentFile, DocumentType pCurrentType)
        {
            List<int> startMarks;
            List<int> endMarks;
            string saveFileName;

            
            // Get all of the start and end points of each document in the joint document
            startMarks = CurrentTextVersion.FindDocumentStarts(pCurrentType.HowToEvalType.FindStart);
            endMarks = CurrentTextVersion.FindDocumentEnds(pCurrentType.HowToEvalType.FindEnd);

            if (startMarks.Count == endMarks.Count)
            {
                for (int i = 0; i < startMarks.Count; i++)
                {
                    ms.SelectSingleChart(startMarks.ElementAt(i), endMarks.ElementAt(i));
                    ms.CreateNewDocument();
                    ms.InsertTextIntoDocument();
                    saveFileName = TempDocsPath + TempDocumentNumber.ToString() + ".doc";


                    if (File.Exists(saveFileName))
                    {
                        ms.CloseNewDocumentWithoutSaving();

                        // Log the error
                        Controller.Instance.RaiseLogError(new LogErrorEventArgs(null,
                        "The document failed to write during setup process because a file with that name already exists: " +
                        "\r\n\tFile name: " + pCurrentFile +
                        "\r\n\tThe file has been moved to: " + saveFileName,
                        pCurrentFile, true));
                    }
                    else
                    {
                        ms.SaveNewDocument(saveFileName);
                        TempDocumentNumber += 1;

                        CurrentTextVersion = ms.ConvertDocToText(saveFileName);
                        SetupNonSplitDocument(saveFileName, pCurrentType);
                        ms.CloseNewDocumentWithoutSaving();
                    }
                }
            }
            else
            {
                // Get the new processed file path
                saveFileName = GetSetupErrorDocPath(pCurrentFile);

                // Move the untyped file to the error directory
                if (!File.Exists(saveFileName))
                    File.Copy(pCurrentFile, saveFileName);

                // Log the error
                Controller.Instance.RaiseLogError(new LogErrorEventArgs(null,
                "The document failed to split correctly during setup process because beginning and end split points did not match: " +
                "\r\n\tFile name: " + pCurrentFile +
                "\r\n\tThe file has been moved to: " + saveFileName,
                pCurrentFile, true));
            }
        }

        public void SetupNonSplitDocument(string pCurrentFile, DocumentType pCurrentType)
        {
            try
            {
                // Load the list of single documents for final processing
                ProcessOneDocument(new SingleDocument
                {
                    DocType = pCurrentType,
                    WorkingFilePath = pCurrentFile,
                    ProcessedFilePath = ImportFromPath + @"\ProcessedFiles\",
                    ErrorFilePath = ImportFromPath + @"\ErrorFiles\"
                });
            }

            catch (Exception ex)
            {
                throw;
                /*
                // Get the new processed file path
                string saveFileName = GetSetupErrorDocPath(pCurrentFile);

                // Move the untyped file to the error directory
                if (!File.Exists(saveFileName))
                    File.Copy(pCurrentFile, saveFileName);
                */
            }
        }

        /// <summary>
        /// Processes only one passed SingleDocument
        /// </summary>
        /// <returns>void</returns>
        public void ProcessOneDocument(SingleDocument pCurrentFile)
        {
            // Get the item values specified for this document type from the current document
            if (GetDocumentItemValues(pCurrentFile))
            {
                // Get the new processed file path
                string saveFileName = GetProcessedDocPath(pCurrentFile, false);
                
                File.Copy( pCurrentFile.WorkingFilePath, saveFileName ); 
                // Not converting to TIF now
                //ms.ConvertDocToTiff(pCurrentFile.WorkingFilePath, saveFileName);

                // Add the single document to the Documents collection
                SingleDocuments.Add(pCurrentFile);
            }
            else
            {
                // Get the new processed file path
                string saveFileName = GetProcessedDocPath(pCurrentFile, true);

                // Copy the file to the error folder
                if (!File.Exists(saveFileName))
                    File.Copy(pCurrentFile.WorkingFilePath, saveFileName);
            }
        }

        private bool GetDocumentItemValues(SingleDocument pCurrentFile)
        {
            string itemToAdd;
            bool errorsFound = false;
            var itemList = Config.DocumentFileDict[pCurrentFile.DocType.Description];

            foreach (var item in itemList)
            {
                string start = item.HowToFind.FindStart;
                string end = item.HowToFind.FindEnd;
                int frontTrim = item.HowToFind.FrontTrim;
                int endTrim = item.HowToFind.EndTrim;
                string startingText = "";
                itemToAdd = null;

                /* If the item to be found is actually the description
                 * value of previous item description, then use the value
                 * of the item with that description to pull the new 
                 * item value from. */

                if (pCurrentFile.Items.ContainsKey(item.PartOf))
                    startingText = pCurrentFile.Items[item.PartOf];

                /* If the item to be found has the value %FILENAME% in the
                 * PartOf element, then use the filename to pull the item
                 * value. */

                else if (item.PartOf == "%FILENAME%")
                    startingText = pCurrentFile.WorkingFilePath.StripPathAndExtension();

                /* If the previous two items failed then we should use the
                 * whole document as our starting text to find the item value. */

                else { startingText = CurrentTextVersion; }

                if ((start.Length > 0) && (end.Length > 0))
                {
                    itemToAdd = RegExHelper.FindItemRange(startingText, start, end, frontTrim, endTrim, item.Critical).Trim();
                }
                else if (start.Length > 0)
                {
                    itemToAdd = RegExHelper.FindItemCollapsed(startingText, start, frontTrim, endTrim, item.Critical).Trim();
                }
                else { errorsFound = true; }

                if (item.Critical && (itemToAdd == null || itemToAdd.Length == 0)) { errorsFound = true; }
                else 
                {
                    itemToAdd = itemToAdd.TrimInternal(item.RemoveInternal);
                    itemToAdd = itemToAdd.Trim(item.RemoveEnds);
                    itemToAdd.Trim();
                    pCurrentFile.Items.Add(item.Name, itemToAdd); 
                }
            }
            // Return the opposite of the errors found flag
            return !errorsFound;
        }

        private string GetSetupErrorDocPath(string pCurrentFile)
        {
            if (pCurrentFile.Length > 0)
            {
                // Store the new filename in the current single document object
                StringBuilder filePath = new StringBuilder();
                filePath.Append(Path.GetDirectoryName(pCurrentFile));
                filePath.Append(@"\ErrorFiles\");
                if (!Directory.Exists(filePath.ToString()))
                    Directory.CreateDirectory(filePath.ToString());
                filePath.Append(Path.GetFileName(pCurrentFile));
                return filePath.ToString();
            }
            return "";
        }

        private string GetProcessedDocPath(SingleDocument pCurrentFile, bool pGetErrorPath)
        {
            string saveFileName;
            if (pCurrentFile.DocType.Rename && !pGetErrorPath)
            {
                StringBuilder newFileName = new StringBuilder();
                foreach (string part in pCurrentFile.DocType.NewFileNameParts.Split(new Char[] { '|' }))
                {
                    newFileName.Append(pCurrentFile.Items[part].Replace("/", ""));
                }

                pCurrentFile.ProcessedFileName = newFileName.ToString();
                newFileName.Insert(0, pCurrentFile.ProcessedFilePath);
                newFileName.Append(".");
                newFileName.Append(pCurrentFile.DocType.NewFileExtension);
                saveFileName = newFileName.ToString();
            }
            else if (!pGetErrorPath)
            {
                // Store the new filename in the current single document object
                pCurrentFile.ProcessedFileName = 
                    pCurrentFile.WorkingFilePath.StripPathAndExtension();
                saveFileName = pCurrentFile.ProcessedFilePath +
                               pCurrentFile.WorkingFilePath.StripPathAndExtension() +
                               "." + pCurrentFile.DocType.NewFileExtension;
            }
            else
            {
                // Send back the error path for this file
                pCurrentFile.ProcessedFileName = 
                    Path.GetFileNameWithoutExtension(pCurrentFile.WorkingFilePath);
                saveFileName = pCurrentFile.ErrorFilePath + Path.GetFileName(pCurrentFile.WorkingFilePath);
            }

            string fPath;
            string fName;
            string fExt;

            int nDuplicateCount = 1;

            while (File.Exists(saveFileName))
            {
                fPath = Path.GetDirectoryName(saveFileName);
                fName = Path.GetFileNameWithoutExtension(saveFileName);
                fExt = Path.GetExtension(saveFileName);

                saveFileName = fPath + "\\" + fName + "_" + nDuplicateCount.ToString() + fExt;
                nDuplicateCount++;
            }

            return saveFileName;
        }

        public void BuildHL7()
        {
            try
            {
                Controller.Instance.RaiseShowMessage(this,
                       new MessagingEventArgs("Creating HL7 Xml document"));

                HL7Charts allCharts = new HL7Charts();
                string trimmedValue;
                string pathToStore;
                var documents = from single in SingleDocuments
                                where single.Ignore == false
                                select single;

                foreach (var value in documents)
                {
                    HL7Chart newChart = new HL7Chart();
                    foreach (string key in Config.HL7defaults.Keys)
                    {
                        // First address environment variables

                        /* The %PATH% variable is intended to create the new file
                         * path for import based on the current working path, filename
                         * and the configured new file extension. */

                        if (Config.HL7defaults[key].Equals("%PATH%"))
                        {   
                            StringBuilder filepath = new StringBuilder();
                            filepath.Append(Path.GetDirectoryName(value.ProcessedFilePath));
                            filepath.Append(@"\");
                            filepath.Append(value.ProcessedFileName);
                            filepath.Append(".");
                            filepath.Append(Config.HL7PathExtension);
                            newChart.RouteValue(key, filepath.ToString());                            
                        }

                        /* The %DOCTYPE% variable will return the evaluated document
                         * type as determined by reading the configuration file */

                        else if (Config.HL7defaults[key].Equals("%DOCTYPE%"))
                        {
                            newChart.RouteValue(key, value.DocType.Description.ToUpper());
                        }
                        else if (Config.HL7defaults[key].Contains("\""))
                        {
                            if (key == "Notes.Path")
                            {
                                StringBuilder filename = new StringBuilder();
                                filename.Append(value.ProcessedFileName);
                                filename.Append(".");
                                filename.Append(Config.HL7PathExtension);

                                StringBuilder filepath = new StringBuilder();
                                filepath.Append(Config.HL7defaults[key].Replace("\"", ""));
                                if (!filepath.ToString().EndsWith(@"\"))
                                {
                                    filepath.Append(@"\");
                                }

                                newChart.RouteValue(key, filepath.Append(filename.ToString()).ToString());
                            }
                            else
                            {
                                trimmedValue = Config.HL7defaults[key].Replace("\"", "");
                                newChart.RouteValue(key, trimmedValue);
                            }
                        }
                        else
                        {
                            newChart.RouteValue(key, value.Items[Config.HL7defaults[key]]);
                        }
                    }

                    newChart.SectionNotes.AddNewPageToPages();
                    allCharts.AddNew(newChart);
                }

                allCharts.CreateFinalXML(ImportFromParentPath + @"\HL7Import.xml");

                Controller.Instance.RaiseShowMessage(this,
                       new MessagingEventArgs("Completed HL7 Xml document"));
            }

            catch (KeyNotFoundException missingKey)
            {
                Controller.Instance.RaiseLogError(new LogErrorEventArgs(missingKey, "The default item could not be found", "HL7 xml", true));
            }
        }

        private void ConvertPatientChartsToText()
        {
            if (ChooseFolderToImport())
            {
                // Clear any previous text conversions from that directory
                var error = from dir in Directory.GetDirectories(ImportFromParentPath, "TextVersions", SearchOption.AllDirectories)
                            select dir;
                foreach (var dir in error)
                {
                    if (VerifyAndClearDirectory(dir))
                        Directory.Delete(dir);
                }

                var AllFiles = from file in Directory.GetFiles(ImportFromParentPath, "*", SearchOption.TopDirectoryOnly)
                               where Path.GetFileName(file).ToUpper().Contains(".DOC")
                               where ((File.GetAttributes(file) & FileAttributes.Hidden) != FileAttributes.Hidden)
                               orderby file
                               select file;

                _totalProcessCount = AllFiles.Count();
                foreach (var fullPath in AllFiles)
                {
                    ImportFromPath = fullPath.ExtractPath();

                    // Open the word document that will be processed in this pass
                    ms.SetOriginalDocument(fullPath);
                    // Convert the file you just opened
                    ConvertWordDocToText(fullPath);
                    // Close the word document that was just processed
                    ms.CloseOriginalDocumentWithoutSaving();
                    // Update the forms status count
                    Controller.Instance.RaiseShowStatus(++_currentProcessCount, _totalProcessCount);
                }
            }
            else
            {
                Controller.Instance.RaiseShowMessage(this,
                    new MessagingEventArgs("Process is ending because you did not choose a folder to process"));
            }
        }

        public void ConvertWordDocToText(string pFileToConvert)
        {
            //Setup and start the xml file listing the charts
            StringBuilder filepath = new StringBuilder();
            filepath.Append(ImportFromPath);
            filepath.Append(@"\TextVersions\");

            // create the text directory if it does not exist
            if (!Directory.Exists(filepath.ToString()))
                Directory.CreateDirectory(filepath.ToString());

            filepath.Append(pFileToConvert.StripPathAndExtension());
            filepath.Append(@".txt");
            // Send an update message to the user interface
            Controller.Instance.RaiseShowCurrentFile(Path.GetFileName(pFileToConvert));
            try
            {
                StreamWriter sw = new StreamWriter(filepath.ToString());
                string convertedDoc = ms.ConvertDocToText(pFileToConvert);
                sw.Write(convertedDoc);
                sw.Close();
            }
            catch
            {
                Controller.Instance.RaiseShowMessage(this,
                    new MessagingEventArgs(string.Format("An error occurred while extracting text from {0}", pFileToConvert)));
            }
        }

        private void ConvertGroupDocsToTif()
        {
            if (ChooseFolderToImport())
            {
                // Clear any previous text conversions from that directory
                var error = from dir in Directory.GetDirectories(ImportFromParentPath, "TifVersions", SearchOption.AllDirectories)
                            select dir;
                foreach (var dir in error)
                {
                    if (VerifyAndClearDirectory(dir))
                        Directory.Delete(dir);
                }

                var AllFiles = from file in Directory.GetFiles(ImportFromParentPath, "*", SearchOption.TopDirectoryOnly)
                               where Path.GetFileName(file).ToUpper().Contains(".DOC")
                               where ((File.GetAttributes(file) & FileAttributes.Hidden) != FileAttributes.Hidden)
                               orderby file
                               select file;

                _totalProcessCount = AllFiles.Count();
                foreach (var fullPath in AllFiles)
                {
                    ImportFromPath = fullPath.ExtractPath();
                    ConvertDocToTif(fullPath);

                    // Update the forms status count
                    Controller.Instance.RaiseShowStatus(++_currentProcessCount, _totalProcessCount);
                }
            }
            else
            {
                Controller.Instance.RaiseShowMessage(this,
                    new MessagingEventArgs("Process is ending because you did not choose a folder to process"));
            }
        }

        public void ConvertDocToTif(string pFileToConvert)
        {
            //Setup and start the xml file listing the charts
            StringBuilder filepath = new StringBuilder();
            filepath.Append(ImportFromPath);
            filepath.Append(@"\TifVersions\");

            // create the text directory if it does not exist
            if (!Directory.Exists(filepath.ToString()))
                Directory.CreateDirectory(filepath.ToString());

            filepath.Append(Path.GetFileNameWithoutExtension(pFileToConvert));
            filepath.Append(@".tif");
            // Send an update message to the user interface
            Controller.Instance.RaiseShowCurrentFile(Path.GetFileNameWithoutExtension(pFileToConvert));
            ms.ConvertDocToTiff(pFileToConvert, filepath.ToString());
        }

        public bool ChooseFolderToImport()
        {
            FolderBrowserDialog folderBrowse = new FolderBrowserDialog();
            folderBrowse.ShowNewFolderButton = false;
            folderBrowse.Description = "Choose the folder containing all of the charts to import";

            // Display the openFile dialog.
            DialogResult result = folderBrowse.ShowDialog();

            // OK button was pressed.
            if (result == DialogResult.OK)
            {
                ImportFromParentPath = folderBrowse.SelectedPath;

                return true;
                //return ConfirmProcessFolder();
            }
            // Cancel button was pressed.
            else { return false; }
        }

        private static bool ConfirmProcessFolder()
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("The directory you chose will be used as a parent directory to process from.\r");
            message.AppendLine("All document files with a '.DOC' extension will be processed.\rPlease make sure that you have not placed any files in this directory or any other directory contained in this one that you do not wish to run through this process.  Each directory one level down will be processed in addition to the parent directory you have chosen.\r\r");
            message.AppendLine("\tPress the 'CANCEL' button now to halt the process.\r\tPress the 'OK' button now to continue the process.");
            string caption = "Warning";

            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult deleteResult;
            // Displays the MessageBox.
            deleteResult = MessageBox.Show(message.ToString(), caption, buttons);

            if (deleteResult == System.Windows.Forms.DialogResult.OK)
            {
                // The user accepted the delete notification - PROCEED
                return true;
            }
            // The user did not accept the delete notification - STOP THE PROCESS
            return false;
        }

        private void ResetDirectoriesForProcess(string ParentDir)
        {
            /* Each parent directory and subdirectory will automatically
                 * clear out ALL files within the directory at the time the
                 * path is set.  A warning will pop up at the beginning when 
                 * the user first selects the parent directory. */

            var processed = from dir in Directory.GetDirectories(ParentDir, "ProcessedFiles", SearchOption.AllDirectories)
                            select dir;
            foreach (var dir in processed)
            {
                if (VerifyAndClearDirectory(dir))
                    Directory.Delete(dir);
            }

            var work = from dir in Directory.GetDirectories(ParentDir, "WorkingFiles", SearchOption.AllDirectories)
                       select dir;
            foreach (var dir in work)
            {
                if (VerifyAndClearDirectory(dir))
                    Directory.Delete(dir);
            }

            var error = from dir in Directory.GetDirectories(ParentDir, "ErrorFiles", SearchOption.AllDirectories)
                        select dir;
            foreach (var dir in error)
            {
                if (VerifyAndClearDirectory(dir))
                    Directory.Delete(dir);
            }
        }

        private bool VerifyAndClearDirectory(string pDirectoryToClear)
        {
            try
            {
                // Get all of the files in the working directory.
                var allFiles = from f in Directory.GetFiles(pDirectoryToClear)
                               select f;

                if (allFiles.Count() > 0)
                {
                    // delete the files
                    foreach (string f in allFiles)
                    {
                        File.Delete(f);
                    }
                    return true;
                }
                return true;
            }

            catch (UnauthorizedAccessException pEx)
            {
                Controller.Instance.RaiseLogError(new LogErrorEventArgs(pEx, "An error occured while clearing processing directories", "", true));
                return false;
            }

            catch (IOException pEx)
            {
                Controller.Instance.RaiseLogError(new LogErrorEventArgs(pEx, "File is in use. Can not delete now.", "", true));
                return false;
            }
        }

        public void CleanUp()
        {
            ms.CleanMe();
        }

    }

    public class SingleDocument
    {
        string _workingFilePath;
        public string WorkingFilePath
        {
            get { return _workingFilePath; }
            set { _workingFilePath = value; }
        }

        string _processedFileName;
        public string ProcessedFileName
        {
            get { return _processedFileName; }
            set { _processedFileName = value; }
        }

        private string _processedFilePath;
        public string ProcessedFilePath
        {
            get { return _processedFilePath; }
            set
            {
                _processedFilePath = value;

                // Create the directory if it doesn't exist
                if (!Directory.Exists(_processedFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(_processedFilePath);
                    }

                    catch (IOException ioEx)
                    {
                        MessageBox.Show("An error occurred while creating the new document directory" +
                                        ioEx.Message);
                    }
                }
            }
        }

        private string _errorFilePath;
        public string ErrorFilePath
        {
            get { return _errorFilePath; }
            set
            {
                _errorFilePath = value;

                // Create the directory if it doesn't exist
                if (!Directory.Exists(_errorFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(_errorFilePath);
                    }

                    catch (IOException ioEx)
                    {
                        MessageBox.Show("An error occurred while creating the new document directory" +
                                        ioEx.Message);
                    }
                }
            }
        }

        bool _ignore;
        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }

        DocumentType _docType;
        public DocumentType DocType
        {
            get { return _docType; }
            set { _docType = value; }
        }

        Dictionary<String, String> _items;
        public Dictionary<String, String> Items
        {
            get 
            {
                if (_items == null)
                {
                    _items = new Dictionary<string, string>();
                }
                return _items; 
            }
            set { _items = value; }
        }
    }
}