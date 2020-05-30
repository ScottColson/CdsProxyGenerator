using System.Collections.Generic;

namespace CCLLC.CDS.ProxyBuilderCmd
{

    public interface ISpklSettingsService
    {
        IEnumerable<IConfigSettings> LoadSettings(string path);
    }
}
