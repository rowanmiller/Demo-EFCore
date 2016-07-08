using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;

namespace ReplacingServices
{
    public static class Extensions
    {
        public static bool HasClrAttribute<TAttribute>(this IProperty property)
            where TAttribute : Attribute
        {
            var clrType = property.DeclaringEntityType.ClrType;
            var clrProperty = property.DeclaringEntityType.ClrType.GetProperty(property.Name);

            return clrProperty == null
                ? false
                : clrProperty.CustomAttributes.Any(a => a.AttributeType == typeof(XmlAttribute));
        }
    }
}
