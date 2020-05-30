using CCLLC.CDS.ProxyGenerator;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    public class ConfigSettings : Settings, IConfigSettings
    {
        public string ConfigurationPath { get; set; }
    }
}
