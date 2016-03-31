using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ReplacingServices
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO Create a ServiceProvider with our custom type mapper


            using (var db = new BloggingContext())
            {
                var serviceProvider = db.GetInfrastructure<IServiceProvider>();
                var typeMapper = serviceProvider.GetService<IRelationalTypeMapper>();

                Console.WriteLine($"Type mapper in use: {typeMapper.GetType().Name}");
                Console.WriteLine($"Mapping for bool: {typeMapper.GetMapping(typeof(bool)).DefaultTypeName}");
                Console.WriteLine($"Mapping for string: {typeMapper.GetMapping(typeof(string)).DefaultTypeName}");

                Console.WriteLine("Recreating database from current model");
                Console.WriteLine(" Dropping database...");
                db.Database.EnsureDeleted();
                Console.WriteLine(" Creating database...");
                db.Database.EnsureCreated();

                Console.WriteLine();
                Console.WriteLine("Inserting a new Blog...");
                db.Blogs.Add(new Blog
                {
                    Url = "http://sample.com",
                    Metadata = "<metadata><type>Wordpress</type><owner>rowan</owner></metadata>"
                });

                db.SaveChanges();
            }
        }
    }

    public class BloggingContext : DbContext
    {
        // TODO Add constructor to allow external ServiceProvider
        
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.ReplacingServices;Trusted_Connection=True;");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        [Xml]
        public string Metadata { get; set; }
    }
}
