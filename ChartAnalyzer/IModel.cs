using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartConverter
{
    public interface IModel
    {
        void ProcessAllDocuments(object sender, Action action);
        void ConvertWordDocToText(string fileToProcess);
        void CleanUp();
    }
}
