using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata.EF6
{
    public class BloggingContext : DbContext
    {
        public BloggingContext()
            : base(@"Server=(localdb)\mssqllocaldb;Database=Demo.Metadata.EF6;Trusted_Connection=True;")
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Types()
                .Configure(t => t.ToTable("tbl_" + t.ClrType.Name.ToLower()));
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public string Metadata { get; set; }

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
