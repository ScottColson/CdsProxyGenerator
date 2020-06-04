using CCLLC.Core;

namespace CCLLC.CDS.ProxyGenerator
{
    public class CDSMetadataServiceFactory : ICDSMetadataServiceFactory
    {
        private readonly ICache Cache;

        public CDSMetadataServiceFactory(ICache cache)
        {
            this.Cache = cache;
        }
        public ICDSMetadataService Create(ISettings settings)
        {
            return new CDSMetadataService(settings, Cache);            
        }
    }
}
