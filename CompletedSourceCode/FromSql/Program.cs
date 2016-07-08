using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FromSql
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            using (var db = new BloggingContext())
            {
                Console.Write("Enter a search term: ");
                var term = Console.ReadLine();

                var blogs = db.Blogs.FromSql("SELECT * FROM dbo.SearchBlogs(@p0)", term)
                    .OrderBy(b => b.Url)
                    .Select(b => b.Url);

                foreach (var blog in blogs)
                {
                    Console.WriteLine(blog);
                }
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext())
            {
                if (db.Database.EnsureCreated())
                {
                    db.Database.ExecuteSqlCommand("CREATE FUNCTION [dbo].[SearchBlogs] (@term nvarchar(200)) RETURNS TABLE AS RETURN (SELECT * FROM dbo.Blogs WHERE Url LIKE '%' + @term + '%')");

                    db.Blogs.Add(new Blog { Url = "http://sample.com/blogs/fish" });
                    db.Blogs.Add(new Blog { Url = "http://sample.com/blogs/catfish" });
                    db.Blogs.Add(new Blog { Url = "http://sample.com/blogs/cats" });
                    db.SaveChanges();
                }
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.FromSql;Trusted_Connection=True;");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
