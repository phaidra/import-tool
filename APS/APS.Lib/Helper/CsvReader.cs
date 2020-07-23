using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace APS.Lib.Helper
{
    public class CsvReader : IDisposable
    {
        private const string cellEncolosure = "\"";
        private FileInfo _csvFile;
        private string _columnSeparator;
        private Encoding _encoding;
        private string[] _lines;

        public CsvReader(string csvFile, string columnSeparator, Encoding encoding)
        {
            _csvFile = new System.IO.FileInfo(csvFile);
            _columnSeparator = columnSeparator;
            _encoding = encoding;

            if (_csvFile.Exists)
            {
                try
                {
                    _lines = System.IO.File.ReadAllLines(_csvFile.FullName, _encoding);
                }
                catch (Exception ex)
                {
                    _lines = new string[0];
                    Logger.LogE($"Error reading csv: {ex.ToString()}");
                }
            }
        }

        public string LastError { get; private set; }

        public void Dispose()
        {

        }

        public async Task<Dictionary<int, string>> ReadHeaders()
        {
            return await Task.Run(() =>
            {
                Dictionary<int, string> columns = new Dictionary<int, string>();
                try
                {
                    var firstLine = _lines.Length > 0 ? _lines[0] : "";
                    columns = ParseLine(firstLine);
                }
                catch (Exception ex)
                {
                    Logger.LogE($"Error parsing Headers: {ex.ToString()}");
                    LastError = ex.Message;
                }
                return columns;
            });
        }

        public async Task<List<Dictionary<int, string>>> ReadRows()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var rows = new List<Dictionary<int, string>>();
                    //skip first row


                    for (int i = 1; i < _lines.Length; i++)
                    {
                        string line = _lines[i];
                        var cells = ParseLine(line);
                        rows.Add(cells);
                    }
                    return rows;
                }
                catch (Exception ex)
                {
                    Logger.LogE($"Error parsing Rows: {ex.ToString()}");
                    return new List<Dictionary<int, string>>();
                }
            });
        }

        private Dictionary<int, string> ParseLine(string line)
        {
            var columns = new Dictionary<int, string>();
            string cell = "";
            bool inQuotes = false;
            int colIndex = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (inQuotes == false && line[i] == '"' && (i + 1 == line.Length || line[i + 1] != '"'))
                {
                    inQuotes = true;
                }
                else if (inQuotes == true && line[i] == '"' && (i + 1 == line.Length || line[i + 1] != '"'))
                {
                    inQuotes = false;
                }
                else if (inQuotes == true && line[i] == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    cell += line[i];
                    i++;
                }
                else if (inQuotes == false && (line[i] == _columnSeparator[0]))
                {
                    columns.Add(colIndex, cell);
                    colIndex++;
                    cell = "";
                }
                else if (inQuotes == false && line[i] == '"')
                {
                    inQuotes = true;
                }
                else
                {
                    cell += line[i];
                }
            }
            columns.Add(colIndex, cell);

            return columns;
        }

        public static void WriteCSVToDisk(string filename, Dictionary<int, string> headers, List<Dictionary<int, string>> rows, string columnSeparator, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Join(columnSeparator, headers.Select(i => $"\"{(i.Value ?? "").Replace("\"", "\"\"")}\"")));

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(columnSeparator, row.Select(i => $"\"{(i.Value ?? "").Replace("\"", "\"\"")}\"")));
            }

            System.IO.File.WriteAllText(filename, sb.ToString(), encoding);
        }
    }
}
