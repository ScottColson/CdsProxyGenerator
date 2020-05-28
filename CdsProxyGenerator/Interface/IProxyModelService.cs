using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;

namespace CCLLC.CDS.ProxyGenerator
{
    using CCLLC.CDS.Sdk.Metadata;
    using Model;

    public interface IProxyModelService : IMessageProvider
    {
        ProxyModel BuildModel(IEnumerable<EntityMetadata> entityMetadata, IEnumerable<SdkMessageMetadata> messageMetadata);
    }
}
