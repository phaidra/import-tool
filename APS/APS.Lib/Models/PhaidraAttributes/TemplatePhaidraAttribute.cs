using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using APS.Lib.Helper;

namespace APS.Lib
{
    public class TemplatePhaidraAttribute : PhaidraAttribute
    {
        private const string CONST_predicatesAttribute = "predicates";
        private const string CONST_fieldsAttribute = "fields";
        private const string CONST_templateAttribute = "template";
        private const string CONST_defaultType = "";
        private string _templateDefinitionFilename;
        private string _predicate;
        private string _displayName;
        private Dictionary<string, FieldItem> _fields;
        private Dictionary<string, string> _fieldNames;
        private JToken _templateObj;

        private static Dictionary<string, Func<string, object>> _parsers;

        public string Predicate => _predicate;

        public static List<TemplatePhaidraAttribute> GetTemplatePhaidraAttributes(string templateDefinitionFilename)
        {
            var attributes = new List<TemplatePhaidraAttribute>();

            try
            {
                List<PredicateItem> predicates = null;
                var templateObj = JObject.Parse(System.IO.File.ReadAllText(templateDefinitionFilename));
                if (templateObj.ContainsKey(CONST_predicatesAttribute))
                {
                    predicates = templateObj[CONST_predicatesAttribute].ToObject<List<PredicateItem>>();
                }

                if (predicates != null)
                {
                    JArray fields = null;

                    if (templateObj.ContainsKey(CONST_fieldsAttribute))
                    {
                        fields = templateObj[CONST_fieldsAttribute] as JArray;
                    }

                    JToken template = null;

                    if (templateObj.ContainsKey(CONST_templateAttribute))
                    {
                        template = templateObj[CONST_templateAttribute];
                    }

                    foreach (var predicate in predicates)
                    {
                        var attribute = new TemplatePhaidraAttribute();
                        attributes.Add(attribute);
                        try
                        {
                            attribute.IsValid = true;
                            attribute._predicate = predicate.predicatename;
                            attribute._displayName = predicate.displayname ?? predicate.predicatename;
                            attribute._templateDefinitionFilename = templateDefinitionFilename;

                            if (fields != null)
                            {
                                foreach (JObject field in fields)
                                {
                                    var fieldItem = field.ToObject<FieldItem>();
                                    attribute._fields.Add(fieldItem.displayname, fieldItem);
                                    attribute._fieldNames.Add(fieldItem.displayname, fieldItem.displayname);
                                }
                            }

                            if (template != null)
                            {
                                attribute._templateObj = template.DeepClone();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogE($"Error in GetTemplatePhaidraAttributes in file '{templateDefinitionFilename}': {ex.ToString()}");
                            attribute.IsValid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in GetTemplatePhaidraAttributes in file '{templateDefinitionFilename}': {ex.ToString()}");
            }

            return attributes;
        }

        public TemplatePhaidraAttribute()
        {
            _fields = new Dictionary<string, FieldItem>();
            _fieldNames = new Dictionary<string, string>();
        }

        static TemplatePhaidraAttribute()
        {
            // init parsers
            _parsers = new Dictionary<string, Func<string, object>>
            {
                { CONST_defaultType, (val) => { return val; } },
                { "datetime", (val) =>
                    {
                        if (val != null && val.Length == 8)
                        {
                            val = val.Substring(0, 4) + "-" + val.Substring(4, 2) + "-" + val.Substring(6, 2);
                        }
                        if (DateTime.TryParse(val, out DateTime date))
                        {
                            return val;
                        }
                        return null;
                    }
                },
            };
        }


        public override string Name => _displayName;

        public override Dictionary<string, string> Fields => _fieldNames;

        public bool IsValid { get; internal set; }

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            try
            {
                var jsonLd = GetJsonLd(file.Metadata);//GetOrCreateProperty<JObject>(file.Metadata, "metadata");

                JToken template = (JToken)_templateObj.DeepClone();

                List<string> usedFields = new List<string>();

                foreach (var kvp in values)
                {
                    if (!string.IsNullOrEmpty(kvp.Value))
                    {
                        //FieldItem fieldItem = _fields[kvp.Key];
                        FieldItem fieldItem = null;// _fields[kvp.Key];
                        _fields.TryGetValue(kvp.Key, out fieldItem);
                        if (fieldItem == null)
                        {
                            continue;
                        }
                        JValue val = null;
                        if (fieldItem.extendedfilter == null)
                        {
                            val = template.SelectToken(fieldItem.path) as JValue;
                        }
                        else
                        {
                            val = SelectValueTokenByExtendedFilter(fieldItem, template);
                            //JToken currentToken = template;

                            //foreach (var extendedFilterItem in fieldItem.extendedfilter)
                            //{
                            //    if (currentToken == null)
                            //    {
                            //        break;
                            //    }
                            //    switch (extendedFilterItem.type)
                            //    {
                            //        case "select":
                            //            try
                            //            {
                            //                currentToken = currentToken.SelectToken(extendedFilterItem.path);
                            //            }
                            //            catch (Exception ex)
                            //            {
                            //                currentToken = null;
                            //            }
                            //            break;
                            //        case "filterarray":
                            //            if (currentToken is JArray currentArray)
                            //            {
                            //                foreach (var token in currentArray)
                            //                {
                            //                    if (token is JObject currentObj)
                            //                    {
                            //                        if (currentObj.ContainsKey(extendedFilterItem.attribute))
                            //                        {
                            //                            JValue currentValue = currentObj.SelectToken(extendedFilterItem.attribute) as JValue;
                            //                            if (currentValue != null && extendedFilterItem.value.Equals(currentValue.Value<string>(), StringComparison.OrdinalIgnoreCase))
                            //                            {
                            //                                currentToken = currentObj;
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //            else
                            //            {
                            //                currentToken = null;
                            //            }
                            //            break;
                            //    }
                            //}

                            //val = currentToken as JValue;
                        }

                        if (val != null)
                        {
                            val.Value = _parsers.ContainsKey(fieldItem.parser) ? _parsers[fieldItem.parser](kvp.Value) : _parsers[CONST_defaultType](kvp.Value);
                            usedFields.Add(val.Path);
                        }
                    }
                }

                RemoveUnusedAttributes(template as JObject, usedFields, false);

                if (usedFields.Count > 0)
                {
                    var predicateToken = GetOrCreateProperty<JArray>(jsonLd, _predicate);
                    predicateToken.Add(template);
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in TemplatePhaidraAttribute.WriteAttributeContent in file '{_templateDefinitionFilename}': {ex.ToString()}");
            }
        }

        private JValue SelectValueTokenByExtendedFilter(FieldItem fieldItem, JToken template)
        {
            try
            {
                JToken currentToken = template;

                foreach (var extendedFilterItem in fieldItem.extendedfilter)
                {
                    if (currentToken == null)
                    {
                        break;
                    }
                    switch (extendedFilterItem.type)
                    {
                        case "select":
                            try
                            {
                                currentToken = currentToken.SelectToken(extendedFilterItem.path);
                            }
                            catch (Exception ex)
                            {
                                currentToken = null;
                            }
                            break;
                        case "filterarray":
                            if (currentToken is JArray currentArray)
                            {
                                foreach (var token in currentArray)
                                {
                                    if (token is JObject currentObj)
                                    {
                                        if (currentObj.ContainsKey(extendedFilterItem.attribute))
                                        {
                                            JValue currentValue = currentObj.SelectToken(extendedFilterItem.attribute) as JValue;
                                            if (currentValue != null && extendedFilterItem.value.Equals(currentValue.Value<string>(), StringComparison.OrdinalIgnoreCase))
                                            {
                                                currentToken = currentObj;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                currentToken = null;
                            }
                            break;
                    }
                }

                return currentToken as JValue;
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in SelectValueTokenByExtendedFilter in template file '{_templateDefinitionFilename}': {ex.ToString()}");
                return null;
            }
        }

        private void RemoveUnusedAttributes(JObject currentObject, List<string> usedFields, bool remove = true)
        {
            if (currentObject == null)
            {
                return;
            }
            if (remove == true)
            {
                foreach (var usedField in usedFields)
                {
                    if (usedField.StartsWith(currentObject.Path))
                    {
                        remove = false;
                        break;
                    }
                }
            }
            if (remove)
            {
                currentObject.Remove();
            }
            else
            {
                var properties = currentObject.Properties().ToList();
                foreach (JProperty prop in properties)
                {
                    if (prop.Value is JArray jarr)
                    {
                        var jarrItems = jarr.ToList();
                        foreach (JToken jarrItem in jarrItems)
                        {
                            if (jarrItem is JObject jarrItemobj)
                            {
                                RemoveUnusedAttributes(jarrItemobj, usedFields);
                            }
                        }
                        if (jarr.Count == 0)
                        {
                            prop.Remove();
                        }
                    }
                    if (prop.Value is JObject jobj)
                    {
                        RemoveUnusedAttributes(jobj, usedFields);
                    }
                }
            }
        }

        private class PredicateItem
        {
            public string predicatename { get; set; }
            public string displayname { get; set; }
        }

        private class FieldItem
        {
            public string displayname { get; set; }
            public string path { get; set; }
            public string parser { get; set; }

            public List<ExtendedFilterItem> extendedfilter { get; set; }

            public FieldItem()
            {
                parser = ""; // if no parser given -> string
            }
        }

        private class ExtendedFilterItem
        {
            public string type { get; set; }
            public string path { get; set; }
            public string attribute { get; set; }
            public string value { get; set; }

            /*
             
            {
              "type": "selectarray",
              "path": "skos:exactMatch"
            },
            {
              "type": "filterarray",
              "attribute": "@type",
              "value": "ids:wikidata"
            },
            {
              "type": "select",
              "path" :  "@value"
            }
          ]
             
             */
        }
    }
}
