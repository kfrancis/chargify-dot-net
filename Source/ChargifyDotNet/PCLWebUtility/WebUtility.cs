using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PCLWebUtility
{
    /// <summary>
    /// Internal copy of PCLWebUtility which isn't available in netstandard. Working on 
    /// getting the author to hopefully move his code so I can add netstandard compat.
    /// </summary>
    internal static class WebUtility
    {
        public static string HtmlDecode(string value)
        {
            return ExtendedHtmlUtility.HtmlEntityDecode(value);
        }

        public static void HtmlDecode(string value, TextWriter output)
        {
            output.Write(ExtendedHtmlUtility.HtmlEntityDecode(value));
        }

        public static string HtmlEncode(string value)
        {
            return ExtendedHtmlUtility.HtmlEntityEncode(value);
        }

        public static void HtmlEncode(string value, TextWriter output)
        {
            output.Write(ExtendedHtmlUtility.HtmlEntityEncode(value));
        }

        public static string UrlDecode(string encodedValue)
        {
            return UrlUtility.UrlDecode(encodedValue);
        }

        public static string UrlEncode(string value)
        {
            return UrlUtility.UrlEncode(value);
        }
    }
}
