﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;


namespace CCLLC.CDS.ProxyGenerator
{ 
    using CCLLC.CDS.Sdk.Metadata;

    public class CDSMetadataService : MessagingBase, ICDSMetadataService
    {
        protected readonly ISettings Settings;
        protected readonly IOrganizationService OrgService;

        public CDSMetadataService(ISettings settings, IOrganizationService orgService) : base()
        {
            this.Settings = settings ?? throw new ArgumentNullException("settings");
            this.OrgService = orgService ?? throw new ArgumentNullException("orgService");
        }

        public IEnumerable<EntityMetadata> GetEntityMetadata()
        {
            if(Settings.EntitiesToInclude is null || Settings.EntitiesToInclude.Count() == 0)
            {
                RaiseMessage("No entities requested...");
                return new List<EntityMetadata>();
            }

            var metadata = GetAllEntityMetadata();
            metadata = FilterEntityMetadata(metadata);

            RaiseMessage(string.Format("Retrieved metadata for {0} entities", metadata.Count()));

            return metadata;
        }

        public IEnumerable<SdkMessageMetadata> GetMessageMetadata()
        {    
            if(Settings.ActionsToInclude is null || Settings.ActionsToInclude.Count() == 0)
            {
                RaiseMessage("No Sdk Messages requested...");
                return new List<SdkMessageMetadata>();
            }

            var service = new SdkMessageMetadataService(Settings.TargetEndPoint);
            RaiseMessage("Getting Sdk MessageNames...");
           
            var names = service.GetSdkMessageNames(this.OrgService);

            names = FilterSdkMessageNames(names);

            RaiseMessage("Getting Sdk MessageMetadata...");

            var messageMetadata = service.GetSdkMessageMetadata(this.OrgService, names);

            RaiseMessage(string.Format("Retrieved metadata for {0} Sdk Messages...", messageMetadata.Count()));

            return messageMetadata;
        }

        private IEnumerable<EntityMetadata> GetAllEntityMetadata()
        {
            RaiseMessage("Gathering Entity Metadata...");

            OrganizationRequest request = new OrganizationRequest("RetrieveAllEntities");

            request.Parameters["EntityFilters"] = EntityFilters.Relationships | EntityFilters.Attributes | EntityFilters.Entity;
            request.Parameters["RetrieveAsIfPublished"] = true;

            return this.OrgService.Execute(request).Results["EntityMetadata"] as EntityMetadata[];
        }

        private IEnumerable<EntityMetadata> FilterEntityMetadata(IEnumerable<EntityMetadata> entityMetadata)
        {
            RaiseMessage("Filtering Entity Metadata...");

            var entitiesToInclude = new MatchEvaluator(Settings.EntitiesToInclude);

            var entitiesToExclude = new MatchEvaluator(Settings.EntitiesToExclude);

            var filteredMetadata = entityMetadata
                .Where(r => ShouldIncludeMetadata(r.LogicalName, entitiesToInclude, entitiesToExclude))
                .ToArray();

            return filteredMetadata;
        }

        private IEnumerable<string> FilterSdkMessageNames(IEnumerable<string> messageNames)
        {
            RaiseMessage("Filtering Sdk Message Names...");

            var actionsToInclude = new MatchEvaluator(Settings.ActionsToInclude);

            var actionsToExclude = new MatchEvaluator(Settings.ActionsToExclude);

            var filteredNames = messageNames
                .Where(r => ShouldIncludeMetadata(r, actionsToInclude, actionsToExclude))
                .ToArray();

            return filteredNames;
        }

        private bool ShouldIncludeMetadata(string logicalName, IMatchEvaluator entitiesToInclude, IMatchEvaluator entitiesToExclude)
        {
            var includeWeight = entitiesToInclude.ScoreMatch(logicalName);
            var excludeWeight = entitiesToExclude.ScoreMatch(logicalName);

            return (includeWeight > excludeWeight);
        }

    }
}