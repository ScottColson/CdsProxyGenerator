
using CCLLC.CDS.ProxyGenerator;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    interface IProxySettingsService
    {
        ISettings LoadSettings(string path);
    }
}
