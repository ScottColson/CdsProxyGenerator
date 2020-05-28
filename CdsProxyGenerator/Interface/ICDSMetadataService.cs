using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;

namespace CCLLC.CDS.ProxyGenerator
{
    public interface ICDSMetadataService : IMessageProvider
    {
        IEnumerable<EntityMetadata> GetEntityMetadata();
    }
}
