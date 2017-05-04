using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFilters.SoftDelete
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            using (var db = new BloggingContext())
            {
                var blogs = db.Blogs.Include(b => b.Posts).ToList();

                foreach (var blog in blogs)
                {
                    Console.WriteLine($"{blog.Url.PadRight(33)} [IsDeleted = {blog.IsDeleted}]");
                    foreach (var post in blog.Posts)
                    {
                        Console.WriteLine($" - {post.Title.PadRight(33)} [IsDeleted = {post.IsDeleted}]");
                    }
                    Console.WriteLine();
                }
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext())
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
                        Url = "http://sample.com/blogs/catfish",
                        IsDeleted = true,
                        Posts = new List<Post>
                        {
                            new Post { Title = "Catfish care 101" },
                            new Post { Title = "History of the catfish name" },
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
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.EntityFilters.SoftDelete;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasQueryFilter(b => !EF.Property<bool>(b, "IsDeleted"));
            modelBuilder.Entity<Post>().HasQueryFilter(b => !b.IsDeleted);
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsDeleted { get; set; }

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
