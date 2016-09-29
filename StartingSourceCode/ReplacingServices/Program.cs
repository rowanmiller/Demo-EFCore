using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ReplacingServices
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                var typeMapper = db.GetService<IRelationalTypeMapper>();

                Console.WriteLine($"Type mapper in use: {typeMapper.GetType().Name}");
                Console.WriteLine($"Mapping for bool: {typeMapper.GetMapping(typeof(bool)).StoreType}");
                Console.WriteLine($"Mapping for string: {typeMapper.GetMapping(typeof(string)).StoreType}");

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
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.ReplacingServices;Trusted_Connection=True;");

            // TODO Register our custom type mapper

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
