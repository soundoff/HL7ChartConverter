using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartConverter
{
    public class ModelChangeEventArgs : EventArgs
    {
        public string _message;

        private ModelChangeEventArgs() { }
        public ModelChangeEventArgs(string pMessage)
        {
            this._message = pMessage;
        }
    }

    public class LogErrorEventArgs : EventArgs
    {
        private Exception _exception;
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _errorFile;
        public string ErrorFile
        {
            get { return _errorFile; }
            set { _errorFile = value; }
        }

        private bool _critical;
        public bool Critical
        {
            get { return _critical; }
            set { _critical = value; }
        }

        public LogErrorEventArgs(Exception pException,
                                 string pDescription,
                                 string pErrorFile,
                                 bool pCriticalError)
        {
            this.Exception = pException;
            this.Description = pDescription;
            this.ErrorFile = pErrorFile;
            this.Critical = pCriticalError;
        }
    }

    public class MessagingEventArgs : EventArgs
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public MessagingEventArgs(string pDescription)
        {
            this.Message = pDescription;
        }
    }
}
