using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace APS.Lib.Helper
{
    public static class Logger
    {
        private static StreamWriter _streamWriter;
        private static string _directory;
        private static DateTime _streamDay;

        public static void Init(string directory)
        {
            _directory = directory;
        }

        public static void LogE(string message)
        {
            Log("ERR ", message);
        }

        public static void LogW(string message)
        {
            Log("WARN", message);
        }
        public static void LogI(string message)
        {
            Log("INFO", message);
        }

        private static void Log(string type, string message)
        {
            var now = DateTime.Now;
            try
            {
                if (_streamDay < now.Date)
                {
                    Close();

                    var fi = GetFI(now.Date);

                    if (!fi.Directory.Exists)
                    {
                        fi.Directory.Create();
                    }
                    _streamWriter = new StreamWriter(fi.FullName, true, Encoding.UTF8);
                    _streamDay = now.Date;
                }
            }
            catch (Exception) { }

            try
            {
                if (_streamWriter != null)
                {
                    _streamWriter.WriteLine($"[{now.ToString("yyyy-MM-dd HH:mm:ss")}][{type}]:{message}");
                    _streamWriter.Flush();
                }
            }
            catch (Exception) { }
        }

        public static void Close()
        {
            try
            {
                if (_streamWriter != null)
                {
                    try
                    {
                        _streamWriter.Flush();
                    }
                    catch (Exception) { }

                    _streamWriter.Close();
                    _streamWriter = null;
                }
            }
            catch (Exception) { }
        }

        private static FileInfo GetFI(DateTime date)
        {
            return new FileInfo(Path.Combine(_directory, "APS_log_" + date.ToString("yyyy-MM-dd") + ".txt"));
        }
    }
}
