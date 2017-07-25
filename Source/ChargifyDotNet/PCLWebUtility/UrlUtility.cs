using System;
using System.Collections.Generic;
using System.Text;

namespace PCLWebUtility
{
    internal class UrlUtility
    {
        public UrlUtility()
        {
        }

        public static string UrlDecode(string text)
        {
            text = text.Replace("+", " ");
            return Uri.UnescapeDataString(text);
        }

        public static string UrlEncode(string text)
        {
            return Uri.EscapeDataString(text);
        }
    }
}
