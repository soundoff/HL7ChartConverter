using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ChartConverter
{
    public static class StringHelper
    {
        public static string StripPathAndExtension(this string pFullPath)
        {
            return Path.GetFileNameWithoutExtension(pFullPath);
        }

        public static string ExtractPath(this string pFullPath)
        {
            return Path.GetDirectoryName(pFullPath);
        }

        public static string TrimInternal(this string OldValue, string TrimCharacters)
        {
            return Regex.Replace(OldValue, TrimCharacters, ""); 
        }
    }
}
