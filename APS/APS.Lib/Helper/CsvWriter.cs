using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace APS.Lib.Helper
{
    public class CsvWriter : IDisposable
    {
        private FileInfo _csvFile;
        private string _columnSeparator;
        private string _rowSeparator;
        private FileStream _stream;
        private StreamWriter _streamWriter;

        public CsvWriter(string csvFile, string columnSeparator, string rowSeparator)
        {
            _csvFile = new System.IO.FileInfo(csvFile);
            _columnSeparator = columnSeparator;
            _rowSeparator = rowSeparator;
            if (!_csvFile.Directory.Exists)
            {
                _csvFile.Directory.Create();
            }
            _stream = new FileStream(_csvFile.FullName, FileMode.Create);
            _stream.SetLength(0);
            _streamWriter = new StreamWriter(_stream, Encoding.UTF8);
        }

        public void Dispose()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Dispose();
                _streamWriter = null;
            }
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }
        }

        public void WriteLine(params object[] columns)
        {
            if (columns == null) return;

            _streamWriter.Write(string.Join(_columnSeparator, columns.Select(i => $"\"{GetValue(i)}\"")) + _rowSeparator);
        }

        private string GetValue(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            if (obj is string)
            {
                return ((string)obj).Replace("\"", "\"\"");
            }
            if (obj is DateTime)
            {
                return ((DateTime)obj).ToString("yyyy-MM-ddTHH:mm:ss");
            }

            return obj.ToString();
        }
    }
}
