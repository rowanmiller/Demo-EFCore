using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MemoryOptimizedTables
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            var cancellationTokenSource = new CancellationTokenSource();
            var workerThreads = new List<Thread>()
            {
                new Thread(() => MonitorSensor("Temperature", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Humidity", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Air Pressure", cancellationTokenSource.Token)),
                new Thread(() => MonitorDatabase(cancellationTokenSource.Token))
            };

            Console.WriteLine("Starting worker threads");
            workerThreads.ForEach(t => t.Start());

            Console.WriteLine("Press 'Enter' to end work");
            Console.WriteLine();
            Console.ReadLine();

            Console.WriteLine("Stopping worker threads");
            cancellationTokenSource.Cancel();
            while (workerThreads.Any(t => t.IsAlive))
            {
                Thread.Sleep(100);
            }
        }

        private static void MonitorSensor(string sensorId, CancellationToken cancellationToken)
        {
            var sequenceNo = 0;

            while (!cancellationToken.IsCancellationRequested)
            {

                using (var db = new BloggingContext())
                {
                    using (var t = db.Database.BeginTransaction())
                    {
                        var ids = new int[200];
                        var comments = new SensorRead[ids.Length];
                        for (int i = 0; i < ids.Length; i++)
                        {
                            ids[i] = sequenceNo;
                            sequenceNo++;
                            comments[i] = new SensorRead { SensorId = sensorId, SequenceNo = ids[i], Value = 0 };
                        }

                        db.SensorReads.AddRange(comments);
                        db.SaveChanges();

                        db.SensorReads
                            .AsNoTracking()
                            .OrderByDescending(s => s.SequenceNo)
                            .Where(c => c.SensorId == sensorId && ids.Contains(c.SequenceNo))
                            .ToList();

                        t.Commit();
                    }
                }
            }
        }

        private static void MonitorDatabase(object threadParams)
        {
            var ct = (CancellationToken)threadParams;

            using (var db = new BloggingContext())
            {
                var cmd = db.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = "select max(cntr_value) FROM sys.dm_os_performance_counters WHERE counter_name = 'Batch Requests/sec'";
                db.Database.OpenConnection();

                var firstValue = (long)cmd.ExecuteScalar();
                var firstRead = DateTime.Now;

                Thread.Sleep(3000);

                while (!ct.IsCancellationRequested)
                {
                    var thisValue = (long)cmd.ExecuteScalar();
                    var thisRead = DateTime.Now;

                    var totalRequests = thisValue - firstValue;
                    var requestsPerSecond = totalRequests / (thisRead - firstRead).TotalSeconds;
                    Console.WriteLine($"Average: {requestsPerSecond:0} requests/sec    ({totalRequests} total requests)");

                    Thread.Sleep(3000);
                }

                db.Database.CloseConnection();
            }
        }

        private static void SetupDatabase()
        {
            using (var db = new BloggingContext())
            {
                Console.WriteLine("Dropping the database");
                db.Database.EnsureDeleted();

                Console.WriteLine("Creating the database");
                db.Database.EnsureCreated();
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<SensorRead> SensorReads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQL2016;Database=Demo.MemoryOptimizedTables;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorRead>().HasKey(c => new { c.SensorId, c.SequenceNo });

            modelBuilder.Entity<SensorRead>().ForSqlServerIsMemoryOptimized();
        }
    }

    public class SensorRead
    {
        public string SensorId { get; set; }
        public int SequenceNo { get; set; }
        public double Value { get; set; }
    }
}
