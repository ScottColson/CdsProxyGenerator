using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.CDS.ProxyGenerator
{
    using Parser;

    public class TypeConverterFactory : ITypeConverterFactory
    {
        public ITypeConverter Create(eTemplalteLanguage language)
        {
            switch (language)
            {
                case eTemplalteLanguage.Csharp:
                    return new CSharpTypeCoverter();

                default:
                    throw new Exception("Unsupported template language");
            }
        }
    }
}
