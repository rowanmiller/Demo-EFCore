using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFunctions
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(databaseName: "Test1")
                .Options;

            using (var db = new BloggingContext(options))
            {
                db.Blogs.Add(new Blog { Url = "aaa" });
                db.Blogs.Add(new Blog { Url = "baaab" });
                db.Blogs.Add(new Blog { Url = "ccc" });
                db.SaveChanges();
            }

            using (var db = new BloggingContext(options))
            {
                var service = new BlogService(db);
                var blogs = service.SearchBlogs("aaa");
                Assert.AreEqual(2, blogs.Count());
            }
        }
    }
}
