using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class PhaidraFile
    {
        public JObject Metadata { get; set; }
        public string Fullfilename { get; set; }
        public string MimeType { get; set; }
        public string PhaidraType { get; set; }
        public string CollectionPath { get; set; }
        public string BaseDirectory { get; set; }
        public List<string> Errors { get; set; }
        public string PhaidraId { get; set; }
        public Dictionary<int, string> CsvRow { get; set; }

        public PhaidraFile()
        {
            Metadata = new JObject();
            Errors = new List<string>();
            MimeType = "application/octet-stream";
        }
    }
}
