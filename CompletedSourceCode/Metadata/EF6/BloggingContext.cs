using System.Data.Entity;

namespace Metadata.EF6
{
    public class BloggingContext : DbContext
    {
        public BloggingContext()
            : base(@"Server=(localdb)\mssqllocaldb;Database=Demo.Metadata.EF6;Trusted_Connection=True;")
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
