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
            var workerThreads = new List<Thread>
            {
                new Thread(() => MonitorSensor("Air Temperature", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Humidity", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Air Pressure", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Oxygen", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Carbon Dioxide", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Wind Speed", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Wind Chill Factor", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Ground Temperature", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Light Level", cancellationTokenSource.Token)),
                new Thread(() => MonitorSensor("Decibels", cancellationTokenSource.Token)),
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
                    var data = CreateSomeFakeSensorReads(sensorId, sequenceNo, 100);
                    sequenceNo += 100;

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        db.SensorReads.AddRange(data);
                        db.SaveChanges();

                        var ids = data.Select(s => s.SequenceNo).ToArray(); ;
                        db.SensorReads
                            .AsNoTracking()
                            .OrderByDescending(s => s.SequenceNo)
                            .Where(c => c.SensorId == sensorId && ids.Contains(c.SequenceNo))
                            .ToList();

                        transaction.Commit();
                    }
                }
            }
        }

        private static SensorRead[] CreateSomeFakeSensorReads(string sensorId, int startSequence, int count)
        {
            var sequenceNo = startSequence;
            var random = new Random();

            var data = new SensorRead[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = new SensorRead
                {
                    SensorId = sensorId,
                    SequenceNo = sequenceNo,
                    Value = random.Next(),
                    UtcTime = DateTime.UtcNow,
                    LocalTime = DateTime.Now,
                    TimeCode = DateTime.UtcNow.Ticks,
                    StatusCode = sequenceNo
                };

                sequenceNo++;
            }

            return data;
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

            modelBuilder.Entity<SensorRead>().HasIndex(r => r.SequenceNo);
            modelBuilder.Entity<SensorRead>().HasIndex(r => r.Value);
            modelBuilder.Entity<SensorRead>().HasIndex(r => r.UtcTime);
            modelBuilder.Entity<SensorRead>().HasIndex(r => r.LocalTime);
            modelBuilder.Entity<SensorRead>().HasIndex(r => r.TimeCode);
            modelBuilder.Entity<SensorRead>().HasIndex(r => r.StatusCode);

            modelBuilder.Entity<SensorRead>().ForSqlServerIsMemoryOptimized();
        }
    }

    public class SensorRead
    {
        public string SensorId { get; set; }
        public int SequenceNo { get; set; }
        public double Value { get; set; }
        public DateTime UtcTime { get; set; }
        public DateTime LocalTime { get; set; }
        public long TimeCode { get; set; }
        public int StatusCode { get; set; }
    }
}
