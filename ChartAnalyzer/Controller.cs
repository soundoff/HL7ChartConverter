using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartConverter
{
    public sealed class Controller
    {
        #region Singleton
        private static readonly Controller instance = new Controller();
        private Controller() { }
        public static Controller Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        private IModel _processModel;

        public void WireUp()
        {
            _processModel = new Charts();
        }

        private IError _errorLog;
        public IError ErrorLog
        {
            get { return _errorLog; }
            set { _errorLog = value; }
        }

        public void StartErrorLogging(string pBaseDir)
        {
            ErrorLog = new ErrorDocumentation(pBaseDir);
        }

        public void StopErrorLogging()
        {
            ErrorLog.CloseErrorLog();
        }

#region Model Change
        public delegate void ModelChangeDelegate(object sender, ModelChangeEventArgs e);
        public event ModelChangeDelegate ModelChangeEvent;

        public void RegisterView(IView view)
        {
            this.ShowMessageEvent += new ShowMessageDelegate(view.ShowMessage);
            this.ShowStartTimeEvent += new ShowStartTimeDelegate(view.DisplayStartTime);
            this.ShowFinishTimeEvent += new ShowFinishTimeDelegate(view.DisplayFinishTime);
            this.ShowCurrentFileEvent += new ShowCurrentFileDelegate(view.DisplayCurrentFile);
            this.ShowStatusEvent += new ShowStatusDelegate(view.DisplayStatusCount);
        }

        public void UnregisterView(IView view)
        {
            this.ShowMessageEvent -= new ShowMessageDelegate(view.ShowMessage);
            this.ShowStartTimeEvent -= new ShowStartTimeDelegate(view.DisplayStartTime);
            this.ShowFinishTimeEvent -= new ShowFinishTimeDelegate(view.DisplayFinishTime);
            this.ShowCurrentFileEvent -= new ShowCurrentFileDelegate(view.DisplayCurrentFile);
            this.ShowStatusEvent -= new ShowStatusDelegate(view.DisplayStatusCount);
        }

        public void RaiseModelChange(object sender, ModelChangeEventArgs e)
        {
            if (ModelChangeEvent != null)
                ModelChangeEvent(sender, e);
        }
#endregion

#region Error Logging
        public delegate void LogErrorDelegate(LogErrorEventArgs e);
        public event LogErrorDelegate LogErrorEvent;

        public void RegisterErrorLog(IError error)
        {
            this.LogErrorEvent += new LogErrorDelegate(error.WriteError);
        }

        public void UnregisterErrorLog(IError error)
        {
            this.LogErrorEvent -= new LogErrorDelegate(error.WriteError);
        }

        public void RaiseLogError(LogErrorEventArgs e)
        {
            if (LogErrorEvent != null)
                LogErrorEvent(e);
        }
#endregion

        #region Update Status
        public delegate void ShowStartTimeDelegate(DateTime start);
        public event ShowStartTimeDelegate ShowStartTimeEvent;

        public void RaiseShowStartTime(DateTime start)
        {
            if (ShowStartTimeEvent != null)
                ShowStartTimeEvent(start);
        }

        public delegate void ShowFinishTimeDelegate(DateTime finish);
        public event ShowFinishTimeDelegate ShowFinishTimeEvent;

        public void RaiseShowFinishTime(DateTime finish)
        {
            if (ShowFinishTimeEvent != null)
                ShowFinishTimeEvent(finish);
        }

        public delegate void ShowCurrentFileDelegate(String filename);
        public event ShowCurrentFileDelegate ShowCurrentFileEvent;

        public void RaiseShowCurrentFile(String filename)
        {
            if (ShowCurrentFileEvent != null)
                ShowCurrentFileEvent(filename);
        }

        public delegate void ShowStatusDelegate(int current, int total);
        public event ShowStatusDelegate ShowStatusEvent;

        public void RaiseShowStatus(int current, int total)
        {
            if (ShowStatusEvent != null)
                ShowStatusEvent(current, total);
        }
        #endregion

#region Show Messages
        public delegate void ShowMessageDelegate(object sender, MessagingEventArgs e);
        public event ShowMessageDelegate ShowMessageEvent;

        public void RaiseShowMessage(object sender, MessagingEventArgs e)
        {
            if (ShowMessageEvent != null)
                ShowMessageEvent(sender, e);
        }
#endregion

#region View Change
        public delegate void BeginProcessingDelegate(object sender, Action action);
        public event BeginProcessingDelegate BeginProcessingEvent;

        public delegate void CleanUpDelegate();
        public event CleanUpDelegate CleanUpEvent;

        public void RegisterModel(IModel model)
        {
            this.BeginProcessingEvent += new BeginProcessingDelegate(model.ProcessAllDocuments);
            this.CleanUpEvent += new CleanUpDelegate(model.CleanUp);
        }

        public void UnregisterModel(IModel model)
        {
            this.BeginProcessingEvent -= new BeginProcessingDelegate(model.ProcessAllDocuments);
            this.CleanUpEvent -= new CleanUpDelegate(model.CleanUp);
        }

        public void RaiseBeginProcessing(object sender, Action action)
        {
            if (BeginProcessingEvent != null)
                BeginProcessingEvent(sender, action);
        }

        public void RaiseCleanUpEvent()
        {
            if (CleanUpEvent != null)
                CleanUpEvent();
        }
#endregion
    }
}
