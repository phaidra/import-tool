using APS.Lib;
using APS.Lib.Helper;
using APS.Lib.Import;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace APS
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MainAsync(args).Wait();
            }
            catch (Exception ex)
            {

            }
        }

        private static void TemplateAttributeTest()
        {
            var templateAttributes = PhaidraAttribute.GetTemplatePhaidraAttributes();

            foreach (var attribute in templateAttributes)
            {
                switch (attribute.Predicate)
                {
                    case "dcterms:created":
                        {
                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Date", "2020" }
                            });
                        }
                        break;
                    case "role:aut":
                        {
                            /*
                            +		[0]	{[Nachname, Nachname]}	System.Collections.Generic.KeyValuePair<string, string>
                            +		[1]	{[Vorname, Vorname]}	System.Collections.Generic.KeyValuePair<string, string>
                            +		[2]	{[Wikidata-ID, Wikidata-ID]}	System.Collections.Generic.KeyValuePair<string, string>
                            +		[3]	{[Affiliation, Affiliation]}	System.Collections.Generic.KeyValuePair<string, string>
                             */
                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Vorname", "Wolfgang Amadeus" },
                                { "Nachname", "Mozart" },
                                { "Wikidata-ID", "Q254" },
                                { "Affiliation", "Institut für Kunstgeschichte" },
                                { "Bla", "Institut für Kunstgeschichte" },
                            });

                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Vorname", "Wolfgang Amadeus" },
                            });

                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Nachname", "Mozart" },
                            });

                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Wikidata-ID", "Q254" },
                            });

                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Affiliation", "Institut für Kunstgeschichte" },
                            });

                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Vorname", "Wolfgang Amadeus" },
                                { "Nachname", "Mozart" },
                            });

                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Vorname", "Wolfgang Amadeus" },
                                { "Nachname", "Mozart" },
                                { "Wikidata-ID", "Q254" },
                            });
                        }
                        break;
                    case "dce:title":
                        {
                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Haupttitel", "test-titel" }
                            });
                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Haupttitel", "test-titel2" },
                                { "Untertitel", "test-titel2" }
                            });
                            ShowCreatedMetadata(attribute, new Dictionary<string, string>
                            {
                                { "Untertitel", "sub-test-titel" }
                            });
                        }
                        break;
                }
            }
        }

        private static void ShowCreatedMetadata(PhaidraAttribute attribute, Dictionary<string, string> values)
        {
            PhaidraFile phaidraFile = new PhaidraFile();
            attribute.WriteAttributeContent(phaidraFile, values);
            Console.WriteLine($"Attribute {attribute.Name}:");
            Console.WriteLine(phaidraFile.Metadata.ToString());
        }

        static async Task MainAsync(string[] args)
        {
            //JsonTest();
            TemplateAttributeTest();
            return;
            //Console.WriteLine("Hello World!");

            //string url = "https://services.phaidra-sandbox.univie.ac.at/api/";
            //string searchEngineUrl = "https://app01.cc.univie.ac.at:8983/solr/phaidra_sandbox/";
            ////string url = "http://localhost:8888/api/";

            ////url = "http://10.13.0.138/bla/api/";

            //string Username = "gutlebeni65";
            //string Password = "EXk2ZJ0Ful6SWxshMVjX";

            //PhaidraClient client = new PhaidraClient(url, searchEngineUrl, Username, Password);

            ////string csvPath = @"N:\Kunden\Kunst Universität Graz\Testdateien\GS Metadaten_Ethnomusikologie.csv";
            //string csvPath = @"N:\Kunden\Kunst Universität Graz\Testdateien\unix_lines.csv";

            //var csvReader = new CsvReader2(csvPath, ";", "\r\n");
            //var columns = csvReader.ReadHeader();







            //string collectionMetadataJson = System.IO.File.ReadAllText(@"C:\GutlebenSystems\PHAIDRA-Schnittstelle\APS\APS.Lib\PhaidraAttributeFiles\create-collection-base.json");

            //JObject collectionMetadataObj = (JObject)JsonConvert.DeserializeObject(collectionMetadataJson);

            //var collectionNameToken = collectionMetadataObj.SelectToken("metadata.json-ld.dce:title[0].bf:mainTitle[0].@value") as JValue;

            //if (collectionNameToken != null)
            //{
            //    collectionNameToken.Value = "My Collection";
            //}

            //var jsonLdToken = collectionMetadataObj.SelectToken("metadata.json-ld") as JObject;

            //if (jsonLdToken != null)
            //{
            //    jsonLdToken.Add("phaidra:systemTag", new JArray(new object[] { "/test1/test2/test4" }));
            //}

            //collectionMetadataJson = JsonConvert.SerializeObject(collectionMetadataObj, Formatting.Indented);

            //var result = await client.CreateCollectionWithMetadata(collectionMetadataJson);

            //string pidToDelete = result.CreatedPid;
            //var uploadResult = await client.UploadFileWithJsonMetadata(@"C:\tmp\Kunstuni_bsp_API.png", System.IO.File.ReadAllText(@"C:\tmp\Kunstuni_bsp_API.png.json-ld"));

            //string pidToDelete = uploadResult.CreatedPid;

            //bool deleteSuccess = await client.DeleteObject(pidToDelete);

            //var searchResult = await client.FindCollectionBySystemTag("/test1/test2");


            //var createCollectionPathResult1 = await client.CreateCollectionPath("/Collection 1/Collection 1.1/Collection 1.1.1/Collection 1.1.1.1");
            //var createCollectionPathResult2 = await client.CreateCollectionPath("/Collection 1/Collection 1.2/Collection 1.2.1/Collection 1.2.1.1");
            //var createCollectionPathResult3 = await client.CreateCollectionPath("/Collection 1/Collection 1.2/Collection 1.2.2/Collection 1.2.2.1");
            //var createCollectionPathResult4 = await client.CreateCollectionPath("/Collection 1/Collection 1.3/Collection 1.3.1/Collection 1.3.1.1");

            //var r = await client.AddCollectionMembers("o:570743", new List<string> { "o:570746" });

            var obj = JObject.Parse(@"{
                  ""@type"": ""bf:Title"",
                  ""bf:mainTitle"": [
                    {
                ""@value"": ""This is the main title"",
                      ""@language"": ""deu""
                    }
                  ],
                  ""bf:subtitle"": [
                    {
                      ""@value"": ""This is subtitle"",
                      ""@language"": ""deu""
                    }
                  ]
                }");


            //TitlePhaidraAttribute attr = new TitlePhaidraAttribute();

            //JObject metadata = new JObject(new JProperty("metadata", new JObject()));
            //attr.WriteAttributeContent(metadata, null);

        }

        private static void JsonTest()
        {
            string jsonPath = @"C:\GutlebenSystems\PHAIDRA-Schnittstelle\APS\APS.Lib\PhaidraAttributeFiles\phaidra-attribute-template.json";

            JObject templateObj = JObject.Parse(System.IO.File.ReadAllText(jsonPath));

            JObject template = templateObj.SelectToken("template") as JObject;
            if (template != null)
            {
                Console.Write("select mode (normal|extended):");
                string mode = Console.ReadLine();
                if (mode == "normal")
                {
                    while (true)
                    {
                        Console.WriteLine("select via path");
                        string tokenpath = Console.ReadLine();
                        if (tokenpath == "exit")
                        {
                            break;
                        }
                        try
                        {
                            JToken token = template.SelectToken(tokenpath);
                            if (token != null)
                            {
                                Console.WriteLine("token found. Content:");
                                Console.WriteLine("-----");
                                Console.WriteLine(token.ToString());
                                Console.WriteLine("-----");
                            }
                            else
                            {
                                Console.WriteLine("token not found");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                else if (mode == "extended")
                {
                    JToken currentToken = template;
                    bool cancelLoop = false;
                    while (!cancelLoop)
                    {
                        if (currentToken != null)
                        {
                            Console.WriteLine("currentToken:");
                            Console.WriteLine("-------");
                            Console.WriteLine(currentToken.ToString());
                            Console.WriteLine("-------");
                        }
                        Console.Write("type (0=select, 1=filterarray):");
                        string type = Console.ReadLine();

                        switch (type)
                        {
                            case "0":
                            case "select":
                                Console.Write("path: ");
                                string path = Console.ReadLine();
                                try
                                {
                                    currentToken = currentToken.SelectToken(path);
                                }
                                catch (Exception ex)
                                {
                                    currentToken = null;
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            case "1":
                            case "filterarray":
                                if (currentToken is JArray currentArray)
                                {
                                    Console.Write("attribute name: ");
                                    string attributeName = Console.ReadLine();
                                    Console.Write("filter value: ");
                                    string filterValue = Console.ReadLine();
                                    foreach (var token in currentArray)
                                    {
                                        if (token is JObject currentObj)
                                        {
                                            if (currentObj.ContainsKey(attributeName))
                                            {
                                                JValue currentValue = currentObj.SelectToken(attributeName) as JValue;
                                                if (currentValue != null && filterValue.Equals(currentValue.Value<string>(), StringComparison.OrdinalIgnoreCase))
                                                {
                                                    currentToken = currentObj;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
