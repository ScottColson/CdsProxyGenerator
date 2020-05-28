
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCLLC.CDS.ProxyGenerator.Model
{
    [Serializable]
    public class ProxyModel
    {
        public ITypeConverter TypeConverter { get; }
        public IEnumerable<EntityModel> Entities { get; internal set; }
        public IEnumerable<EnumModel> GlobalOptionSets { get; internal set; }  
        public IEnumerable<SdkMessageModel> Messages { get; internal set; }

        public ProxyModel(ITypeConverter typeConverter)
        {
            this.TypeConverter = typeConverter ?? throw new ArgumentNullException("typeConverter is required");
        }        

        internal string GetGlobalEnumType(string name)
        {
            return this.GlobalOptionSets.Where(e => e.DisplayName == name).Select(e => e.Type).FirstOrDefault();
        }
    }
}
