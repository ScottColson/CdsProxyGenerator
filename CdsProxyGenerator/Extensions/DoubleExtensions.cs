using System;

namespace CCLLC.CDS.ProxyGenerator.Extensions
{
    public static class DoubleExtensions
    {
        public static Decimal ToDecimal(this double value)
        {
            return (decimal)value;
        }
    }
}
