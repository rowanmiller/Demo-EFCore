using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Net.Http;

namespace FieldMapping
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            using (var db = new BloggingContext())
            {
                var blog = new Blog();
                blog.Name = "Rowan's Blog";
                blog.SetUrl("http://romiller.com");

                db.Blogs.Add(blog);
                db.SaveChanges();
            }

            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs
                    .OrderBy(b => b.Url)
                    .ToList();

                Console.WriteLine("All blogs:");
                foreach (var item in blogs)
                {
                    Console.WriteLine($" * {item.Name} | {item.Url}");
                }
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext())
            {
                Console.WriteLine("Recreating database from current model");
                Console.WriteLine(" Dropping database...");
                db.Database.EnsureDeleted();

                Console.WriteLine(" Creating database...");
                db.Database.EnsureCreated();
                Console.WriteLine();
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.FieldMapping;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }

    public class Blog
    {
        private string _url;

        public int BlogId { get; set; }
        public string Name { get; set; }

        public string Url
        {
            get { return _url; }
        }

        public void SetUrl(string url)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
            }

            _url = url;
        }
    }
}
