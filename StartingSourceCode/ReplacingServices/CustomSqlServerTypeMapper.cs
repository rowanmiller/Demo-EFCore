using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Storage.Internal;
using System.Data;

namespace ReplacingServices
{
    public class CustomSqlServerTypeMapper : SqlServerTypeMapper
    {
        private readonly RelationalTypeMapping _xml = new RelationalTypeMapping("xml", typeof(string), DbType.Xml);

        public override RelationalTypeMapping GetMapping(IProperty property)
        {
            // TODO Use XML type if property has [Xml] attribute

            return base.FindMapping(property);
        }

        public override RelationalTypeMapping GetMapping(string typeName)
        {
            if (typeName == "xml")
            {
                return _xml;
            }

            return base.GetMapping(typeName);
        }
    }
}
