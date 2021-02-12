using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using System.Linq;
using Newtonsoft.Json;

namespace APS.VocabularyHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj2 = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(@"C:\GutlebenSystems\PHAIDRA-Schnittstelle\APS\APS.Lib\TestFiles\json-ld-05-collection-minimal.json"));


            var wc = new WebClient();
            //Console.WriteLine("Hello World!");

            //https://pid.phaidra.org/vocabulary/7E4S-MA30.rdf

            var di = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;

            var lines = File.ReadAllLines(System.IO.Path.Combine(di.FullName, "vocabulary.txt")).Where(i => !string.IsNullOrWhiteSpace(i) && !i.StartsWith("#")).Distinct().ToList();

            var orininalLines = lines.ToList();


            Dictionary<string, string> urlToJsonDict = new Dictionary<string, string>();

            Dictionary<string, Collection> collectionsDict = new Dictionary<string, Collection>();
            Dictionary<string, Concept> conceptsDict = new Dictionary<string, Concept>();


            List<string> collectionUrls = new List<string>();


            //foreach (var line in lines)
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith("#"))
                {
                    continue;
                }

                if (urlToJsonDict.ContainsKey(line))
                {
                    continue;
                }

                string rdfText = wc.DownloadString(line + ".rdf");

                if (rdfText.Contains("rdf:type rdf:resource=\"http://www.w3.org/2004/02/skos/core#Collection\""))
                {
                    var t = Get<Collection.RDF>(rdfText);

                    if (t != null)
                    {
                        var collection = new Collection();

                        if (t.Description != null)
                        {
                            collection.Url = t.Description.about;
                            if (t.Description.prefLabel != null)
                            {
                                collection.NameLang = t.Description.prefLabel.lang;  // "Resource type" string
                                collection.Name = t.Description.prefLabel.Value;  // "Resource type" string
                                //var members = new List<string>();
                                if (t.Description.member != null)
                                {
                                    foreach (var member in t.Description.member)
                                    {
                                        if (member != null)
                                        {
                                            collection.Members.Add(member.resource);
                                            lines.Add(member.resource);
                                            collectionUrls.Add(member.resource);
                                        }
                                    }
                                }
                            }
                        }
                        urlToJsonDict.Add(t.Description.about, JsonConvert.SerializeObject(collection, Formatting.Indented));
                        collectionsDict.Add(t.Description.about, collection);
                    }
                }
                else if (rdfText.Contains("rdf:type rdf:resource=\"http://www.w3.org/2004/02/skos/core#Concept\""))
                {
                    var t = Get<Concept.RDF>(rdfText);

                    var concept = new Concept();

                    if (t != null)
                    {
                        if (t.Description != null)
                        {
                            concept.Url = t.Description.about;

                            if (t.Description.memberOf != null)
                            {
                                concept.MemberOfUrl = t.Description.memberOf.resource;
                                lines.Add(t.Description.memberOf.resource);
                            }

                            if (t.Description.prefLabel != null)
                            {
                                foreach (var disp in t.Description.prefLabel)
                                {
                                    concept.DisplayItems.Add(new Concept.DisplayItem
                                    {
                                        Value = disp.Value,
                                        Lang = disp.lang
                                    });
                                }
                            }
                        }
                        urlToJsonDict.Add(t.Description.about, JsonConvert.SerializeObject(concept, Formatting.Indented));
                        conceptsDict.Add(t.Description.about, concept);
                    }

                    /*                 
                		t.Description.about	"https://pid.phaidra.org/vocabulary/44TN-P1S0"	string
		                t.Description.prefLabel[0].Value	"image"	string
		                t.Description.prefLabel[0].lang	"en"	string
		                t.Description.prefLabel[1].Value	"Bild"	string
		                t.Description.prefLabel[1].lang	"de"	string
                    */
                }
            }

            foreach (var kvp in collectionsDict)
            {
                foreach (var member in kvp.Value.Members)
                {
                    if (conceptsDict.ContainsKey(member))
                    {
                        kvp.Value.MemberObjects.Add(conceptsDict[member]);
                    }
                }
            }

            var sb = new StringBuilder();
            //foreach (var kvp in collectionsDict)
            //{
            //    sb.AppendLine($"{kvp.Key}");
            //    sb.AppendLine();
            //    sb.AppendLine($"{JsonConvert.SerializeObject(kvp.Value, Formatting.Indented)}");
            //    sb.AppendLine();
            //}
            sb.Append($"{JsonConvert.SerializeObject(collectionsDict.Values.ToList(), Formatting.Indented)}");

            System.IO.File.WriteAllText(System.IO.Path.Combine(di.FullName, $"out_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt"), sb.ToString());
        }

        private static T Get<T>(string rdfText)
        {
            T obj = default(T);
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(rdfText)))
                {
                    var xs = new XmlSerializer(typeof(T));
                    obj = (T)xs.Deserialize(ms);
                }
            }
            catch (Exception ex)
            {

            }
            return obj;
        }
    }
}
