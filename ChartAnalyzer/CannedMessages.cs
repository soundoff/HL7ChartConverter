using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartConverter
{
    public static class CannedMessages
    {
        public static string MovingFile(string FromFile, string ToFile)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Moving");
            message.Append("\t");
            message.AppendLine(FromFile);
            message.AppendLine("\t\tinto directory:");
            message.Append("\t");
            message.AppendLine(ToFile);
            return message.ToString();
        }

        public static string TextFollowedByFilePath(string FreeForm, string FilePath)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(FreeForm);
            message.Append("\t");
            message.AppendLine(FilePath);
            return message.ToString();
        }
    }
}
