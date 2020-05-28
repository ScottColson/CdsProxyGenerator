using System;
using System.Collections.Generic;
using System.Linq;

namespace CCLLC.CDS.ProxyGenerator.Model
{
    public class EntityModel 
    {   
        internal ProxyModel ProxyModel { get; }
        internal string PrimaryKeyAttribute { get; }
        public string LogicalName { get; }
        public string SchemaName { get; }
       
        public string DisplayName { get; }
        public string PluralName { get; }

        private IEnumerable<FieldModel> _fields;
        public IEnumerable<FieldModel> Fields
        {
            get { return _fields; }
            internal set
            {
                _fields = value;
                SetPrimaryKey();
            }
        }
        public FieldModel PrimaryKey { get; internal set; }
        public Enum[] Enums { get; }

       //public MappingRelationship1N[] RelationshipsOneToMany { get; set; }
        //public MappingRelationshipN1[] RelationshipsManyToOne { get; set; }
        //public MappingRelationshipN1[] RelationshipsManyToMany { get; set; }

       
        public EntityModel(
            ProxyModel parent,
            string logicalName, 
            string schemaName, 
            string displayName, 
            string pluralName, 
            string primaryKey)
        {
            this.ProxyModel = parent;
            this.LogicalName = logicalName;
            this.SchemaName = schemaName;
            this.DisplayName = displayName;
            this.PluralName = pluralName;
            this.PrimaryKeyAttribute = primaryKey;
        }


        private void SetPrimaryKey()
        {
            this.PrimaryKey = _fields?.Where(f => f.LogicalName == this.PrimaryKeyAttribute).FirstOrDefault();
        }
        
       
    }
}
