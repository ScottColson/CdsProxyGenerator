﻿using System.Collections.Generic;
using CCLLC.CDS.Sdk.Metadata;

namespace CCLLC.CDS.ProxyGenerator
{
    public interface ISettings
    {
        string Namespace { get; }
        string TemplateFilePath { get; }
        string OutputPath { get; }

        IEnumerable<string> EntitiesToExclude { get; }
        IEnumerable<string> EntitiesToInclude { get; }
        IEnumerable<string> ActionsToInclude { get; }     
        IEnumerable<string> ActionsToExclude { get; }

        eEndpoint TargetEndPoint { get; }
        eTemplalteLanguage TemplateLanguage { get; }
    }
}
