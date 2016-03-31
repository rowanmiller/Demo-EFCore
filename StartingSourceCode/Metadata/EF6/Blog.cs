using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metadata.EF6
{
    [Table("tbl_blog")]
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public string Metadata { get; set; }

        public List<Post> Posts { get; set; }
    }
}
