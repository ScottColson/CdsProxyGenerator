using CCLLC.CDS.ProxyGenerator;
using CCLLC.CDS.Sdk.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    public class ProxyConfigFile : IConfigSettings
    {
        public string classNamespace;
        public string templatePath;
        public string outputPath;
        public string entitiesToExclude;
        public string entitiesToInclude;
        public string actionsToExclude;
        public string actionsToInclude;
        public string targetEndpoint;
        public string targetLanguage;

        string ISettings.Namespace => classNamespace ?? "Proxy";

        string ISettings.TemplateFilePath => templatePath ?? "proxytemplate.tt";

        string ISettings.OutputPath => outputPath ?? "";

        IEnumerable<string> ISettings.EntitiesToExclude => SplitList(entitiesToExclude, "*");

        IEnumerable<string> ISettings.EntitiesToInclude => SplitList(entitiesToInclude);

        IEnumerable<string> ISettings.ActionsToInclude => SplitList(actionsToInclude);

        IEnumerable<string> ISettings.ActionsToExclude => SplitList(actionsToExclude, "*");

        eEndpoint ISettings.TargetEndPoint => targetEndpoint?.ToLower() == "webapi" ? eEndpoint.WebApi : eEndpoint.OrgService;

        eTemplalteLanguage ISettings.TemplateLanguage => eTemplalteLanguage.Csharp;

        string IConfigSettings.ConfigurationPath { get; set; }

        private IEnumerable<string> SplitList(string value, string defaultValue = "")
        {
            return value?.Split(',').Select(v => v.Trim()) ?? new string[] { defaultValue };
        }
    }
}
