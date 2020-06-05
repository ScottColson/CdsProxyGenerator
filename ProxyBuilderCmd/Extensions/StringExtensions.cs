using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CCLLC.CDS.ProxyBuilderCmd.Extensions
{
    public static class StringExtensions
    {
        public static string ColapseWhiteSpace(this string input)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"\s+", options);
            return regex.Replace(input.Trim(), " ");
        }
    }
}
