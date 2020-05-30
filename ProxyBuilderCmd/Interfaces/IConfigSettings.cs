using CCLLC.CDS.ProxyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.CDS.ProxyBuilderCmd
{
    public interface IConfigSettings : ISettings
    {
        string ConfigurationPath { get; set; }
    }
}
