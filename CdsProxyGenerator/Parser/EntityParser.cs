
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace CCLLC.CDS.ProxyGenerator.Parser
{
    using Extensions;
    using Model;

    public class EntityParser
    {
        public static EntityModel Parse(ProxyModel parent, EntityMetadata metadata)
        {
            var entityModel = new EntityModel(
                parent,
                metadata.LogicalName,
                metadata.SchemaName,
                metadata.NormalizedName(),
                metadata.PluralName(),
                metadata.PrimaryIdAttribute);

            var fields = metadata.Attributes               
               .Select(a => FieldParser.Parse(entityModel, a)).ToList();

            fields.ForEach(f =>
            {
                if (f.DisplayName == entityModel.DisplayName)
                    f.DisplayName += "1";
            });
          
            fields = fields.OrderBy(f => f.DisplayName).ToList();

            entityModel.Fields = fields;

            return entityModel;
        }
    }
}
