using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsumingServices
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                var serviceProvider = db.GetInfrastructure<IServiceProvider>();
                var typeMapper = serviceProvider.GetService<IRelationalTypeMapper>();

                Console.WriteLine($"Type mapper in use: {typeMapper.GetType().Name}");
                Console.WriteLine($"Mapping for bool: {typeMapper.GetMapping(typeof(bool)).StoreType}");
                Console.WriteLine($"Mapping for string: {typeMapper.GetMapping(typeof(string)).StoreType}");
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.ConsumingServices;Trusted_Connection=True;");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }
}
