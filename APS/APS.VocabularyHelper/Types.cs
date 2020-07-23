using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace APS.VocabularyHelper
{
    public class Concept
    {
        public string Url { get; set; }
        public List<DisplayItem> DisplayItems { get; set; }
        public string MemberOfUrl { get; set; }

        public Concept()
        {
            DisplayItems = new List<DisplayItem>();
        }
        public class DisplayItem
        {
            public string Value{ get; set; }
            public string Lang { get; set; }
        }

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#", IsNullable = false)]
        public partial class RDF
        {

            private RDFDescription descriptionField;

            /// <remarks/>
            public RDFDescription Description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public partial class RDFDescription
        {

            private RDFDescriptionType typeField;

            private memberOf memberOfField;

            private topConceptOf topConceptOfField;

            private inScheme inSchemeField;

            private prefLabel[] prefLabelField;

            private changeNote[] changeNoteField;

            private exactMatch exactMatchField;

            private string aboutField;

            /// <remarks/>
            public RDFDescriptionType type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "https://pid.phaidra.org/vocabulary/schema#")]
            public memberOf memberOf
            {
                get
                {
                    return this.memberOfField;
                }
                set
                {
                    this.memberOfField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public topConceptOf topConceptOf
            {
                get
                {
                    return this.topConceptOfField;
                }
                set
                {
                    this.topConceptOfField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public inScheme inScheme
            {
                get
                {
                    return this.inSchemeField;
                }
                set
                {
                    this.inSchemeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("prefLabel", Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public prefLabel[] prefLabel
            {
                get
                {
                    return this.prefLabelField;
                }
                set
                {
                    this.prefLabelField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("changeNote", Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public changeNote[] changeNote
            {
                get
                {
                    return this.changeNoteField;
                }
                set
                {
                    this.changeNoteField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public exactMatch exactMatch
            {
                get
                {
                    return this.exactMatchField;
                }
                set
                {
                    this.exactMatchField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
            public string about
            {
                get
                {
                    return this.aboutField;
                }
                set
                {
                    this.aboutField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public partial class RDFDescriptionType
        {

            private string resourceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
            public string resource
            {
                get
                {
                    return this.resourceField;
                }
                set
                {
                    this.resourceField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "https://pid.phaidra.org/vocabulary/schema#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "https://pid.phaidra.org/vocabulary/schema#", IsNullable = false)]
        public partial class memberOf
        {

            private string resourceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
            public string resource
            {
                get
                {
                    return this.resourceField;
                }
                set
                {
                    this.resourceField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class topConceptOf
        {

            private string resourceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
            public string resource
            {
                get
                {
                    return this.resourceField;
                }
                set
                {
                    this.resourceField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class inScheme
        {

            private string resourceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
            public string resource
            {
                get
                {
                    return this.resourceField;
                }
                set
                {
                    this.resourceField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class prefLabel
        {

            private string langField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang
            {
                get
                {
                    return this.langField;
                }
                set
                {
                    this.langField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class changeNote
        {

            private Description descriptionField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
            public Description Description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#", IsNullable = false)]
        public partial class Description
        {

            private comment commentField;

            private System.DateTime createdField;

            private bool createdFieldSpecified;

            private string creatorField;

            private System.DateTime modifiedField;

            private bool modifiedFieldSpecified;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
            public comment comment
            {
                get
                {
                    return this.commentField;
                }
                set
                {
                    this.commentField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/terms/")]
            public System.DateTime created
            {
                get
                {
                    return this.createdField;
                }
                set
                {
                    this.createdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool createdSpecified
            {
                get
                {
                    return this.createdFieldSpecified;
                }
                set
                {
                    this.createdFieldSpecified = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/terms/")]
            public string creator
            {
                get
                {
                    return this.creatorField;
                }
                set
                {
                    this.creatorField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://purl.org/dc/terms/")]
            public System.DateTime modified
            {
                get
                {
                    return this.modifiedField;
                }
                set
                {
                    this.modifiedField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool modifiedSpecified
            {
                get
                {
                    return this.modifiedFieldSpecified;
                }
                set
                {
                    this.modifiedFieldSpecified = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2000/01/rdf-schema#", IsNullable = false)]
        public partial class comment
        {

            private string langField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang
            {
                get
                {
                    return this.langField;
                }
                set
                {
                    this.langField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class exactMatch
        {

            private string resourceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
            public string resource
            {
                get
                {
                    return this.resourceField;
                }
                set
                {
                    this.resourceField = value;
                }
            }
        }


    }

    public class Collection
    {
        public string Name { get; set; }
        public string NameLang { get; set; }
        public string Url { get; set; }
        public List<Concept> MemberObjects { get; set; }

        [JsonIgnore]
        public List<string> Members { get; set; }

        public Collection()
        {
            Members = new List<string>();
            MemberObjects = new List<Concept>();
        }

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#", IsNullable = false)]
        public partial class RDF
        {

            private RDFDescription descriptionField;

            /// <remarks/>
            public RDFDescription Description
            {
                get
                {
                    return this.descriptionField;
                }
                set
                {
                    this.descriptionField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public partial class RDFDescription
        {

            private RDFDescriptionType typeField;

            private prefLabel prefLabelField;

            private definition definitionField;

            private member[] memberField;

            private string aboutField;

            /// <remarks/>
            public RDFDescriptionType type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public prefLabel prefLabel
            {
                get
                {
                    return this.prefLabelField;
                }
                set
                {
                    this.prefLabelField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public definition definition
            {
                get
                {
                    return this.definitionField;
                }
                set
                {
                    this.definitionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("member", Namespace = "http://www.w3.org/2004/02/skos/core#")]
            public member[] member
            {
                get
                {
                    return this.memberField;
                }
                set
                {
                    this.memberField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
            public string about
            {
                get
                {
                    return this.aboutField;
                }
                set
                {
                    this.aboutField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public partial class RDFDescriptionType
        {

            private string resourceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
            public string resource
            {
                get
                {
                    return this.resourceField;
                }
                set
                {
                    this.resourceField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class prefLabel
        {

            private string langField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang
            {
                get
                {
                    return this.langField;
                }
                set
                {
                    this.langField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class definition
        {

            private string langField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
            public string lang
            {
                get
                {
                    return this.langField;
                }
                set
                {
                    this.langField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.w3.org/2004/02/skos/core#")]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.w3.org/2004/02/skos/core#", IsNullable = false)]
        public partial class member
        {

            private string resourceField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
            public string resource
            {
                get
                {
                    return this.resourceField;
                }
                set
                {
                    this.resourceField = value;
                }
            }
        }


    }
}
