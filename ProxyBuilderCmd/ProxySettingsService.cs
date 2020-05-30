using CCLLC.CDS.ProxyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    public class ProxySettingsService : IProxySettingsService
    {
        private readonly IDirectoryService DirectoryService;

        public ProxySettingsService(IDirectoryService directoryService)
        {
            this.DirectoryService = directoryService ?? throw new ArgumentNullException("directoryService");
        }

        public ISettings LoadSettings(string folder)
        {
            return null;
        }
    }
}
