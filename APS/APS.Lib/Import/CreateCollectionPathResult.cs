using System.Collections.Generic;

namespace APS.Lib.Import
{
    public class CreateCollectionPathResult
    {
        public Dictionary<string,string> Collections { get; set; }
        public Dictionary<string, string> CreatedCollections { get; set; }
        public string CollectionPathPid { get; internal set; }

        public CreateCollectionPathResult()
        {
            Collections = new Dictionary<string, string>();
            CreatedCollections = new Dictionary<string, string>();
        }
    }
}