using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;

namespace SeedData
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SeeData;Trusted_Connection=True;");

            optionsBuilder.ReplaceService<IMigrationCommandExecutor, MyExecutor>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().SeedData(
                new Tag { TagId = ".NET" },
                new Tag { TagId = "VisualStudio" },
                new Tag { TagId = "C#" },
                new Tag { TagId = "F#" },
                new Tag { TagId = "VB.NET" },
                new Tag { TagId = "EnityFramework" },
                new Tag { TagId = "ASP.NET" });

            modelBuilder.Entity<User>().SeedData(
                new User { UserId = 2, UserName = "Administrator" });
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

        public List<PostTag> Tags { get; set; }
    }

    public class PostTag
    {
        public int Id { get; set; }

        public string TagId { get; set; }
        public Tag Tag { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }

    public class Tag
    {
        public string TagId { get; set; }
        public List<PostTag> Posts { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public List<Blog> OwnedBlogs { get; set; }
    }
}
