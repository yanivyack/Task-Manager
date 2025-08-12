using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogManager
{
    public static class LogManager
    {
        /// <summary>
        /// a general use class for writing logs
        /// </summary>
        private static string _logFile = "Log.txt";
        private static string DefaultPath { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _logFile); } }

        public static void LogException(Exception ex, string comments = null)
        {
            if (comments == null)
                comments = string.Empty;

            string finalMessage = ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + comments;
            WriteLog(finalMessage);
        }
        public static void WriteLog(string log)
        {
            try
            {
                Console.WriteLine(log);

                string filePath = Path.Combine(Path.GetDirectoryName(DefaultPath), _logFile);

                using (StreamWriter fs = new StreamWriter(filePath, true))
                {
                    fs.WriteLine(log + Environment.NewLine + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                }
            }
            catch (Exception)
            { }
        }
    }
}