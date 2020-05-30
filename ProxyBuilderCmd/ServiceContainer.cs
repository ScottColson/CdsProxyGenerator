using CCLLC.Core;
using CCLLC.CDS.ProxyGenerator;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    public class ServiceContainer
    {
        private static IocContainer IocContainer;

        static ServiceContainer()
        {
            Init();
        }        

        public static void Init()
        {
            IocContainer = new IocContainer();

            IocContainer.Implement<ISpklSettingsService>().Using<Spkl.SpklSettingsService>().AsSingleInstance();
            IocContainer.Implement<IProxySettingsService>().Using<ProxySettingsService>().AsSingleInstance();
            IocContainer.Implement<IDirectoryService>().Using<DirectoryService>().AsSingleInstance();
            IocContainer.Implement<ITypeConverterFactory>().Using<TypeConverterFactory>();       
        }

        public static T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }
    }
}
