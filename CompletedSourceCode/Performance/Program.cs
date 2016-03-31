using System;
using System.Diagnostics;
using System.Linq;

namespace Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            ResetAndWarmup();

            RunToListTest();
            RunComplexQueryTest();
            RunAddAndSaveChangesTest();
            RunAddAndSaveChangesOptimizedTest();
        }

        private static void RunToListTest()
        {
            Console.WriteLine("Query ToList");
            RunTest(
                ef6Test: () =>
                {
                    using (var db = new EF6.AdventureWorksContext())
                    {
                        db.Customers.ToList();
                    }
                },
                ef7Test: () =>
                {
                    using (var db = new EFCore.AdventureWorksContext())
                    {
                        db.Customers.ToList();
                    }
                });
        }

        private static void RunComplexQueryTest()
        {
            Console.WriteLine("Query Complex");
            RunTest(
                ef6Test: () =>
                {
                    using (var db = new EF6.AdventureWorksContext())
                    {
                        db.Customers
                            .Where(c => !c.AccountNumber.EndsWith("1"))
                            .OrderBy(c => c.AccountNumber)
                            .ThenBy(c => c.ModifiedDate)
                            .Skip(100)
                            .GroupBy(c => c.TerritoryID)
                            .ToList();
                    }
                },
                ef7Test: () =>
                {
                    using (var db = new EFCore.AdventureWorksContext())
                    {
                        db.Customers
                            .Where(c => !c.AccountNumber.EndsWith("1"))
                            .OrderBy(c => c.AccountNumber)
                            .ThenBy(c => c.ModifiedDate)
                            .Skip(100)
                            .GroupBy(c => c.TerritoryID)
                            .ToList();
                    }
                });
        }

        private static void RunAddAndSaveChangesTest()
        {
            Console.WriteLine("Add & SaveChanges");
            RunTest(
                () =>
                {
                    using (var db = new EF6.AdventureWorksContext())
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            db.ProductCategories.Add(new EF6.ProductCategory { Name = $"Test {Guid.NewGuid()}" });
                        }
                        db.SaveChanges();
                    }
                },
                () =>
                {
                    using (var db = new EFCore.AdventureWorksContext())
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            db.ProductCategories.Add(new EFCore.ProductCategory { Name = $"Test {Guid.NewGuid()}" });
                        }
                        db.SaveChanges();
                    }
                });
        }

        private static void RunAddAndSaveChangesOptimizedTest()
        {
            Console.WriteLine("Add & SaveChanges (EF6 Optimized)");
            RunTest(
                () =>
                {
                    using (var db = new EF6.AdventureWorksContext())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        var categories = new EF6.ProductCategory[1000];
                        for (int i = 0; i < 1000; i++)
                        {
                            categories[i] = new EF6.ProductCategory { Name = $"Test {Guid.NewGuid()}" };
                        }
                        db.ProductCategories.AddRange(categories);
                        db.SaveChanges();
                    }
                },
                () =>
                {
                    using (var db = new EFCore.AdventureWorksContext())
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            db.ProductCategories.Add(new EFCore.ProductCategory { Name = $"Test {Guid.NewGuid()}" });
                        }
                        db.SaveChanges();
                    }
                });
        }

        private static void ResetAndWarmup()
        {
            using (var db = new EF6.AdventureWorksContext())
            {
                db.Database.ExecuteSqlCommand(@"DELETE FROM Production.ProductCategory WHERE Name LIKE 'Test %'");
                db.Customers.FirstOrDefault();
            }

            using (var db = new EFCore.AdventureWorksContext())
            {
                db.Customers.FirstOrDefault();
            }
        }

        private static void RunTest(Action ef6Test, Action ef7Test)
        {
            var stopwatch = new Stopwatch();
            for (int iteration = 0; iteration < 3; iteration++)
            {
                Console.WriteLine($" Iteration {iteration}");

                stopwatch.Restart();
                ef6Test();
                stopwatch.Stop();
                var ef6 = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"  - EF6.x:      {ef6.ToString().PadLeft(4)}ms");

                stopwatch.Restart();
                ef7Test();
                stopwatch.Stop();
                var efCore = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"  - EF Core:    {efCore.ToString().PadLeft(4)}ms");

                var result = (ef6 - efCore) / (double)ef6;
                Console.WriteLine($"  - Improvement: {result.ToString("P0")}");
                Console.WriteLine();
            }
        }
    }
}
