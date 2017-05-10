using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.Data;
using JetBrains.Annotations;

namespace ReplacingServices
{
    public class CustomSqlServerTypeMapper : SqlServerTypeMapper
    {
        private readonly RelationalTypeMapping _xml = new RelationalTypeMapping("xml", typeof(string), DbType.Xml);

        public CustomSqlServerTypeMapper(RelationalTypeMapperDependencies dependencies) 
            : base(dependencies)
        {
        }

        public override RelationalTypeMapping FindMapping(IProperty property)
        {
            // TODO Use XML type if property has [Xml] attribute

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
