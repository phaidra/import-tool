using APS.Lib.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace APS.Lib
{
    public class FilenamePhaidraAttribute : PhaidraAttribute
    {
        public const string CONST_absolutefilepath = "fullfilename";
        public const string CONST_relativefilepath = "relativefilename";
        public const string CONST_absolutedirectorypath = "fulldirectorypath";
        public const string CONST_relativedirectorypath = "relativedirectorypath";
        public const string CONST_filename = "filename";
        public override string Name => "Filename";
        public override Dictionary<string, string> Fields
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { CONST_absolutefilepath, "Absolute file path" },
                    { CONST_relativefilepath, "Relative file path" },
                    { CONST_absolutedirectorypath, "Absolute directory path" },
                    { CONST_relativedirectorypath, "Relative directory path" },
                    { CONST_filename, "File name" },
                };
            }
        }

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            JObject jsonLd = GetJsonLd(file.Metadata);

            string fullPathToFile;
            string absolutefilepath = GetStringValue(values, CONST_absolutefilepath);
            string relativefilepath = GetStringValue(values, CONST_relativefilepath);
            string absolutedirectorypath = GetStringValue(values, CONST_absolutedirectorypath);
            string relativedirectorypath = GetStringValue(values, CONST_relativedirectorypath);
            string filename = GetStringValue(values, CONST_filename);

            if (!string.IsNullOrEmpty(absolutefilepath))
            {
                fullPathToFile = absolutefilepath;
            }
            else if (!string.IsNullOrEmpty(file.BaseDirectory) && !string.IsNullOrEmpty(relativefilepath))
            {
                fullPathToFile = System.IO.Path.Combine(file.BaseDirectory, absolutefilepath);
            }
            else if (!string.IsNullOrEmpty(absolutedirectorypath) && !string.IsNullOrEmpty(filename))
            {
                fullPathToFile = System.IO.Path.Combine(absolutedirectorypath, filename);
            }
            else if (!string.IsNullOrEmpty(file.BaseDirectory) && !string.IsNullOrEmpty(relativedirectorypath) && !string.IsNullOrEmpty(filename))
            {
                fullPathToFile = System.IO.Path.Combine(file.BaseDirectory, relativedirectorypath, filename);
            }
            else
            {
                file.Errors.Add("no file name specified");
                return;
            }

            file.Fullfilename = fullPathToFile;

            if (!System.IO.File.Exists(file.Fullfilename))
            {
                Logger.LogE($"File not found ({file.Fullfilename}).");
                file.Errors.Add("file not found");
                return;
                //System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(file.Fullfilename));
                //System.IO.File.WriteAllText(file.Fullfilename, "test");
            }

            filename = System.IO.Path.GetFileName(file.Fullfilename);
            jsonLd.Add(new JProperty("ebucore:filename", new JArray(filename)));

            string extension = System.IO.Path.GetExtension(filename).ToLower();

            if (PhaidraAttributesCache.Extension2MimeTypeDict.TryGetValue(extension, out string mimeType))
            {
                file.MimeType = mimeType;
                jsonLd.Add(new JProperty("ebucore:hasMimeType", new JArray(mimeType)));
                string phaidraType;
                if (!PhaidraAttributesCache.MimeType2PhaidraTypeDict.TryGetValue(mimeType, out phaidraType))
                {
                    phaidraType = PhaidraAttributesCache.MimeType2PhaidraTypeDict["default"];
                }
                file.PhaidraType = phaidraType;
                if (PhaidraAttributesCache.PhaidraType2DctermsTypeDict.TryGetValue(phaidraType, out JArray dctermsType))
                {
                    jsonLd.Add(new JProperty("dcterms:type", (JArray)dctermsType.DeepClone()));
                }
            }

            /*
              "ebucore:filename": [ "file.jpg" ],
              "ebucore:hasMimeType": [ "image/jpeg" ], 
              "dcterms:type": [
               {
                 "skos:prefLabel": [ { "@language": "eng", "@value": "image" } ],
                 "skos:exactMatch": [ "https://pid.phaidra.org/vocabulary/44TN-P1S0" ],
                 "@type": "skos:Concept"
               }
              ]             
             */
        }

        public override bool ValidateMappings(PhaidraMapping mapping)
        {
            bool hasabsolutefilepathMapping = false;
            bool hasrelativefilepathMapping = false;
            bool hasabsolutedirectorypathMapping = false;
            bool hasrelativedirectorypathMapping = false;
            bool hasfilenameMapping = false;

            foreach (var m in mapping.PhaidraAttributeFields2CsvColumns)
            {
                switch (m.Field.Key)
                {
                    case CONST_absolutefilepath:
                        if (m.SelectedCsvColumn != CsvColumn.Empty)
                        {
                            hasabsolutefilepathMapping = true;
                        }
                        break;
                    case CONST_relativefilepath:
                        if (m.SelectedCsvColumn != CsvColumn.Empty)
                        {
                            hasrelativefilepathMapping = true;
                        }
                        break;
                    case CONST_absolutedirectorypath:
                        if (m.SelectedCsvColumn != CsvColumn.Empty)
                        {
                            hasabsolutedirectorypathMapping = true;
                        }
                        break;
                    case CONST_relativedirectorypath:
                        if (m.SelectedCsvColumn != CsvColumn.Empty)
                        {
                            hasrelativedirectorypathMapping = true;
                        }
                        break;
                    case CONST_filename:
                        if (m.SelectedCsvColumn != CsvColumn.Empty)
                        {
                            hasfilenameMapping = true;
                        }
                        break;
                }
            }
            // XOR
            mapping.IsValid =
                hasabsolutefilepathMapping ||
                hasrelativefilepathMapping && hasfilenameMapping ||
                hasabsolutedirectorypathMapping && hasfilenameMapping ||
                hasrelativedirectorypathMapping && hasfilenameMapping;

            return mapping.IsValid;
        }
        public override bool OnlyOneAllowed => true;
        public override bool IsRequired => true;
        public override bool IsSystem => true;

    }
}

