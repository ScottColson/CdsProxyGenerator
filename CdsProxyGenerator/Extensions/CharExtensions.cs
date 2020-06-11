using System.Globalization;


namespace CCLLC.CDS.ProxyGenerator.Extensions
{
    public static class CharExtensions
    {
        public static bool IsCurrencySymbol(this char c)
        {
            return char.GetUnicodeCategory(c) == UnicodeCategory.CurrencySymbol;
        }

        public static bool IsPercentSymbol(this char c)
        {
            return c == '%';
        }
    }
}
