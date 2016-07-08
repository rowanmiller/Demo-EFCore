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
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddEntityFrameworkSqlServer();

            serviceCollection.AddSingleton<SqlServerTypeMapper, CustomSqlServerTypeMapper>();

            var options = new DbContextOptionsBuilder()
                .UseInternalServiceProvider(serviceCollection.BuildServiceProvider())
                .Options;

            using (var db = new BloggingContext(options))
            {
                var serviceProvider = db.GetInfrastructure<IServiceProvider>();
                var typeMapper = serviceProvider.GetService<IRelationalTypeMapper>();

                Console.WriteLine($"Type mapper in use: {typeMapper.GetType().Name}");
                Console.WriteLine($"Mapping for bool: {typeMapper.GetMapping(typeof(bool)).StoreType}");
                Console.WriteLine($"Mapping for string: {typeMapper.GetMapping(typeof(string)).StoreType}");

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
        public BloggingContext(DbContextOptions options)
            : base(options)
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
