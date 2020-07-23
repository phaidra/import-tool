using System.Collections.Generic;

namespace APS.Lib.Import
{
    public class FindCollectionResult
    {
        public bool Success { get; set; }
        public long NumFound { get; set; }

        public List<string> FoundPids { get; set; }

        public FindCollectionResult()
        {
            FoundPids = new List<string>();
        }
    }
}