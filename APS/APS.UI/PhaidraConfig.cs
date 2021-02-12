using APS.Lib.Helper;
using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace APS.UI
{
    public class PhaidraConfig
    {
        private static PhaidraConfig _instance;
        private SolidColorBrush _mappedColorBrush;

        public string Url { get; set; }
        public string SearchEngineUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool AutoLogon { get; set; }
        public string CollectionRoot { get; set; }
        public bool SimulateUpload { get; set; }
        public bool WriteDebugFiles { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue((string)null)]
        public string MappedColor { get; set; }

        [JsonIgnore]
        public IBrush MappedColorBrush
        {
            get
            {
                if (_mappedColorBrush == null)
                {

                    byte r = 0xAB;
                    byte g = 0xAB;
                    byte b = 0xAB;
                    try
                    {
                        if (!string.IsNullOrEmpty(MappedColor) && MappedColor.Length == 7)
                        {
                            r = byte.Parse(MappedColor.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                            g = byte.Parse(MappedColor.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                            b = byte.Parse(MappedColor.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                        }
                    }
                    catch { }
                    _mappedColorBrush = new SolidColorBrush(new Color(0xFF, r, g, b));
                }
                return _mappedColorBrush;
            }
        }

        public static PhaidraConfig Instance
        {
            get
            {
                return (_instance ?? (_instance = CreateInstance()));
            }
        }

        public static void Save()
        {
            try
            {
                string configFileName = GetConfigFilename();
                string configJson = JsonConvert.SerializeObject(Instance, Formatting.Indented);
                System.IO.File.WriteAllText(configFileName, configJson);
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in PhaidraConfig.Save: {ex.ToString()}");
            }
        }

        private static PhaidraConfig CreateInstance()
        {
            try
            {
                string configFileName = GetConfigFilename();

                if (System.IO.File.Exists(configFileName))
                {
                    string configJson = System.IO.File.ReadAllText(configFileName);
                    return JsonConvert.DeserializeObject<PhaidraConfig>(configJson);
                }
            }
            catch (Exception ex)
            {
                Logger.LogE($"Error in PhaidraConfig loading: {ex.ToString()}");
            }
            return new PhaidraConfig();
        }

        private static string GetConfigFilename()
        {
            string configFileName = System.IO.Path.Combine(new System.IO.FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "APS.UI.Config.json");
            return configFileName;
        }
    }
}
