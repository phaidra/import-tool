using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace APS.Lib.Models.PhaidraAttributes
{
    public class KeywordsAttribute : PhaidraAttribute
    {
        public const string CONST_Keyword = "Stichwort";
        public const string CONST_KeywordLang = "Language";
        public static string[] CONST_SplitChars = new string[] { "," };

        public override string Name => "Stichwort";

        public override Dictionary<string, string> Fields => new Dictionary<string, string>()
                {
                    { CONST_Keyword, "Stichwort" },
                    { CONST_KeywordLang, "Sprache" }
                };

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            var jsonLd = GetJsonLd(file.Metadata);

            string keywordStr = GetStringValue(values, CONST_Keyword);

            if (!string.IsNullOrEmpty(keywordStr))
            {
                var keywords = keywordStr.Split(CONST_SplitChars, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();

                JArray subjectArray = GetOrCreateProperty<JArray>(jsonLd, "dce:subject");
                foreach (var keyword in keywords)
                {
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        string language = GetStringValue(values, CONST_KeywordLang);

                        var prefLabelObj = new JObject(
                                new JProperty("@value", keyword)
                                );

                        if (!string.IsNullOrEmpty(language))
                        {
                            prefLabelObj.Add("@language", language);
                        }

                        subjectArray.Add(new JObject(
                            new JProperty("skos:prefLabel", new JArray(prefLabelObj)),
                            new JProperty("@type", "skos:Concept")));
                    }
                }
            }
        }
    }
}
