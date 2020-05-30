using CCLLC.CDS.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLLC.CDS.ProxyGenerator
{
    public class CDSMetadataServiceFactory : ICDSMetadataServiceFactory
    {
        private readonly Dictionary<eEndpoint, ICDSMetadataService> ServiceDictionary = new Dictionary<eEndpoint, ICDSMetadataService>();

        public ICDSMetadataService Create(ISettings settings)
        {
            var endpoint = settings.TargetEndPoint;

            if (ServiceDictionary.ContainsKey(endpoint))
                return ServiceDictionary[endpoint];

            var service = new CDSMetadataService(settings);
            ServiceDictionary.Add(endpoint, service);

            return service;           
        }
    }
}
