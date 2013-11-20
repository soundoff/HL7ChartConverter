using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace ChartConverter
{
    static class RegExHelper
    {
        public static Boolean ValidateDocumentType(this string pTextToSearch, 
                                                   string pStartRegEx,
                                                   string pEndRegEx)
        {
            RegexOptions options = RegexOptions.IgnoreCase;
            Regex r1 = new Regex(pStartRegEx, options);
            bool test1 = r1.IsMatch(pTextToSearch);
            Regex r2 = new Regex(pEndRegEx, options);
            bool test2 = r2.IsMatch(pTextToSearch);
            return (r1.IsMatch(pTextToSearch) && r2.IsMatch(pTextToSearch));
        }

        public static Boolean ValidateDocumentType(this string pTextToSearch,
                                                   string pStartRegEx)
        {
            RegexOptions options = RegexOptions.IgnoreCase;
            Regex r1 = new Regex(pStartRegEx, options);
            bool test1 = r1.IsMatch(pTextToSearch);
            return (r1.IsMatch(pTextToSearch));
        }

        public static List<int> FindDocumentStarts(this string TextToSearch, string pRegExToUse)
        {
            Regex r;
            int startPoint;
            List<int> selectStarts = new List<int>();


            r = new Regex(pRegExToUse);
            MatchCollection matches = r.Matches(TextToSearch);
            foreach (Match m in matches)
            {
                startPoint = m.Index;
                selectStarts.Add(startPoint);
            }

            return selectStarts;
        }

        public static List<int> FindDocumentEnds(this string pTextToSearch, string pRegExToUse)
        {
            Regex r;
            int startPoint;
            int length;
            int endPoint;
            List<int> selectEnds = new List<int>();


            r = new Regex(pRegExToUse);
            MatchCollection matches = r.Matches(pTextToSearch);
            foreach (Match m in matches)
            {
                startPoint = m.Index;
                length = m.Length;
                endPoint = startPoint + length;
                selectEnds.Add(endPoint);
            }

            return selectEnds;
        }

        /*
        public static string FindItemCollapsed(this string pTextToSearch, string pRegExToUse)
        {
            Regex r;
            string _item;
            
            r = new Regex(pRegExToUse);
            Match m = r.Match(pTextToSearch);
            _item = m.ToString();

            return _item;
        }
        */

        public static string FindItemCollapsed(this string pTextToSearch, 
                                                    string pRegExToUse,
                                                    int pFrontTrim,
                                                    int pEndTrim,
                                                    bool pCritical)
        {
            try
            {
                Regex r;
                string _item;
                RegexOptions options = RegexOptions.IgnoreCase;
                r = new Regex(pRegExToUse, options);
                Match m = r.Match(pTextToSearch);

                if (m.Success)
                {
                    _item = m.ToString();
                    return _item.Substring(pFrontTrim, _item.Length - pEndTrim - pFrontTrim);
                }
                else
                {
                    Controller.Instance.RaiseLogError(new LogErrorEventArgs(null,
                        "Item was unable to be found using the following parameters:" +
                        "\r\n\tText to search: " + pTextToSearch +
                        "\r\n\tRegex to use: " + pRegExToUse +
                        "\r\n\tFront trim: " + pFrontTrim +
                        "\r\n\tEnd trim: " + pEndTrim, "", pCritical));
                    return "";
                }
            }

            catch (Exception ex)
            {
                Controller.Instance.RaiseLogError(new LogErrorEventArgs(ex,
                                                      "A regular expression search failed.", "", true));
                return null;
            }
        }

        public static string FindItemRange(this string pTextToSearch,
                                           string pStartRegEx,
                                           string pEndRegEx,
                                           int pFrontTrim,
                                           int pEndTrim,
                                           bool pCritical)
        {
            try
            {
                Regex r1;
                Regex r2;
                string _item;
                int _startPoint;
                int _endPoint;

                r1 = new Regex(pStartRegEx);
                r2 = new Regex(pEndRegEx);
                Match m1 = r1.Match(pTextToSearch);
                Match m2 = r2.Match(pTextToSearch);

                if (m1.Success && m2.Success)
                {
                    _startPoint = m1.Index;
                    _endPoint = m2.Index + m2.Length;
                    _item = pTextToSearch.Substring(_startPoint, _endPoint - _startPoint);
                    return _item.Substring(pFrontTrim, _item.Length - pEndTrim - pFrontTrim); 
                }
                else
                {
                    Controller.Instance.RaiseLogError(new LogErrorEventArgs(null,
                        "Item was unable to be found using the following parameters:" +
                        "\r\n\tText to search: " + pTextToSearch +
                        "\r\n\tStart Regex to use: " + pStartRegEx +
                        "\r\n\tEnd Regex to use: " + pEndRegEx +
                        "\r\n\tFront trim: " + pFrontTrim +
                        "\r\n\tEnd trim: " + pEndTrim, "", pCritical));
                    return "";
                }
            }
            catch (Exception ex)
            {
                Controller.Instance.RaiseLogError(new LogErrorEventArgs(ex,
                                                      "A regular expression search failed.", "", true));
                return null;
            }
        }

        /*
        public static R Execute<T1, T2, T3, T4, T5, R>(Expression<Func<T1, T2, T3, T4, T5, R>> exp,
                                     T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return exp.Compile()(param1, param2, param3, param4, param5);
        }
        */
        public static R Execute<T1, T2, T3, T4, R>(Expression<Func<T1, T2, T3, T4, R>> exp,
                                     T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return exp.Compile()(param1, param2, param3, param4);
        }

        public static R Execute<T1, T2, T3, R>(Expression<Func<T1, T2, T3, R>> exp,
                                     T1 param1, T2 param2, T3 param3)
        {
            return exp.Compile()(param1, param2, param3);
        }

        public static R Execute<T1, T2, R>(Expression<Func<T1, T2, R>> exp,
                                        T1 param1, T2 param2)
        {
            return exp.Compile()(param1, param2);
        }
    }
}
