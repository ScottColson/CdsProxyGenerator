﻿using Microsoft.Xrm.Sdk.Metadata;

namespace CCLLC.CDS.ProxyGenerator.Parser
{
    using Extensions;
    using Model;

    public class FieldParser
    {
        public static FieldModel Parse(EntityModel parent, AttributeMetadata metadata)
        {
            var fieldModel = new FieldModel(
                parent,
                metadata.LogicalName,
                metadata.SchemaName,
                metadata.DisplayName(),
                metadata.PrivatePropertyName(),
                metadata.CdsDataType(),
                metadata.IsPrimaryKey(),
                metadata.IsLookup(),
                metadata.IsRequired(),
                metadata.IsUpdatable(),
                metadata.IsCreateable(),
                metadata.MinValue(),
                metadata.MaxValue(),
                metadata.MaxLength());

            EnumModel enumModel = null;
            if(metadata is PicklistAttributeMetadata)
            {
                enumModel = EnumParser.Parse(fieldModel, metadata as PicklistAttributeMetadata);
            }
            if(metadata is StateAttributeMetadata)
            {
                enumModel = EnumParser.Parse(fieldModel, metadata as StateAttributeMetadata);
            }
            if(metadata is StatusAttributeMetadata)
            {
                enumModel = EnumParser.Parse(fieldModel, metadata as StatusAttributeMetadata);
            }
           

            fieldModel.Enum = enumModel;

            return fieldModel;
        }
    }
}
