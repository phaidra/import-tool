using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class AssociationPhaidraAttribute : PhaidraAttribute
    {
        private const string CONST_organizationname = "organizationname";

        public override string Name => "Association";


        public override Dictionary<string, string> Fields
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { CONST_organizationname, "Organization name" }
                };
            }
        }

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            string organization = GetStringValue(values, CONST_organizationname);

            var jsonLd = GetJsonLd(file.Metadata);

            if (!string.IsNullOrEmpty(organization))
            {
                JArray associationArray = GetOrCreateProperty<JArray>(jsonLd, "rdax:P00009");

                JObject associationObj = new JObject
                    (
                        new JProperty("@type", "foaf:Organization"),
                        new JProperty("skos:prefLabel", new JArray(new JObject(new JProperty("@value", organization), new JProperty("@language", "deu"))))
                    );

                if (PhaidraAttributesCache.OrganizationName2OrganizazionDict.TryGetValue(organization, out JObject orgObj))
                {
                    JValue idVal = orgObj.SelectToken("id") as JValue;
                    //JValue oracleIdVal = orgObj.SelectToken("oracle_id") as JValue;

                    if (idVal != null)
                    {
                        associationObj.Add(new JProperty("skos:exactMatch", new JArray(idVal.Value<string>())));
                    }
                }
                associationArray.Add(associationObj);

            }
            /*
             
          "rdax:P00009": [
            {
              "@type": "foaf:Organization",
              "skos:prefLabel": [
                {
                  "@value": "Institut 13 Ethnomusikologie",
                  "@language": "deu"
                }
              ],
              "skos:exactMatch": [
                "https://pid.phaidra.org/kug-org/N3RC-0W64"
              ]
            }
          ]
             
             */
        }

        public override bool OnlyOneAllowed => true;
        public override bool IsSystem => true;

    }
}
