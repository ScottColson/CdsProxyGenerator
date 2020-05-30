using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CCLLC.CDS.ProxyBuilderCmd.Spkl
{
    public class ConfigFile
    {       
        public List<EarlyBoundTypeConfig> earlyboundtypes;
        
        [JsonIgnore]
        public string filePath;

        public virtual void Save()
        {
            var file = Path.Combine(filePath, "spkl.json");
            if (File.Exists(file))
            {
                File.Copy(file, file + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak");
            }
            File.WriteAllText(file, Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        }
    
        public virtual EarlyBoundTypeConfig[] GetEarlyBoundConfig(string profile)
        {
            if (earlyboundtypes == null)
                return new EarlyBoundTypeConfig[] { new EarlyBoundTypeConfig(){                    
                    entities = "account,contact",
                    classNamespace = "Proxy"
                } };

            EarlyBoundTypeConfig[] config = null;
            if (profile == "default")
            {
                profile = null;
            }
            if (profile != null)
            {
                config = earlyboundtypes.Where(c => c.profile != null && c.profile.Replace(" ", "").Split(',').Contains(profile)).ToArray();
            }
            else
            {
                // Default profile or empty
                config = earlyboundtypes.Where(c => c.profile == null || c.profile.Replace(" ", "").Split(',').Contains("default") || String.IsNullOrWhiteSpace(c.profile)).ToArray();
            }

            return config;
        }

    }

   
}
