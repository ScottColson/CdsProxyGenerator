using System.Collections.Generic;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    interface IProxySettingsService
    {
        IEnumerable<IConfigSettings> LoadSettings(string path);
    }
}
