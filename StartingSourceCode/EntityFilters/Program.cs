using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFilters.MultiTenant
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            using (var db = new BloggingContext("rowan"))
            {
                var blogs = db.Blogs
                    .Include(b => b.Posts)
                    .ToList();

                foreach (var blog in blogs)
                {
                    Console.WriteLine($"{blog.Url.PadRight(33)} [Tenant: {blog.TenantId}]");

                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($" - {post.Title.PadRight(30)} [IsDeleted: {post.IsDeleted}]");
                    }

                    Console.WriteLine();
                }
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext("rowan"))
            {
                if (db.Database.EnsureCreated())
                {
                    db.Blogs.Add(new Blog
                    {
                        Url = "http://sample.com/blogs/fish",
                        Posts = new List<Post>
                        {
                            new Post { Title = "Fish care 101" },
                            new Post { Title = "Caring for tropical fish", IsDeleted = true },
                            new Post { Title = "Types of ornamental fish" }
                        }
                    });

                    db.Blogs.Add(new Blog
                    {
                        Url = "http://sample.com/blogs/cats",
                        Posts = new List<Post>
                        {
                            new Post { Title = "Cat care 101", IsDeleted = true },
                            new Post { Title = "Caring for tropical cats" },
                            new Post { Title = "Types of ornamental cats" }
                        }
                    });

                    db.SaveChanges();

                    using (var jeff_db = new BloggingContext("jeff"))
                    {
                        jeff_db.Blogs.Add(new Blog
                        {
                            Url = "http://sample.com/blogs/catfish",
                            Posts = new List<Post>
                        {
                            new Post { Title = "Catfish care 101" },
                            new Post { Title = "History of the catfish name" },
                        }
                        });

                        jeff_db.SaveChanges();
                    }
                }
            }
        }
    }

    public class BloggingContext : DbContext
    {
        private readonly string _tenantId;

        public BloggingContext(string tenant)
        {
            _tenantId = tenant;
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.EntityFilters;Trusted_Connection=True;ConnectRetryCount=0;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            foreach (var item in ChangeTracker.Entries().Where(e => e.State == EntityState.Added && e.Metadata.GetProperties().Any(p => p.Name == "TenantId")))
            {
                item.CurrentValues["TenantId"] = _tenantId;
            }

            return base.SaveChanges();
        }
    }

    public class Blog 
    {
        public string TenantId { get; set; }
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post 
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
