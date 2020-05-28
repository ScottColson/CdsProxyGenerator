using CCLLC.CDS.ProxyGenerator.Model;

namespace CCLLC.CDS.ProxyGenerator
{
    public interface IProxyGeneratorService : IMessageProvider
    {
        void BuildProxies(ProxyModel model);
    }
}
