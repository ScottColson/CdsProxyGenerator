
namespace CCLLC.CDS.ProxyGenerator
{
    public interface ITypeConverterFactory
    {
        ITypeConverter Create(eTemplalteLanguage language);
    }
}
