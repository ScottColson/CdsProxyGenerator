using CCLLC.CDS.ProxyGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CCLLC.CDS.ProxyBuilderCmd.Spkl
{
    public class SpklSettingsService : ISpklSettingsService
    {
        private readonly IDirectoryService DirectoryService;

        public SpklSettingsService(IDirectoryService directoryService)
        {
            this.DirectoryService = directoryService ?? throw new ArgumentNullException("directoryService");
        }

        public ISettings LoadSettings(string path)
        {
            List<string> configfilePaths = null;

            // search for the spkl.json configuration file.
            if (path.EndsWith("spkl.json") && File.Exists(path))
            {
                configfilePaths = new List<string> { path };
            }
            else
            {
                configfilePaths = DirectoryService.Search(path, "spkl.json");
            }

            var configFile = configfilePaths.Where(f => f != null && !Regex.IsMatch(f, @"packages\\spkl[0-9|.]*\\tools")).FirstOrDefault();

            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(configFile));
            config.filePath = Path.GetDirectoryName(configFile);

            return config?.earlyboundtypes.FirstOrDefault() as ISettings;

        }
    }
}
