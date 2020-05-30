using System.Collections.Generic;
using CCLLC.CDS.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CCLLC.CDS.ProxyGenerator
{
    public interface ICDSMetadataService : IMessageProvider
    {
        IEnumerable<EntityMetadata> GetEntityMetadata(IOrganizationService orgService);
        IEnumerable<SdkMessageMetadata> GetMessageMetadata(IOrganizationService orgService);
    }
}
