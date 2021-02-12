using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class OwnerPhaidraAttribute : PhaidraAttribute
    {
        private const string CONST_owner = "owner";

        public override string Name => "Owner";

        public override Dictionary<string, string> Fields
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { CONST_owner, "Owner user name" }
                };
            }
        }

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            string owner = GetStringValue(values, CONST_owner);

            var metadata = GetOrCreateProperty<JObject>(file.Metadata, "metadata");

            if (!string.IsNullOrEmpty(owner))
            {
                metadata.Add(new JProperty("ownerid", owner));

                /*
                "rights": {
                     "department": [
                       {
                         "value": "12345"
                       }
                     ]
                 }
                */
            }
        }

        public override bool OnlyOneAllowed => true;
        public override bool IsSystem => true;

    }
}
