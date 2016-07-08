using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Metadata
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintEF6Mappings();
            PrintEFCoreMappings();
        }

        private static void PrintEFCoreMappings()
        {
            Console.WriteLine("EF Core Mappings");
            using (var db = new EFCore.BloggingContext())
            {
                foreach (var entity in db.Model.GetEntityTypes())
                {
                    Console.WriteLine($" {entity.ClrType.Name} => {entity.SqlServer().TableName}");
                }
            }
        }

        private static void PrintEF6Mappings()
        {
            Console.WriteLine("EF6.x Mappings");
            using (var db = new EF6.BloggingContext())
            {
                var metadata = ((IObjectContextAdapter)db).ObjectContext.MetadataWorkspace;

                // Get the part of the model that contains info about the actual CLR types
                var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

                foreach (var entityType in metadata.GetItems<EntityType>(DataSpace.OSpace))
                {
                    // Get the entity set that uses this entity type
                    var entitySet = metadata
                        .GetItems<EntityContainer>(DataSpace.CSpace)
                        .Single()
                        .EntitySets
                        .Single(s => s.ElementType.Name == entityType.Name);

                    // Find the mapping between conceptual and storage model for this entity set
                    var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                            .Single()
                            .EntitySetMappings
                            .Single(s => s.EntitySet == entitySet);

                    // Find the storage entity set (table) that the entity is mapped
                    var table = mapping
                        .EntityTypeMappings.Single()
                        .Fragments.Single()
                        .StoreEntitySet;

                    var tableName = (string)table.MetadataProperties["Table"].Value ?? table.Name;
                    var clrType = objectItemCollection.GetClrType(entityType);

                    Console.WriteLine($" {clrType.Name} => {tableName}");
                }

                Console.WriteLine();
            }
        }
    }
}
