using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.Data;

namespace ReplacingServices
{
    public class CustomSqlServerTypeMapper : SqlServerTypeMapper
    {
        private readonly RelationalTypeMapping _xml = new RelationalTypeMapping("xml", typeof(string), DbType.Xml);

        public override RelationalTypeMapping FindMapping(IProperty property)
        {
            if (property.HasClrAttribute<XmlAttribute>())
            {
                return _xml;
            }

            return base.FindMapping(property);
        }

        public override RelationalTypeMapping FindMapping(string typeName)
        {
            if (typeName == "xml")
            {
                return _xml;
            }

            return base.FindMapping(typeName);
        }
    }
}
