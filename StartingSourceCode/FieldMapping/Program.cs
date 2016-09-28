using Microsoft.EntityFrameworkCore;
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
                blog.Url = "http://romiller.com";

                db.Blogs.Add(blog);
                db.SaveChanges();
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
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
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
