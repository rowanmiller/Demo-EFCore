using Microsoft.EntityFrameworkCore;
using Performance.EFCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CompiledQueries
{
    class Program
    {
        static void Main(string[] args)
        {
            // Warmup
            using (var db = new AdventureWorksContext())
            {
                var customer = db.Customers.Single(c => c.AccountNumber == "AW00030118");
            }

            RunTest(
                regularTest: (accountNumbers) =>
                {
                    using (var db = new AdventureWorksContext())
                    {
                        foreach (var accountNumber in accountNumbers)
                        {
                            var customer = db.Customers.Single(c => c.AccountNumber == accountNumber);
                        }
                    }
                },
                compiledTest: (accountNumbers) =>
                {
                    var query = EF.CompileQuery((AdventureWorksContext db, string accountNumber) =>
                        db.Customers.Single(c => c.AccountNumber == accountNumber));

                    using (var db = new AdventureWorksContext())
                    {
                        foreach (var accountNumber in accountNumbers)
                        {
                            var customer = query(db, accountNumber);
                        }
                    }
                });
        }

        private static void RunTest(Action<string[]> regularTest, Action<string[]> compiledTest)
        {
            var accountNumbers = GetAccountNumbers(500);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            regularTest(accountNumbers);
            stopwatch.Stop();
            var regularResult = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Regular:  {regularResult.ToString().PadLeft(4)}ms");

            stopwatch.Restart();
            compiledTest(accountNumbers);
            stopwatch.Stop();
            var compiledResult = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Compiled: {compiledResult.ToString().PadLeft(4)}ms");
        }

        private static string[] GetAccountNumbers(int count)
        {
            var accountNumbers = new string[count];
            for (int i = 0; i < count; i++)
            {
                accountNumbers[i] = "AW" + (i + 1).ToString().PadLeft(8, '0');
            }

            return accountNumbers;
        }
    }
}
