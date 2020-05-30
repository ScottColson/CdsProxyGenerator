using Microsoft.Xrm.Sdk;

namespace CCLLC.CDS.ProxyGenerator
{
    public interface ICDSMetadataServiceFactory
    {
        ICDSMetadataService Create(ISettings settings);
    }
}
