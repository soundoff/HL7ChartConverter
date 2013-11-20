using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartConverter
{
    public interface IView
    {
        void ShowMessage(object sender, MessagingEventArgs e);
        void DisplayStartTime(DateTime start);
        void DisplayFinishTime(DateTime finish);
        void DisplayStatusCount(int current, int total);
        void DisplayCurrentFile(string filename);
    }
}
