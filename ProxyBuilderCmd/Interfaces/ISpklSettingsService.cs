using System.Collections.Generic;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    using CCLLC.CDS.ProxyGenerator;
    using Spkl;

    public interface ISpklSettingsService
    {
        ISettings LoadSettings(string path);
    }
}
