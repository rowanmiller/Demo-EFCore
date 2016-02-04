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
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEntityFramework()
                .AddSqlServer();

            serviceCollection.AddSingleton<SqlServerTypeMapper, CustomSqlServerTypeMapper>();

            using (var db = new BloggingContext(serviceCollection.BuildServiceProvider()))
            {
                var serviceProvider = db.GetInfrastructure<IServiceProvider>();
                var typeMapper = serviceProvider.GetService<IRelationalTypeMapper>();

                Console.WriteLine($"Type mapper in use: {typeMapper.GetType().Name}");
                Console.WriteLine($"Mapping for bool: {typeMapper.GetMapping(typeof(bool)).DefaultTypeName}");
                Console.WriteLine($"Mapping for string: {typeMapper.GetMapping(typeof(string)).DefaultTypeName}");

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

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
        public BloggingContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

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
