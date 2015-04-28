using System;
using System.IO;

namespace ChargifyConsoleSample
{
    /// <summary>
    /// SimpleLog just writes a text log file, and writes the time for every entry.
    /// </summary>
    public class SimpleLog
    {
        private string sLogFormat;

        private string sErrorTime;
        public SimpleLog()
        {
            sLogFormat = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " ==> ";
            string sYear = System.DateTime.Now.Year.ToString("0000");
            string sMonth = System.DateTime.Now.Month.ToString("00");
            string sDay = System.DateTime.Now.Day.ToString("00");
            sErrorTime = sYear + sMonth + sDay;
        }

        public void Write(string sPathName, string sErrorMsg, bool useLogFormat)
        {
            StreamWriter sw = new StreamWriter(sPathName + sErrorTime + ".txt", true);
            if (useLogFormat)
            {
                sw.WriteLine(sLogFormat + sErrorMsg);
            }
            else
            {
                sw.WriteLine(sErrorMsg);
            }
            sw.Flush();
            sw.Close();
        }
    }
}
