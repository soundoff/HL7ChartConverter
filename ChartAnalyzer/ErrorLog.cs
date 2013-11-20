using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace ChartConverter
{
    public class ErrorDocumentation : IError
    {
        private StreamWriter _errorLog;
        public StreamWriter ErrorLog
        {
            get  { return _errorLog; }
            private set { _errorLog = value; }
        }

        private int _criticalErrCount;
        public int CriticalErrCount
        {
            get { return _criticalErrCount; }
            set { _criticalErrCount = value; }
        }

        private int _nonCriticalErrCount;
        public int NonCriticalErrCount
        {
            get { return _nonCriticalErrCount; }
            set { _nonCriticalErrCount = value; }
        }



        public ErrorDocumentation(string pBaseFilePath)
        {
            // Create the error file
            ErrorLog = CreateNewErrorFile(pBaseFilePath);
            Controller.Instance.RegisterErrorLog(this);
        }

        private StreamWriter CreateNewErrorFile(string pBaseFilePath)
        {
            string dt = DateTime.Now.ToShortDateString()
                .Replace(@"/",@"").Replace(@"\",@"");
            string tm = DateTime.Now.ToLongTimeString()
                .Replace(@":", @"").Replace(@" ", @"");

            StringBuilder logFile = new StringBuilder();
            logFile.Append(dt).Append(@"_").Append(tm).Append(@".log");

            FileStream fs = new FileStream(pBaseFilePath + logFile.ToString(),
                FileMode.CreateNew, FileAccess.Write, FileShare.None);

            StreamWriter swFromFileStream = new StreamWriter(fs);

            return swFromFileStream;
        }

        public void CloseErrorLog()
        {
            if (ErrorLog != null)
            {
                WriteCounts();
                ErrorLog.Flush();
                ErrorLog.Close();
                ErrorLog = null;
            }
        }

#region IError Members

        public void WriteError(LogErrorEventArgs e)
        {
            if (e.Critical)
            {
                StringBuilder sb = new StringBuilder();
                if (e.Exception != null) { sb.AppendLine(e.Exception.ToString()); }
                sb.AppendLine("\t");
                sb.Append(e.Description);
                sb.AppendLine("\t");
                if (e.ErrorFile.Length > 0)
                    sb.Append(e.ErrorFile);
                sb.AppendLine("");
                sb.AppendLine("");
                ErrorLog.WriteLine(sb.ToString());

                CriticalErrCount++;
            }
            else
                NonCriticalErrCount++;
        }

        private void WriteCounts()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("");
            sb.Append("Critical Error Count: ");
            sb.Append(CriticalErrCount.ToString());
            sb.AppendLine("");
            sb.Append("Non-Critical Error Count: ");
            sb.Append(NonCriticalErrCount.ToString());
            ErrorLog.WriteLine(sb.ToString());
        }

        public void ShowError(string message)
        {
            throw new NotImplementedException();
        }

#endregion
    }
}
