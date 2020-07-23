using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using APS.Lib.Models.PhaidraAttributes;

namespace APS.Lib
{
    public abstract class PhaidraAttribute
    {
        private static List<PhaidraAttribute> _phaidraAttributes;

        public abstract string Name { get; }

        public override string ToString()
        {
            return $"{Name}";
        }

        public abstract void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values);

        public virtual void WriteAttributeContentToCSV(PhaidraFile file, Dictionary<string, string> values)
        {

        }

        public virtual bool ValidateMappings(PhaidraMapping mapping)
        {
            return true;
        }

        public abstract Dictionary<string, string> Fields { get; }

        public virtual bool OnlyOneAllowed => false;

        public virtual bool IsRequired => false;

        public virtual bool IsSystem => false;

        protected string GetStringValue(Dictionary<string, string> values, string key)
        {
            if (!values.ContainsKey(key))
            {
                return null;
            }
            return values[key];
        }

        protected JObject GetJsonLd(JObject metadata)
        {
            var metadataProp = GetOrCreateProperty<JObject>(metadata, "metadata");
            var jsonLdProp = GetOrCreateProperty<JObject>(metadataProp, "json-ld");
            return jsonLdProp;
        }

        protected TPropertyValue GetOrCreateProperty<TPropertyValue>(JObject obj, string propertyName) where TPropertyValue : JToken, new()
        {
            var subObj = obj.SelectToken(propertyName) as TPropertyValue;
            if (subObj == null)
            {
                subObj = new TPropertyValue();
                obj.Add(new JProperty(propertyName, subObj));
            }
            return subObj;
        }

        public static List<PhaidraAttribute> GetPhaidraAttributes()
        {
            if (_phaidraAttributes == null)
            {
                _phaidraAttributes = new List<PhaidraAttribute>();
                _phaidraAttributes.Add(new FilenamePhaidraAttribute());
                _phaidraAttributes.Add(new CollectionPhaidraAttribute());
                _phaidraAttributes.Add(new AssociationPhaidraAttribute());
                _phaidraAttributes.Add(new AccessRestrictionPhaidraAttribute());
                //_phaidraAttributes.Add(new TitlePhaidraAttribute());
                //_phaidraAttributes.Add(new DescriptionPhaidraAttribute());
                //_phaidraAttributes.Add(new DateArrayPhaidraAttribute("Creation date", "Creation date", "dcterms:created"));
                //_phaidraAttributes.Add(new DateArrayPhaidraAttribute("Date of production", "Date of production", "rdau:P60071"));
                //_phaidraAttributes.Add(new RolePhaidraAttribute("Photographer", "role:pht"));
                //_phaidraAttributes.Add(new RolePhaidraAttribute("Author", "role:aut"));
                //_phaidraAttributes.Add(new LicensePhaidraAttribute());
                //_phaidraAttributes.Add(new ObjectPhaidraAttribute("Keywords", "Keywords", "dce:subject", "skos:Concept"));
                //_phaidraAttributes.Add(new ObjectPhaidraAttribute("Provenance", "Provenance", "dcterms:provenance", "dcterms:ProvenanceStatement"));
                _phaidraAttributes.Add(new PhaidraIdPhaidraAttribute());
                _phaidraAttributes.Add(new OwnerPhaidraAttribute());
                _phaidraAttributes.Add(new KeywordsAttribute());


                var templatePhaidraAttributes = GetTemplatePhaidraAttributes();

                if (templatePhaidraAttributes.Count > 0)
                {
                    _phaidraAttributes.AddRange(templatePhaidraAttributes);
                }
            }
            return _phaidraAttributes;
        }

        public static List<TemplatePhaidraAttribute> GetTemplatePhaidraAttributes()
        {
            var attributes = new List<TemplatePhaidraAttribute>();
            var attributeFilesDir = new System.IO.DirectoryInfo(System.IO.Path.Combine(new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory.FullName, "PhaidraAttributeFiles", "AttributeTemplates"));

            if (attributeFilesDir.Exists)
            {
                var orderedFiles = attributeFilesDir.GetFiles("*.json").OrderBy(i => i.Name);
                foreach (var attributeFile in orderedFiles)
                {
                    if (!attributeFile.Name.StartsWith("x_"))
                    {
                        var attrs = TemplatePhaidraAttribute.GetTemplatePhaidraAttributes(attributeFile.FullName);
                        foreach (var attr in attrs)
                        {
                            if (attr.IsValid)
                            {
                                attributes.Add(attr);
                            }
                        }
                    }
                }
            }

            return attributes;
        }
    }
}
