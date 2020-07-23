using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class CollectionPhaidraAttribute : PhaidraAttribute
    {
        private const string CONST_collection = "collection";

        public override string Name => "Collection";

        public override Dictionary<string, string> Fields
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { CONST_collection, "Collection structure" }
                };
            }
        }

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            string collection = GetStringValue(values, CONST_collection);
            if (!string.IsNullOrEmpty(collection))
            {

                file.CollectionPath = collection.Replace("\\", "/");
            }
            else
            {
                file.Errors.Add("no collection set");
            }
        }

        public override bool OnlyOneAllowed => true;

        public override bool IsRequired => true;
        public override bool IsSystem => true;

    }
}
