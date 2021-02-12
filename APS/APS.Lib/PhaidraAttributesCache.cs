using APS.Lib.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public static class PhaidraAttributesCache
    {
        public static Dictionary<string, string> Extension2MimeTypeDict { get; set; }
        public static Dictionary<string, string> MimeType2PhaidraTypeDict { get; set; }
        public static Dictionary<string, JArray> PhaidraType2DctermsTypeDict { get; set; }
        public static Dictionary<string, JObject> OrganizationName2OrganizazionDict { get; set; }

        static PhaidraAttributesCache()
        {
            Extension2MimeTypeDict = new Dictionary<string, string>();
            MimeType2PhaidraTypeDict = new Dictionary<string, string>();
            PhaidraType2DctermsTypeDict = new Dictionary<string, JArray>();
            OrganizationName2OrganizazionDict = new Dictionary<string, JObject>();
        }

        public static void Init(string rootDir)
        {
            try
            {
                {
                    string file = System.IO.Path.Combine(rootDir, "PhaidraAttributeFiles", "extension2mimetype.json");
                    JObject obj = JObject.Parse(System.IO.File.ReadAllText(file));

                    foreach (JProperty prop in obj.Properties())
                    {
                        Extension2MimeTypeDict[prop.Name] = prop.Value.Value<string>();
                    }
                }
                {
                    string file = System.IO.Path.Combine(rootDir, "PhaidraAttributeFiles", "mimetype2phaidratype.json");
                    JObject obj = JObject.Parse(System.IO.File.ReadAllText(file));

                    foreach (JProperty prop in obj.Properties())
                    {
                        MimeType2PhaidraTypeDict[prop.Name] = prop.Value.Value<string>();
                    }
                }
                {
                    string file = System.IO.Path.Combine(rootDir, "PhaidraAttributeFiles", "phaidratype2dctermstype.json");
                    JObject obj = JObject.Parse(System.IO.File.ReadAllText(file));

                    foreach (JProperty prop in obj.Properties())
                    {
                        PhaidraType2DctermsTypeDict[prop.Name] = prop.Value as JArray;
                    }
                }
                {
                    string file = System.IO.Path.Combine(rootDir, "PhaidraAttributeFiles", "list_json_types_kug-org.json");
                    JArray objArray = JArray.Parse(System.IO.File.ReadAllText(file));

                    foreach (JObject obj in objArray)
                    {
                        var prop = obj.Property("de");
                        OrganizationName2OrganizazionDict[prop.Value.Value<string>()] = obj;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in PhaidraAttributesCache.Init: {ex.ToString()}");
            }
        }
    }
}
