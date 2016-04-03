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

            RunTest(
                ef6Test: () =>
                {
                    using (var db = new EF6.AdventureWorksContext())
                    {
                        
                    }
                },
                ef7Test: () =>
                {
                    using (var db = new EFCore.AdventureWorksContext())
                    {
                        
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

                var result = ef6 / (double)efCore;
                Console.WriteLine($"  - Speedup: {result.ToString("F1")}x");
                Console.WriteLine();
            }
        }
    }
}
