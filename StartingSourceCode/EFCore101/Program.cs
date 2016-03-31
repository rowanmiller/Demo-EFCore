using Microsoft.Data.Entity;
using System;
using System.Linq;

namespace EFCore101
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BloggingContext())
            {
                // TODO Do some data access
                var blog = new Blog { Name = "The Dog Blog", Url = "http://sample.com/dogs" };

            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        // TODO Setup the database to connect to
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
