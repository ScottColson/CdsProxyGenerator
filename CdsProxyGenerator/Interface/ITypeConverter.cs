namespace CCLLC.CDS.ProxyGenerator
{
    using Model;

    public interface ITypeConverter
    {
        TypeModel GetTypeModel(eCdsDataType dataType, string enumTypeName);
    }
}
