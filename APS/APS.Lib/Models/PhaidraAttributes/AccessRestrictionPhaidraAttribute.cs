using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class AccessRestrictionPhaidraAttribute : PhaidraAttribute
    {
        private const string CONST_organizationname = "organizationname";
        public override string Name => "Access organization";

        public override Dictionary<string, string> Fields
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { CONST_organizationname  , "Organization name" }
                };
            }
        }

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            string organization = GetStringValue(values, CONST_organizationname);

            var metadata = GetOrCreateProperty<JObject>(file.Metadata, "metadata");

            if (!string.IsNullOrEmpty(organization))
            {

                if (PhaidraAttributesCache.OrganizationName2OrganizazionDict.TryGetValue(organization, out JObject orgObj))
                {
                    JValue oracleIdVal = orgObj.SelectToken("oracle_id") as JValue;

                    if (oracleIdVal != null)
                    {
                        metadata.Add(new JProperty("rights", new JObject
                            (
                                new JProperty("department", new JArray
                                (
                                    new JObject(new JProperty("value", oracleIdVal.Value<string>()))
                                )
                            ))));
                    }

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
        }

        public override bool OnlyOneAllowed => true;

        public override bool IsSystem => true;
    }
}
