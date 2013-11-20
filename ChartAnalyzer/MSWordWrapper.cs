using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace ChartConverter
{
    public sealed class MSWord
    {
        /// <summary>
        /// Singleton Pattern
        /// </summary>
        private static readonly MSWord instance = new MSWord();

        /// <summary>
        /// Constructor for MSWord.  Creates an MS Word object
        /// to use for this application.  The Singleton Pattern
        /// takes care of creating the class object.
        /// </summary>
        private MSWord()
        {
            WordApp = new Application();
        }

        /// <summary>
        /// This is the property call that all other classes will use
        /// to get the ONE instance of this MS Word class.  The entire
        /// application will use the same MS Word application object.
        /// </summary>
        public static MSWord Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Used for null values when calling MS Word COM's
        /// </summary>
        private object nullobj = System.Reflection.Missing.Value;

        /// <summary>
        /// An instance of the MS Word application
        /// </summary>
        private Application _wordApp;
        public Application WordApp
        {
            get { return _wordApp; }
            private set { _wordApp = value; }
        }

        /// <summary>
        /// The all charts file currently being processed
        /// </summary>
        private Document _originalDocument;
        public Document OriginalDocument
        {
            get 
            {
                _originalDocument.Activate();
                return _originalDocument; 
            }
            private set
            {

                _originalDocument = value;
                _originalDocument.Activate();
                // start out with the new document referencing the original
                _newDocument = _originalDocument;
                DocumentUnprotect(_originalDocument, "");
            }
        }

        /// <summary>
        /// The new file opened for inserting a single chart
        /// </summary>
        private Document _newDocument;
        public Document NewDocument
        {
            get 
            {
                _newDocument.Activate();
                return _newDocument; 
            }
            private set
            {
                _newDocument = value;
                _newDocument.Activate();
            }
        }

        private Selection _originalDocumentSelection;
        public Selection OriginalDocumentSelection
        {
            get
            {
                _originalDocumentSelection = OriginalDocument.ActiveWindow.Selection;
                return _originalDocumentSelection;
            }
        }

        private Selection _newDocumentSelection;
        public Selection NewDocumentSelection
        {
            get
            {
                if (_newDocumentSelection == null)
                {
                    _newDocumentSelection = NewDocument.ActiveWindow.Selection;
                }
                return _newDocumentSelection;
            }
        }

        public bool SelectSingleChart(int startPoint, int endPoint)
        {
            try
            {
                OriginalDocumentSelection.Start = startPoint;
                OriginalDocumentSelection.End = endPoint;
                return true;
            }
            catch (COMException ex)
            {
                throw;
            }
            catch (NullReferenceException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SetOriginalDocument(string FileName)
        {
            OriginalDocument = OpenImportFromFile(FileName);
        }

        public void SetSubDocument(string FileName)
        {
            NewDocument = OpenImportFromFile(FileName);
        }

        private Document OpenImportFromFile(string InFilePath)
        {
            try
            {
                Document justOpened;
                object file = (Object)InFilePath;
                WordApp.Visible = false;
                Object visible = (object)false;

                justOpened = WordApp.Documents.Open(
                                    ref file, ref nullobj, ref nullobj,
                                    ref nullobj, ref nullobj, ref nullobj,
                                    ref nullobj, ref nullobj, ref nullobj,
                                    ref nullobj, ref nullobj, ref visible,
                                    ref nullobj, ref nullobj, ref nullobj,
                                    ref nullobj);

                return justOpened;
            }
            catch (NullReferenceException ex)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public void CreateNewDocument()
        {
            try
            {
                WordApp.Visible = false;
                Object visible = (object)false;
                NewDocument = WordApp.Documents.Add(ref nullobj, ref nullobj, ref nullobj, ref visible);
                SetupNewDocument();
            }
            catch (NullReferenceException ex)
            {
                throw;
            }
            catch (COMException ex)
            {
                throw;
            }

        }

        public void SaveNewDocument(string inFileName)
        {
            try
            {
                object filename = (object)inFileName;
                object fileformat = WdSaveFormat.wdFormatDocument;
                RemovePageBreaks(NewDocument);
                NewDocument.SaveAs(ref filename, ref fileformat, ref nullobj, ref nullobj, ref nullobj,
                            ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                            ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                            ref nullobj);
            }
            catch (NullReferenceException ex)
            {
                throw;
            }
            catch (COMException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool SaveAndCloseNewDocument(string inFileName)
        {
            try
            {
                object filename = (object)inFileName;
                object fileformat = WdSaveFormat.wdFormatDocument;
                RemovePageBreaks(NewDocument);
                NewDocument.SaveAs(ref filename, ref fileformat, ref nullobj, ref nullobj, ref nullobj,
                            ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                            ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                            ref nullobj);
                NewDocument.Close(ref nullobj, ref nullobj, ref nullobj);
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }

        public void CloseOriginalDocumentWithoutSaving()
        {
            object save = (object)WdSaveOptions.wdDoNotSaveChanges;
            object format = WdOriginalFormat.wdOriginalDocumentFormat;
            try
            {
                if ( FindDocumentInWordApp(OriginalDocument.Name) != null )
                    OriginalDocument.Close(ref save, ref format, ref nullobj);
            }
            catch (COMException ex)
            {
                throw;
            }
            catch (NullReferenceException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CloseDocumentWithoutSaving(Document doc)
        {
            try
            {
                object save = (object)WdSaveOptions.wdDoNotSaveChanges;
                object format = WdOriginalFormat.wdOriginalDocumentFormat;
                doc.Close(ref save, ref format, ref nullobj);
            }
            catch (NullReferenceException ex)
            {
                throw;
            }
        }

        public bool CloseNewDocumentWithoutSaving()
        {
            try
            {
                object save = (object)WdSaveOptions.wdDoNotSaveChanges;
                if (FindDocumentInWordApp(NewDocument.Name) != null)
                    NewDocument.Close(ref save, ref nullobj, ref nullobj);
                return true;
            }

            catch (NullReferenceException ex)
            {
                throw;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        private void RemovePageBreaks(Document pInDoc)
        {
            object replaceAll = WdReplace.wdReplaceAll;
            object wrap = WdFindWrap.wdFindContinue;
            object forward = true;
            pInDoc.ActiveWindow.Selection.Find.ClearFormatting();
            pInDoc.ActiveWindow.Selection.Find.Text = "^m";

            pInDoc.ActiveWindow.Selection.Find.Replacement.ClearFormatting();
            pInDoc.ActiveWindow.Selection.Find.Replacement.Text = "";

            pInDoc.ActiveWindow.Selection.Find.Execute(
                ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                ref nullobj, ref forward, ref wrap, ref nullobj, ref nullobj,
                ref replaceAll, ref nullobj, ref nullobj, ref nullobj, ref nullobj);
        }

        private void SetupNewDocument()
        {
            NewDocument.Activate();
            PageSetup setup = NewDocument.PageSetup;
            setup.LineNumbering.Active = 0;
            setup.Orientation = WdOrientation.wdOrientPortrait;
            setup.TopMargin = ConvertInches(0.5);
            setup.BottomMargin = ConvertInches(0.5);
            setup.LeftMargin = ConvertInches(0.5);
            setup.RightMargin = ConvertInches(0.5);
            setup.Gutter = ConvertInches(0);
            setup.HeaderDistance = ConvertInches(0.5);
            setup.FooterDistance = ConvertInches(0.5);
            setup.PageWidth = ConvertInches(8.5);
            setup.PageHeight = ConvertInches(11);
            setup.FirstPageTray = WdPaperTray.wdPrinterDefaultBin;
            setup.OtherPagesTray = WdPaperTray.wdPrinterDefaultBin;
            setup.SectionStart = WdSectionStart.wdSectionNewPage;
            setup.OddAndEvenPagesHeaderFooter = 0;
            setup.DifferentFirstPageHeaderFooter = 0;
            setup.VerticalAlignment = WdVerticalAlignment.wdAlignVerticalTop;
            setup.SuppressEndnotes = 0;
            setup.MirrorMargins = 0;
            setup.TwoPagesOnOne = false;
            setup.BookFoldPrinting = false;
            setup.BookFoldRevPrinting = false;
            setup.BookFoldPrintingSheets = 1;
            setup.GutterPos = WdGutterStyle.wdGutterPosLeft;
            WordApp.ActiveDocument.PageSetup = setup;

            NewDocument.DefaultTabStop = OriginalDocument.DefaultTabStop;
            NewDocument.Paragraphs.Format = OriginalDocument.Paragraphs.Format;
        }

        private float ConvertInches(double inch)
        {
            return WordApp.InchesToPoints((float)inch);
        }

        public bool InsertTextIntoDocument()
        {
            OriginalDocument.ActiveWindow.Selection.ClearFormatting();
            OriginalDocument.ActiveWindow.Selection.Copy();
            NewDocument.ActiveWindow.Selection.ClearFormatting();
            NewDocument.ActiveWindow.Selection.Paste();
            return true;
        }

        public string ConvertDocToText(string DocToConvert)
        {
            string outText;
            Document CurrentDoc;
            object file = (object)DocToConvert;

            try
            {
                CurrentDoc = WordApp.Documents.get_Item(ref file);
                if (CurrentDoc == null)
                    CurrentDoc = OpenImportFromFile(DocToConvert);

                CurrentDoc.ActiveWindow.Selection.WholeStory();

                Selection sel = CurrentDoc.ActiveWindow.Selection;
                outText = sel.Text;
                return outText;
            }

            catch (NullReferenceException nrefx)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Document FindDocumentInWordApp(string DocumentToFind)
        {
            try
            {
                foreach (Document d in WordApp.Documents)
                {
                    d.Activate();
                    if (DocumentToFind == d.Name)
                        return d;
                }
                return null;
            }
            catch (COMException ex)
            {
                throw;
            }
            catch (NullReferenceException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Opens a specified Word document and converts it to a .tif file in the output location.
        /// </summary>
        /// <param name="InputFileName"></param>
        /// <param name="OutputFilename"></param>
        /// <returns>boolean</returns>
        public bool ConvertDocToTiff(string InputFileName, string OutputFilename)
        {
            object file = (object)InputFileName;
            Document CurrentDoc;

            try
            {
                CurrentDoc = FindDocumentInWordApp (InputFileName);
                if (CurrentDoc == null)
                    CurrentDoc = OpenImportFromFile(InputFileName);

                CurrentDoc.Activate();
                WordApp.ActivePrinter = "Microsoft Office Document Image Writer";
                object Background = true;
                object Range = WdPrintOutRange.wdPrintAllDocument;
                object OutputFile = OutputFilename;
                object Copies = 1;
                object PageType = WdPrintOutPages.wdPrintAllPages;
                object PrintToFile = true;
                object Collate = false;
                object ManualDuplexPrint = false;
                object PrintZoomColumn = 1;
                object PrintZoomRow = 1;

                CurrentDoc.PrintOut(ref Background, ref nullobj, ref Range, ref OutputFile,
                    ref nullobj, ref nullobj, ref nullobj, ref Copies,
                    ref nullobj, ref PageType, ref PrintToFile, ref Collate,
                    ref nullobj, ref ManualDuplexPrint, ref PrintZoomColumn,
                    ref PrintZoomRow, ref nullobj, ref nullobj);

                return true;

            }
            catch (NullReferenceException ex)
            {
                throw;
            }
            catch (COMException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string sMODIKey = @"HKEY_CURRENT_USER\Software\Microsoft\Office\11.0\MODI\MDI writer";

        public void SetMODISaveLocation(string pTifSavePath)
        {
            Microsoft.Win32.Registry.SetValue(sMODIKey, "DefaultFolder", pTifSavePath);
        }

        private void SetMODIKeys()
        {
            Microsoft.Win32.Registry.SetValue(sMODIKey, "COMPIMGEMF", 1);
            Microsoft.Win32.Registry.SetValue(sMODIKey, "PrivateFlags", 17);
            Microsoft.Win32.Registry.SetValue(sMODIKey, "Public_PaperSize", 1);
            Microsoft.Win32.Registry.SetValue(sMODIKey, "Public_Orientation", 2);
            Microsoft.Win32.Registry.SetValue(sMODIKey, "TIFDPI", 200);
            Microsoft.Win32.Registry.SetValue(sMODIKey, "OpenInMODI", 0);
        }

        private void DocumentUnprotect(Document DocToUnlock, string Password)
        {
            try
            {
                if (DocToUnlock.ProtectionType != WdProtectionType.wdNoProtection)
                {
                    object password = (object)Password;
                    DocToUnlock.Unprotect(ref password);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CleanMe()
        {
            try
            {
                if (WordApp != null)
                {
                    object save = (object)WdSaveOptions.wdDoNotSaveChanges;
                    object format = WdOriginalFormat.wdOriginalDocumentFormat;
                    foreach (Document d in WordApp.Documents)
                    {
                        d.Activate();
                        d.Close(ref save, ref format, ref nullobj);
                    }
                    WordApp.Quit(ref nullobj, ref nullobj, ref nullobj);
                }
            }
            catch
            {
            }
        }
    }
}