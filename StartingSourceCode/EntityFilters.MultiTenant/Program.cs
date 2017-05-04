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
                    Console.WriteLine(blog.Url);
                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($" - {post.Title}");
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
                            new Post { Title = "Caring for tropical fish" },
                            new Post { Title = "Types of ornamental fish" }
                        }
                    });

                    db.Blogs.Add(new Blog
                    {
                        Url = "http://sample.com/blogs/cats",
                        Posts = new List<Post>
                        {
                            new Post { Title = "Cat care 101" },
                            new Post { Title = "Caring for tropical cats" },
                            new Post { Title = "Types of ornamental cats" }
                        }
                    });

                    db.SaveChanges();
                }
            }

            using (var db = new BloggingContext("jeff"))
            {
                db.Blogs.Add(new Blog
                {
                    Url = "http://sample.com/blogs/catfish",
                    Posts = new List<Post>
                        {
                            new Post { Title = "Catfish care 101" },
                            new Post { Title = "History of the catfish name" },
                        }
                });

                db.SaveChanges();
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
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.EntityFilters.MultiTenant;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().Property<string>("TenantId").HasField("_tenantId").Metadata.IsReadOnlyAfterSave = true;
            modelBuilder.Entity<Post>().Property<string>("TenantId").HasField("_tenantId").Metadata.IsReadOnlyAfterSave = true;

            modelBuilder.Entity<Blog>().HasQueryFilter(b => EF.Property<string>(b, "TenantId") == _tenantId);
            modelBuilder.Entity<Post>().HasQueryFilter(b => EF.Property<string>(b, "TenantId") == _tenantId);
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
        private string _tenantId;

        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post 
    {
        private string _tenantId;

        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
